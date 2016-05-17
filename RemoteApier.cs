using UnityEngine;
using System.Collections;
using UnityStomp;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace RFNEet {
    public class RemoteApier {
        private static readonly string KEY_ME_ID = "meId";
        private static readonly string KEY_SESSION_ID = "sessionId";
        private static readonly string KEY_PLAYER_LIST = "playList";
        private static readonly string KEY_INFORMATION = "information";
        private StompClient sc;
        internal string roomId {
            get; private set;
        }
        internal string meId {
            get; private set;
        }
        private Action<string, List<string>> handshakeCb;

        public RemoteApier(string url, string roomId) {
            sc = new StompClientAll(url);
            this.roomId = roomId;
        }

        public void connect(Action<string,List<string>> handshakeCb) {
            this.handshakeCb = handshakeCb;
            sc.StompConnect(onConnected);
        }

        private void onConnected(object o) {
            sc.Subscribe("/app/" + roomId + "/joinBattle", (message) => {
                message = message.Substring(0, message.Length - 3);
                parseHandshake(message);
            });
            /*sc.Subscribe("/message/rooms/" + roomId + "/player/ready", (message) => {
            });*/
            sc.Subscribe("/message/rooms/" + roomId + "/broadcast", (message) => {
                message = message.Substring(0, message.Length - 3);
                Debug.Log("broadcast="+message);
                string s = parse(message, KEY_SESSION_ID).ToString();
                Debug.Log("s=" + s);
            });
            sc.Subscribe("/message/rooms/" + roomId + "/player/leave", (message) => {

            });
        }

        private void parseHandshake(string msg) {
            HandshakeDto d = JsonConvert.DeserializeObject<HandshakeDto>(msg);
            meId = d.meId;
            subscribeInbox();
            Loom.QueueOnMainThread(()=> {
                handshakeCb(meId,d.information.playList);
            });
        }

        private void subscribeInbox() {
            sc.Subscribe("/message/rooms/" + roomId + "/player/" + meId + "/inbox", (message) => {

            });
        }

        public void subscribeShooted(string pid,Action<string> cb) {
            sc.Subscribe("/message/rooms/" + roomId + "/player/" + pid + "/shooted", (message) => {
                cb(message);
            });
        }


        public static object parse(string msg, string key) {
            Dictionary<string, object> d = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
            return d[key];

        }

    }

    public class HandshakeDto {
        public string meId;
        public Information information;

        public class Information {
            public List<string> playList;
        }

    }
}



