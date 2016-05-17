using UnityEngine;
using System.Collections;
using UnityStomp;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace RFNEet {
    public class RemoteApier  {
        private static readonly string KEY_ME_ID = "meId";
        private StompClient sc;
        internal string roomId {
            get;private set;
        }
        internal string meId {
            get; private set;
        }
        private Action handshakeCb;

        public RemoteApier(string url, string roomId) {
            sc = new StompClientAll(url);
            this.roomId = roomId;
        }

        public void connect(Action handshakeCb) {
            this.handshakeCb = handshakeCb;
            sc.StompConnect(onConnected);
        }

        private void onConnected(object o) {
            sc.Subscribe("/app/" + roomId + "/joinBattle", (message) => {
                message = message.Substring(0, message.Length - 3);
                meId = parse(message, KEY_ME_ID).ToString();
                Loom.QueueOnMainThread(handshakeCb);
            });
            /*sc.Subscribe("/message/rooms/" + roomId + "/player/ready", (message) => {
            });*/
            sc.Subscribe("/message/rooms/" + roomId + "/broadcast", (message) => {

            });
            sc.Subscribe("/message/rooms/" + roomId + "/player/leave", (message) => {

            });
        }

        private void subscribeInbox() {
            sc.Subscribe("/message/rooms/" + roomId + "/player/"+meId+"/inbox", (message) => {

            });
        }


        public static object parse(string msg, string key) {
            Dictionary<string,object> d = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
            return d[key];

        }

    }
}



