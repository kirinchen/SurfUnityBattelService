using UnityEngine;
using System.Collections;
using System;
using UnityStomp;
using BestHTTP.WebSocket;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnityStomp {
    public class StompClientDebug : StompClient {
        private static readonly string JOINED_MSG = "{\"information\":{\"gameId\":\"TankIO\",\"playList\":[\"4A9CTaZO\"],\"initCount\":3},\"meId\":\"4A9CTaZO\",\"stamp\":\"1.200768\",\"index\":0,\"playedTime\":112908817,\"success\":true,\"exceptionName\":null}";
        private string sessionId;

        public StompClientDebug(string connectString) {
            sessionId = UidUtils.getRandomString(8);
        }

        public void CloseWebSocket() {
        }

        public string getSessionId() {
            return sessionId;
        }

        public void SendMessage(string destination, string message) {
        }

        public void SetCookie(string name, string value) {
        }

        public void setOnError(Action<string> errorCb) {
        }

        public void StompConnect(Action<object> openAction) {
            openAction(null);
        }

        public void Subscribe(string destination, OnMessageListener act) {
            if (match(destination, "/app/.+/joinBattle/.+")) {
                act(JOINED_MSG);
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
