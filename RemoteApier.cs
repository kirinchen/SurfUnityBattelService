﻿using UnityEngine;
using System.Collections;
using UnityStomp;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace RFNEet {
    public class RemoteApier {
        private static readonly string KEY_ME_ID = "meId";
        private static readonly string KEY_SESSION_ID = "sessionId";
        private static readonly string KEY_SENDER_ID = "senderId";
        public static readonly string KEY_TYPE = "type";
        public static readonly string KEY_TYPE_NEW_PLAYER_JOINED = "NewPlayerJoined";
        private StompClient sc;
        internal string roomId {
            get; private set;
        }
        internal string meId {
            get; private set;
        }
        private Action<string, List<string>> handshakeCb;
        internal Action<string> onNewPlayerJoined;
        internal Action<string, AllSyncDataResp> onRemoteFirstSync;
        internal Action<string> onPlayerLeaved;
        internal Action<ErrorBundle> onErrorCb;

        public RemoteApier(string url, string roomId) {
            sc = new StompClientAll(url);
            sc.setOnError((s)=> {
                throwErrorBundle(ErrorBundle.Type.SeverError,s);
            });
            meId = sc.getSessionId();
            this.roomId = roomId;
        }

        public void connect(Action<string, List<string>> handshakeCb) {
            this.handshakeCb = handshakeCb;
            sc.StompConnect(onConnected);
        }

        private void throwErrorBundle(ErrorBundle.Type t,string msg) {
            ErrorBundle eb = new ErrorBundle(t);
            eb.message = msg;
            onErrorCb(eb);
        }

        private void onConnected(object o) {
            sc.Subscribe("/user/message/errors/", (message) => {
                Debug.Log("/message/errors/" + message);
                throwErrorBundle(ErrorBundle.Type.Runtime,message);
            });
            sc.Subscribe("/app/" + roomId + "/joinBattle", (message) => {
                Debug.Log("joinBattle="+message);
                parseHandshake(message);
            });
            sc.Subscribe("/message/rooms/" + roomId + "/broadcast", (message) => {
                Dictionary<string, object> d = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
                string ts = d[KEY_TYPE].ToString();
                if (ts.Equals(KEY_TYPE_NEW_PLAYER_JOINED)) {
                    string newSid = d[KEY_SESSION_ID].ToString();
                    onNewPlayerJoined(newSid);
                }
            });
            sc.Subscribe("/message/rooms/" + roomId + "/player/leave", (message) => {
                string sid = parse(message,KEY_SESSION_ID).ToString();
                onPlayerLeaved(sid);
            });

            sc.Subscribe("/message/rooms/" + roomId + "/player/" + meId + "/inbox", (message) => {
                Debug.Log("subscribeInbox=" + message);
                AllSyncDataResp asdr = JsonConvert.DeserializeObject<AllSyncDataResp>(message);
                string sid = asdr.senderId;
                onRemoteFirstSync(sid, asdr);
            });

        }

        

        private void parseHandshake(string msg) {
            HandshakeDto d = JsonConvert.DeserializeObject<HandshakeDto>(msg);
            if (d.success) {
                meId = d.meId;
                handshakeCb(meId, d.information.playList);
            } else {
                throwErrorBundle(ErrorBundle.Type.HandShake,d.exceptionName);
            }
        }

        public void subscribeShooted(string pid, Action<RemoteData> cb) {
            sc.Subscribe("/message/rooms/" + roomId + "/player/" + pid + "/shooted", (message) => {
                RemoteData rd = JsonConvert.DeserializeObject<RemoteData>(message);
                rd.setSource(message);
                cb(rd);
            });
        }

        public void sendToInbox(string sendTo,object o) {
            string path = "/app/" + roomId + "/send/"+ sendTo;
            send(path,o);
        }

        public void send(string path,object o) {
            string json = JsonConvert.SerializeObject(o);
            sc.SendMessage(path, json);
        }

        public static object parse(string msg, string key) {
            Dictionary<string, object> d = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
            return d[key];

        }

        internal void shoot(object o) {
            string path = "/app/" + roomId + "/shoot";
            send(path,o);            
        }
    }

    public class HandshakeDto {
        public string meId;
        public Information information;
        public bool success;
        public string exceptionName;

        public class Information {
            public List<string> playList;
        }

    }
}



