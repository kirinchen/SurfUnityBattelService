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
        private static readonly string KEY_HANDOVER_ID = "handoverId";
        private static readonly string KEY_SENDER_ID = "senderId";
        public static readonly string KEY_TYPE = "type";
        public static readonly string KEY_TYPE_NEW_PLAYER_JOINED = "NewPlayerJoined";
        public static readonly string KEY_TYPE_GENERAL = "General";
        private StompClient sc;
        internal string roomId {
            get; private set;
        }
        internal string meId {
            get; private set;
        }
        internal float lastSyncServerTime;
        internal float lastSyncLocalTime;
        private Action<string, List<string>> handshakeCb;
        internal Action<string> onNewPlayerJoined;
        internal Action<string, AllSyncDataResp> onRemoteFirstSync;
        internal Action<string, string> onPlayerLeaved;
        internal Action<ErrorBundle> onErrorCb;
        internal Action<RemoteBroadcastData> onBroadcast;

        public RemoteApier(string url, string roomId) {
            sc = new StompClientAll(url);
            sc.setOnError((s) => {
                throwErrorBundle(ErrorBundle.Type.SeverError, s);
            });
            meId = sc.getSessionId();
            this.roomId = roomId;
        }

        public void connect(Action<string, List<string>> handshakeCb) {
            this.handshakeCb = handshakeCb;
            sc.StompConnect(onConnected);
        }

        private void throwErrorBundle(ErrorBundle.Type t, string msg) {
            ErrorBundle eb = new ErrorBundle(t);
            eb.message = msg;
            onErrorCb(eb);
        }

        private void onConnected(object o) {
            sc.Subscribe("/user/message/errors/", (message) => {
                Debug.Log("/message/errors/" + message);
                throwErrorBundle(ErrorBundle.Type.Runtime, message);
            });
            sc.Subscribe("/app/" + roomId + "/joinBattle/" + Time.time, (message) => {
                parseHandshake(message);
            });
            sc.Subscribe("/message/rooms/" + roomId + "/broadcast", (message) => {
                RemoteBroadcastData d = JsonConvert.DeserializeObject<RemoteBroadcastData>(message);
                d.setSource(message);
                if (KEY_TYPE_NEW_PLAYER_JOINED.Equals(d.type)) {
                    onNewPlayerJoined(d.senderId);
                } else if (KEY_TYPE_GENERAL.Equals(d.type)) {
                    onBroadcast(d);
                }
            });
            sc.Subscribe("/message/rooms/" + roomId + "/player/leave", (message) => {
                Dictionary<string, string> d = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
                string sid = d[KEY_SESSION_ID];
                string hid = d[KEY_HANDOVER_ID];
                onPlayerLeaved(sid, hid);
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
                setupSyncTime(d.playedTime, d.stamp);
                meId = d.meId;
                handshakeCb(meId, d.information.playList);
            } else {
                throwErrorBundle(ErrorBundle.Type.HandShake, d.exceptionName);
            }
        }

        private void setupSyncTime(long playedTime, string stamp) {
            float beforeLT = float.Parse(stamp);
            lastSyncLocalTime = Time.time;
            lastSyncServerTime = playedTime * 0.001f;
            float lDT_half = (lastSyncLocalTime - beforeLT) * 0.5f;
            lastSyncServerTime += lDT_half;
        }

        public float getCurrentServerTime() {
            float lD = Time.time - lastSyncLocalTime;
            return lastSyncServerTime + lD;
        }

        public void subscribeShooted(string pid, Action<RemoteData> cb) {
            sc.Subscribe("/message/rooms/" + roomId + "/player/" + pid + "/shooted", (message) => {
                RemoteData rd = JsonConvert.DeserializeObject<RemoteData>(message);
                rd.setSource(message);
                cb(rd);
            });
        }

        public void sendToInbox(string sendTo, object o) {
            string path = "/app/" + roomId + "/send/" + sendTo;
            send(path, o);
        }

        public void send(string path, object o) {
            string json = JsonConvert.SerializeObject(o);
            sc.SendMessage(path, json);
        }

        internal void close() {
            sc.CloseWebSocket();
            handshakeCb = null;
            onNewPlayerJoined = null;
            onRemoteFirstSync = null;
            onPlayerLeaved = null;
            onErrorCb = null;
            onBroadcast = null;
        }

        public static object parse(string msg, string key) {
            Dictionary<string, object> d = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
            return d[key];

        }

        internal void shoot(object o) {
            string path = "/app/" + roomId + "/shoot";
            send(path, o);
        }

        internal void broadcastUpdate(RemoteBroadcastData b) {
            string path = "/app/" + roomId + "/broadcast";
            send(path, b);
        }

    }

    public class HandshakeDto {
        public string meId;
        public Information information;
        public bool success;
        public string stamp;
        public long playedTime;
        public string exceptionName;

        public class Information {
            public List<string> playList;
        }
    }
}



