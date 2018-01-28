using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStomp;

namespace RFNEet.realtimeDB {
    public class RealTimeDB : DBInit {
        public static string ROOT_KEY = "@ROOT";
        private StompClient sc { get { return stompIniter.client; } }
        public string meId { get; private set; }
        public string roomId { get { return stompIniter.roomId; } }
        private StompIniter stompIniter;
        public bool ok { get; private set; }
        public RealtimeDBRest rest { get; private set; }
        public URestApi api { get; private set; }
        private Dictionary<string, NodeRef> nMap = new Dictionary<string, NodeRef>();
        private Dictionary<string, long> _valueVersion = new Dictionary<string, long>();

        private Action initializeFirebase;

        public RealTimeDB(URestApi api, StompIniter si) {
            stompIniter = si;
            this.api = api;
            rest = new RealtimeDBRest(roomId, api);
            meId = sc.getSessionId();
        }

        public void init(Action<string> onFailInitializeFirebase, Action initializeFirebase) {
            this.initializeFirebase = initializeFirebase;
            sc.setOnErrorAndClose(onFailInitializeFirebase, onFailInitializeFirebase);
            stompIniter.addOnHandshake(m => {
                Debug.Log("joinBattle=" + m);
                initializeFirebase();
            });
            stompIniter.connect();
        }

        public void createConnect() {
        }

        public DBRefenece createRootRef(MonoBehaviour mb,string roomId) {
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
            List<Action<DBResult>> ans = new List<Action<DBResult>>();
            ans.Add(initA);
            sc.Subscribe(url, d => {
                SendEventArgs nr = RealtimeDBRest.parseDBResult(d);
                if (!checkValueVersion(url, nr)) return;
                ans.ForEach(a => { a(nr); });
            });
            if (t == ListenType.ValueChange) {
                rest.fetchNode(path, n => {
                    if (!checkValueVersion(url, (SendEventArgs)n)) return;
                    ans.ForEach(a => { a(n); });
                });
            }
            return ans;
        }

        private bool checkValueVersion(string url, SendEventArgs nr) {
            if (!_valueVersion.ContainsKey(url)) {
                _valueVersion.Add(url, nr.sid);
                return true;
            }
            long cs = _valueVersion[url];
            if (nr.sid > cs) {
                _valueVersion.Remove(url);
                _valueVersion.Add(url, nr.sid);
                return true;
            }
            return false;
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
