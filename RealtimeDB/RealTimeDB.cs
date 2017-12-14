using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStomp;

namespace RFNEet.realtimeDB {
    public class RealTimeDB : DBInit {
        public static string ROOT_KEY = "@ROOT";
        private StompClient client;
        public string meId { get; private set; }
        public string roomId { get; private set; }
        public bool ok { get; private set; }
        public RealtimeDBRest rest { get; private set; }
        public URestApi api { get; private set; }
        private Dictionary<string, NodeRef> nMap = new Dictionary<string, NodeRef>();

        public RealTimeDB(URestApi api, StompClient sc) {
            this.api = api;
            client = sc;
            meId = client.getSessionId();
        }

        public void init(Action<string> onFailInitializeFirebase, Action initializeFirebase) {
            client.setOnErrorAndClose(onFailInitializeFirebase, onFailInitializeFirebase);
            client.StompConnect((o) => {
                initializeFirebase();
                ok = true;
            });
        }



        public void createConnect() {

        }

        public DBRefenece createRootRef(string roomId) {
            rest = new RealtimeDBRest(roomId, api);
            this.roomId = roomId;
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
            client.unSubscribe(url);
        }

        internal List<Action<DBResult>> listen(ListenType t, string path, Action<DBResult> initA) {
            string url = string.Format("/message/rooms/{0}/db/{1}/{2}", roomId, ROOT_KEY + path, t);
            List<Action<DBResult>> ans = new List<Action<DBResult>>();
            ans.Add(initA);
            client.Subscribe(url, d => {
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
            l.ForEach(s=> { ans += "/" + s; });
            return ans;
        }

        public static List<string> getPathDirs(string path) {
            string[] ans = path.Split('/');
            return new List<string>( ans);
        }
    }
}
