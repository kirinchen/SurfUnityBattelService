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
        private static readonly string KEY_HANDOVER_ID = "handoverId";
        private static readonly string KEY_SENDER_ID = "senderId";
        private static readonly string KEY_SYSTEM = "system";
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
        private RemoteApierHandler handler;

        public RemoteApier(string url, string roomId, RemoteApierHandler handler, bool localDebug) {
            this.handler = handler;
            sc = localDebug ? (StompClient)new StompClientDebug(url) : (StompClient)new StompClientAll(url);
            sc.setOnErrorAndClose((s) => {
                try {
                    throwErrorBundle(ErrorBundle.Type.SeverError, s);
                } catch (Exception e) {
                    Debug.Log(e);
                }
            }, handler.onConnectClosedCb);
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
            handler.onErrorCb(eb);
        }

        private void onConnected(object o) {
            sc.Subscribe("/user/message/errors/", (message) => {
                Debug.Log("/message/errors/" + message);
                throwErrorBundle(ErrorBundle.Type.Runtime, message);
            });
            sc.Subscribe("/app/" + roomId + "/joinBattle/" + Time.time, (message) => {
                Debug.Log("joinBattle=" + message);
                parseHandshake(message);
            });
            sc.Subscribe("/message/rooms/" + roomId + "/broadcast", (message) => {
                RemoteBroadcastData d = JsonConvert.DeserializeObject<RemoteBroadcastData>(message);
                d.setSource(message);
                if (KEY_TYPE_NEW_PLAYER_JOINED.Equals(d.type)) {
                    handler.onNewPlayerJoined(d);
                } else if (KEY_TYPE_GENERAL.Equals(d.type)) {
                    try {
                        handler.onBroadcast(d);
                    } catch (Exception e) {
                        Debug.Log(e);
                    }
                }
            });
            sc.Subscribe("/message/rooms/" + roomId + "/player/leave", (message) => {
                Dictionary<string, string> d = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
                string sid = d[KEY_SESSION_ID];
                string hid = d[KEY_HANDOVER_ID];
                handler.onPlayerLeaved(sid, hid);
            });

            sc.Subscribe("/message/rooms/" + roomId + "/player/" + meId + "/inbox", (message) => {
                InboxData id = JsonConvert.DeserializeObject<InboxData>(message);
                if (id.type == InboxData.Type.AllData) {
                    AllSyncDataResp asdr = JsonConvert.DeserializeObject<AllSyncDataResp>(message);
                    string sid = asdr.senderId;
                    handler.onRemoteFirstSync(sid, asdr);
                } else if (id.type == InboxData.Type.MissObject) {
                    InboxMissData imd = JsonConvert.DeserializeObject<InboxMissData>(message);
                    handler.repairMissObject(imd.missWho, imd.moid);
                } else if (id.type == InboxData.Type.ObjectMsg) {
                    InboxTellObjectData iaod = JsonConvert.DeserializeObject<InboxTellObjectData>(message);
                    iaod.setSource(message);
                    handler.onRemotePlayTellMyObject(iaod);
                }
            });

            sc.Subscribe("/message/rooms/" + roomId + "/player/" + meId + "/sysinbox", (message) => {
                Debug.Log("subscribeSysinbox=" + message);
                SysInboxDto sdto = JsonConvert.DeserializeObject<SysInboxDto>(message);
                Debug.Log("sdto type=" + sdto.type);
                if (sdto.type == SysInboxDto.Type.SurplusPlayerList) {
                    foreach (string lsid in sdto.surplusList) {
                        handler.onPlayerLeavedByIndex(lsid);
                    }
                } else if (sdto.type == SysInboxDto.Type.LostPlayerList) {
                    handler.onRepairLostPlayer(sdto.lostPlayerId);
                }
            });

            sc.Subscribe("/message/rooms/" + roomId + "/player/ready", (message) => {
                Debug.Log("player/ready=" + message);
                string sid = parse(message, KEY_SESSION_ID).ToString();
                if (!sid.Equals(meId)) {
                    handler.onNewPlayerReadyed(sid);
                }
            });

            send("/app/" + roomId + "/ready", null);

        }

        internal void checkPlayerList(List<string> ps) {
            string path = "/app/" + roomId + "/v2/checklist/";
            send(path, ps);
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

        public void syncTime() {
            NtpDto n = new NtpDto();
            n.stamp = Time.time + "";
            n.playedTime = (long)(getCurrentServerTime() * 1000);
            sc.SendMessage("/app/" + roomId + "/ntp", JsonConvert.SerializeObject(n), "/user/message/ntp", (message) => {
                SyncTimeDto std = JsonConvert.DeserializeObject<SyncTimeDto>(message);
                setupSyncTime(std.playedTime, std.timeStamp);
            });
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

        internal void unSubscribeShooted(string pid) {
            sc.unSubscribe("/message/rooms/" + roomId + "/player/" + pid + "/shooted");
        }

        public void sendToInbox(string sendTo, object o) {
            string path = "/app/" + roomId + "/send/" + sendTo;
            send(path, o);
        }

        public void send(string path, object o) {
            string json = "";
            if (o != null) {
                json = JsonConvert.SerializeObject(o);
            }
            sc.SendMessage(path, json);
        }

        internal void close() {
            sc.CloseWebSocket();
            handshakeCb = null;
            handler = null;
        }

        public static object parse(string msg, string key) {
            Dictionary<string, object> d = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
            return d[key];

        }

        internal void shoot(object o) {
            string path = "/app/" + roomId + "/shoot";
            send(path, o);
        }

        internal void shootWithPid(object o, string pid) {
            string path = "/app/" + roomId + "/shoot/" + pid;
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



