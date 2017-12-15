using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStomp;

namespace RFNEet.realtimeDB {
    public class RealTimeDB : DBInit {
        public static string ROOT_KEY = "@ROOT";
        private StompClient sc;
        public string meId { get; private set; }
        public string roomId { get; private set; }
        public bool ok { get; private set; }
        public RealtimeDBRest rest { get; private set; }
        public URestApi api { get; private set; }
        private Dictionary<string, NodeRef> nMap = new Dictionary<string, NodeRef>();
        private Action initializeFirebase;

        public RealTimeDB(URestApi api, StompClient c, string roomId) {
            sc = c;
            this.api = api;
            rest = new RealtimeDBRest(roomId, api);
            meId = sc.getSessionId();
            this.roomId = roomId;
        }

        public void init(Action<string> onFailInitializeFirebase, Action initializeFirebase) {
            this.initializeFirebase = initializeFirebase;
            sc.setOnErrorAndClose(onFailInitializeFirebase, onFailInitializeFirebase);
            sc.StompConnect(onConnected);
        }

        private void onConnected(object o) {
            sc.Subscribe("/user/message/errors/", (message) => {
                Debug.Log("/message/errors/" + message);
            });
            sc.Subscribe("/app/" + roomId + "/joinBattle/" + Time.time, (message) => {
                Debug.Log("joinBattle=" + message);
                initializeFirebase();
            });
            sc.Subscribe("/message/rooms/" + roomId + "/broadcast", (message) => {
                RemoteBroadcastData d = JsonConvert.DeserializeObject<RemoteBroadcastData>(message);
            });
            sc.Subscribe("/message/rooms/" + roomId + "/player/leave", (message) => {
            });

            sc.Subscribe("/message/rooms/" + roomId + "/player/" + meId + "/inbox", (message) => {

            });

            sc.Subscribe("/message/rooms/" + roomId + "/player/" + meId + "/sysinbox", (message) => {
                Debug.Log("subscribeSysinbox=" + message);

            });

            sc.Subscribe("/message/rooms/" + roomId + "/player/ready", (message) => {
                Debug.Log("player/ready=" + message);
            });

            send("/app/" + roomId + "/ready", null);

        }


        public void send(string path, object o) {
            string json = "";
            if (o != null) {
                json = JsonConvert.SerializeObject(o);
            }
            sc.SendMessage(path, json);
        }

        public void createConnect() {

        }

        public DBRefenece createRootRef(string roomId) {
            return getNode("");
        }

        internal NodeRef getNode(string path) {
            if (!nMap.ContainsKey(path)) {
                NodeRef nr = new NodeRef(this, path);
                nMap.Add(path, nr);
            }
            return nMap[path];
        }

        internal void unlisten(ListenType t, string path) {
            string url = string.Format("/message/rooms/{0}/db/{1}/{2}", roomId, ROOT_KEY + path, t);
            sc.unSubscribe(url);
        }

        internal List<Action<DBResult>> listen(ListenType t, string path, Action<DBResult> initA) {
            string url = string.Format("/message/rooms/{0}/db/{1}/{2}", roomId, ROOT_KEY + path, t);
            Debug.Log("statrt listen=" + t + " url=" + url);
            List<Action<DBResult>> ans = new List<Action<DBResult>>();
            ans.Add(initA);
            sc.Subscribe(url, d => {
                Debug.Log("listen=" + d);
                DBResult nr = RealtimeDBRest.parseDBResult(d);
                ans.ForEach(a => { a(nr); });
            });
            if (t == ListenType.ValueChange) {
                rest.fetchNode(path, n => {
                    ans.ForEach(a => { a(n); });
                });
            }
            return ans;
        }



        public bool isOK() {
            return ok;
        }

        public static string getPathByDirs(List<string> l) {
            string ans = string.Empty;
            l.ForEach(s => { ans += "/" + s; });
            return ans;
        }

        public static List<string> getPathDirs(string path) {
            string[] ans = path.Split('/');
            return new List<string>(ans);
        }
    }
}
