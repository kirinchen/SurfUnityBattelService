using UnityEngine;
using System.Collections;
using System;
using UnityStomp;
using BestHTTP.WebSocket;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnityStomp {
    public class StompClientDebug : StompClient {
        private static readonly string JOINED_MSG = "{\"roomId\":\"QRU6F\",\"gameUid\":\"TankIO2\",\"currentCount\":1,\"maxPlayerCount\":35,\"data\":null,\"information\":{\"gameId\":\"TankIO2\",\"playList\":[\"2aBdTRed\"],\"initCount\":3},\"meId\":\"2aBdTRed\",\"stamp\":\"99999999999\",\"index\":0,\"playedTime\":124024,\"success\":true,\"exceptionName\":null}";
        private static readonly string INTO_ROOM_MSG = "{\"senderId\":\"4A9CTaZO\",\"tellerIds\":[],\"sessionId\":\"4A9CTaZO\",\"type\":\"NewPlayerJoined\"}";
        private string sessionId;

        public StompClientDebug(string connectString) {
            sessionId = "4A9CTaZO";
        }

        public void CloseWebSocket() {
        }

        public string getSessionId() {
            return sessionId;
        }

        public void SendMessage(string destination, string message) {
        }

        public void SendMessage(string destination, string message, string subscribeDestination, OnMessageListener act) {
        }

        public void SetCookie(string name, string value) {
        }

        public void setOnErrorAndClose(Action<string> errorCb, Action<string> cCb) {
        }

        public void StompConnect(Action<object> openAction) {
            openAction(null);
        }

        public void Subscribe(string destination, OnMessageListener act) {
            if (match(destination, "/app/.+/joinBattle/.+")) {
                act(JOINED_MSG);
            } else if (match(destination, "/message/rooms/.+/broadcast")) {
                act(INTO_ROOM_MSG);
            }
        }

        public void unSubscribe(string destination) {
        }

        private bool match(string path, string reqx) {
            Regex rgx = new Regex(reqx, RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(path);
            return matches.Count > 0;
        }

    }
}
