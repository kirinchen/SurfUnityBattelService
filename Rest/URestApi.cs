﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Text;
using BestHTTP;

public class URestApi : MonoBehaviour {

    public class ErrorBundle {
        public string error;
        public HTTPRequestStates s;
        public HTTPResponse resp;
        public Exception e;

        public override string ToString() {
            return string.Format("error={0} s={1} resp={2} e={3}", error, s, resp, e);
        }
    }

    public delegate void OnError(string error, HTTPRequestStates s, HTTPResponse resp, Exception e);

    public delegate void OnErrorBundle(ErrorBundle eb);

    public class OnErrorB {

        protected OnErrorBundle ob;

        public OnErrorB(OnErrorBundle o) {
            ob = o;
        }

        public virtual void onError(string error, HTTPRequestStates s, HTTPResponse resp, Exception e) {
            ErrorBundle eb = new ErrorBundle();
            eb.s = s;
            eb.error = error;
            eb.resp = resp;
            eb.e = e;
            if (!onCustom(eb)) {
                ob(eb);
            }
        }

        protected virtual bool onCustom(ErrorBundle eb) {
            return false;
        }
    }

    public string host;
    public string port;
    public float timeOut = 5f;
    private int genId;
    private Dictionary<int, RequestBundle> map = new Dictionary<int, RequestBundle>();
    public string authorization;

    public struct B {
        public bool noned;
        public string host;
        public string port;
        public float timeOut;
        public string authorization;
    }





    void Start() { }

    public void abortAll() {
        foreach (RequestBundle rb in map.Values) {
            rb.www.Abort();
            rb.www.Dispose();
        }
        map.Clear();
    }

    public int get(string url, Action<string> onOk, OnError onError) {
        Uri u = new Uri(getUrl(url));
        HTTPRequest hr = new HTTPRequest(u, HTTPMethods.Get);
        return runWWWW(hr, (w) => { onOk(w.DataAsText); }, onError);
    }

    private int runWWWW(HTTPRequest hr, Action<HTTPResponse> onOk, OnError oe) {
        setupHeaders(hr);
        OnFinishedHandler oh = new OnFinishedHandler(onOk, oe);
        hr.Callback = oh.onFinished;
        hr.ConnectTimeout = new TimeSpan((long)(10000000 * timeOut));
        int id = getId();
        RequestBundle rb = new RequestBundle(hr);
        map.Add(id, rb);
        hr.Send();
        return id;
    }

    class OnFinishedHandler {
        private Action<HTTPResponse> onOk;
        private OnError oe;
        public OnFinishedHandler(Action<HTTPResponse> k, OnError e) { onOk = k; oe = e; }

        public void onFinished(HTTPRequest req, HTTPResponse resp) {
            // Increase the finished count regardless of the state of our request
            string msg = "";
            switch (req.State) {
                // The request finished without any problem.
                case HTTPRequestStates.Finished:
                    if (resp.IsSuccess) {
                        try {
                            onOk(resp);
                        } catch (Exception e) {
                            oe(msg, req.State, resp, e);
                        }
                    } else {
                        msg =
                       (string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                                       resp.StatusCode,
                                                       resp.Message,
                                                       resp.DataAsText));
                        oe(msg, req.State, resp, null);
                    }
                    break;
                // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
                case HTTPRequestStates.Error:
                    msg = ("Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
                    oe(msg, req.State, resp, null);
                    break;

                // The request aborted, initiated by the user.
                case HTTPRequestStates.Aborted:
                    msg = ("Request Aborted!");
                    oe(msg, req.State, resp, null);
                    break;

                // Ceonnecting to the server is timed out.
                case HTTPRequestStates.ConnectionTimedOut:
                    msg = ("Connection Timed Out!");
                    oe(msg, req.State, resp, null);
                    break;

                // The request didn't finished in the given time.
                case HTTPRequestStates.TimedOut:
                    msg = ("Processing the request Timed Out!");
                    oe(msg, req.State, resp, null);
                    break;
            }
        }

    }



    public void abort(int id) {
        if (map.ContainsKey(id)) {
            RequestBundle rb = map[id];
            rb.www.Abort();
            rb.www.Dispose();
            map.Remove(id);
        }
    }

    private int getId() {
        return genId++;
    }

    public string getUrl(string urlorPath) {
        if (host != null && host.Length > 0) {
            return "http://" + host + ":" + port + "/" + urlorPath;
        } else {
            return urlorPath;
        }
    }

    public int postJsonForHttpResp(string url, object data, Action<HTTPResponse> onOk, OnError onError) {
        Uri u = new Uri(getUrl(url));
        HTTPRequest hr = new HTTPRequest(u, HTTPMethods.Post);
        if (data != null) {
            string ourPostData = JsonConvert.SerializeObject(data);
            byte[] pData = Encoding.ASCII.GetBytes(ourPostData.ToCharArray());
            hr.RawData = pData;
        }
        return runWWWW(hr, (w) => { onOk(w); }, onError);
    }

    public int postJson(string url, object data, Action<string> onOk, OnError onError) {
        Action<HTTPResponse> onWOk = (r) => { onOk(r.DataAsText); };
        return postJsonForHttpResp(url, data, onWOk, onError);
    }

    private void setupHeaders(HTTPRequest hr) {
        hr.AddHeader("charset", "utf-8");
        hr.AddHeader("Content-Type", "application/json");
        if (!string.IsNullOrEmpty(authorization)) {
            hr.AddHeader("Authorization", authorization);
        }
    }



    public int loadRes(string url, Action<HTTPResponse> tCB, OnError onError, bool usePrefix = true) {
        Uri u = new Uri(getUrl(url));
        HTTPRequest hr = new HTTPRequest(u, HTTPMethods.Get);
        return runWWWW(hr, (w) => {
            tCB(w);
        }, onError);
    }

    public B getB() {
        return new B() {
            authorization = authorization,
            host = host,
            port = port,
            timeOut = timeOut
        };
    }

    public static URestApi newInstance(B b) {
        if (b.noned) return null;
        string s = string.Format("h={0} p={1} t={2} a={3}", b.host, b.port, b.timeOut, b.authorization);
        GameObject go = new GameObject(s);
        URestApi ans = go.AddComponent<URestApi>();
        ans.host = b.host;
        ans.port = b.port;
        ans.timeOut = b.timeOut;
        ans.authorization = b.authorization;
        return ans;
    }

    public class RequestBundle {
        public HTTPRequest www;

        public RequestBundle(HTTPRequest www) {
            this.www = www;
        }
    }
}
