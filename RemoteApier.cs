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
        private static readonly string KEY_SYSTEM = "system";
        public static readonly string KEY_TYPE = "type";
        public static readonly string KEY_TYPE_NEW_PLAYER_JOINED = "NewPlayerJoined";
        public static readonly string KEY_TYPE_GENERAL = "General";
        public static readonly string KEY_TYPE_SHOTDOWN = "ShutDown";
        private StompClient sc { get { return stompIniter.client; } }
        internal string roomId { get { return stompIniter.roomId; } }
        internal string meId { get; private set; }
        private StompIniter stompIniter;
        internal float lastSyncServerTime;
        internal float lastSyncLocalTime;
        public float ping { get; private set; }
        private Action<HandshakeDto, List<string>> handshakeCb;
        private RemoteApierHandler handler;
        private bool localDebug = false;

        public RemoteApier(StompIniter si, RemoteApierHandler handler) {
            this.handler = handler;
            stompIniter = si;
            sc.setOnErrorAndClose((s) => {
                try {
                    throwErrorBundle(ErrorBundle.Type.SeverError, s);
                } catch (Exception e) {
                    Debug.Log(e);
                }
            }, handler.onConnectClosedCb);
            meId = sc.getSessionId();
        }

        public void connect(Action<HandshakeDto, List<string>> handshakeCb) {
            this.handshakeCb = handshakeCb;
            stompIniter.
                addOnConnects(onConnected).
                addOnHandshake(parseHandshake).
                connect();
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
                } else if (KEY_TYPE_SHOTDOWN.Equals(d.type)) {
                    handler.onServerShutdown(d.cutTime);
                }
            });
            sc.Subscribe("/message/rooms/" + roomId + "/player/leave", (message) => {
                try {
                    Dictionary<string, string> d = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
                    string sid = d[KEY_SESSION_ID];
                    string hid = d[KEY_HANDOVER_ID];
                    Debug.Log("/player/leave" + message);
                    handler.onPlayerLeaved(sid, hid);
                } catch (Exception e) {
                    Debug.Log(e);
                }
            });

            sc.Subscribe("/message/rooms/" + roomId + "/player/" + meId + "/inbox", (message) => {
                InboxData id = JsonConvert.DeserializeObject<InboxData>(message);
                if (id.type == InboxData.Type.AllData) {
                    AllSyncDataResp asdr = JsonConvert.DeserializeObject<AllSyncDataResp>(message);
                    string sid = asdr.senderId;
                    handler.onRemoteFirstSync(sid, asdr);
                } else if (id.type == InboxData.Type.MissObject) {
                    Debug.Log("Miss obj t=" + Time.time);
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
                PlayerDto pdto = JsonConvert.DeserializeObject<PlayerDto>(message);
                if (!pdto.sessionId.Equals(meId)) {
                    handler.onNewPlayerReadyed(pdto);
                }
            });
        }

        internal void checkPlayerList(List<string> ps) {
            string path = "/app/" + roomId + "/v2/checklist/";
            send(path, ps);
        }

        private void parseHandshake(string msg) {
            HandshakeDto d = JsonConvert.DeserializeObject<HandshakeDto>(msg);
            if (d.success) {
                setupSyncTime(d.playedTime, d.stamp);
                handshakeCb(d, d.information.playList);
            } else {
                throwErrorBundle(ErrorBundle.Type.HandShake, d.exceptionName);
            }
        }

        internal float debugLocalMonitorPing;
        public void syncTime() {

            if (localDebug) {
                lastSyncLocalTime = Time.time;
                lastSyncServerTime = Time.time;
                ping = 0;
                return;
            }
            NtpDto n = new NtpDto();
            float t = Time.time;
            n.stamp = t + "";
            n.playedTime = (long)(getCurrentServerTime() * 1000);
            sc.SendMessage("/app/" + roomId + "/ntp", JsonConvert.SerializeObject(n), "/user/message/ntp", (message) => {
                debugLocalMonitorPing = (Time.time - t) * 0.5f;
                SyncTimeDto std = JsonConvert.DeserializeObject<SyncTimeDto>(message);
                setupSyncTime(std.playedTime, std.timeStamp);
            });
        }

        private void setupSyncTime(long playedTime, string stamp) {
            float beforeLT = float.Parse(stamp);
            lastSyncLocalTime = Time.time;
            lastSyncServerTime = playedTime * 0.001f;
            ping = (lastSyncLocalTime - beforeLT) * 0.5f;
            lastSyncServerTime += ping;
            Debug.Log("lastSyncServerTime=" + lastSyncServerTime);
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
            StompIniter.send(sc, path, o);
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

        public void command(CommandDto.Command cmd) {
            string path = "/app/" + roomId + "/command";
            CommandDto cdt = new CommandDto(cmd);
            send(path, cdt);
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
        public long meStartAt;
        public Information information;
        public bool success;
        public string stamp;
        public long playedTime;
        public string exceptionName;
        public int currentCount;
        public int maxPlayerCount;
        public Dictionary<string, object> data = new Dictionary<string, object>();

        public class Information {
            public List<string> playList;
        }
    }
}



