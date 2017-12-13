using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStomp;

namespace RFNEet.realtimeDB {
    public class RealTimeDB : DBInit {
        public static string ROOT_KEY = "<ROOT>";
        private StompClient client;
        public string meId { get; private set; }
        public string roomId { get; private set; }
        public bool ok { get; private set; }
        private Dictionary<string, NodeRef> nMap = new Dictionary<string, NodeRef>();

        public RealTimeDB(StompClient sc) {

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

        internal List<Action<DBResult>> listen(ListenType t, string path) {
            string url = string.Format("/message/rooms/{0}/{1}/{2}", roomId, ROOT_KEY + path, t);
            List<Action<DBResult>> ans = new List<Action<DBResult>>();
            client.Subscribe(url, d => {
                NodeResult nr = JsonConvert.DeserializeObject<NodeResult>(d);
                ans.ForEach(a => {
                    a(nr);
                });
            });
            return ans;
        }

        public bool isOK() {
            return ok;
        }
    }
}
