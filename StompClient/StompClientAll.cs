using UnityEngine;
using System.Collections;
using System;
using UnityStomp;
using BestHTTP.WebSocket;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnityStomp {
    public class StompClientAll : StompClient {

        private WebSocket webSocket;
        private string sessionId;

        public WebSocket websocket;
        public static string acceptVersion = "1.1,1.0";
        public static string heartBeat = "10000,10000";
        private Action<string> onErrorCb = (s)=> { };
        private Dictionary<string, OnMessageListener> actionMap = new Dictionary<string, OnMessageListener>();
        public int subNo;


        public StompClientAll(string connectString) {
            sessionId = UidUtils.getRandomString(8);
            connectString += "/" + UidUtils.getRandomNumber(3) + "/" + sessionId + "/websocket";
            websocket = new WebSocket(new Uri(connectString));
            subNo = 0;
        }

        public void SetCookie(string name, string value) {
            //websocket.SetCookie(new WebSocketSharp.Net.Cookie(name, value));
        }

        //Stomp Connect...
        public void StompConnect(Action<object> openAction) {

            websocket.OnOpen += (sender) => {
                Debug.Log("WebSocket connect...... ");
                onOpened();
                openAction(sender);
            };

            websocket.OnMessage += (sender, e) => {
                WsResponse resp = new WsResponse(e);
                if (resp.parse()) {
                    string key = getKeyByDestination(resp.getDestination());
                    if (key != null) {
                        OnMessageListener a = actionMap[key];
                        a(resp.getMessage());
                    }
                }

            };

            websocket.OnError += (sender, e) => {
                Debug.Log("Error message : " + e.Message);
                onErrorCb(e.Message);
            };

            websocket.Open();
        }

        private string getKeyByDestination(string d) {
            foreach (string key in actionMap.Keys) {
                Regex regex = new Regex(key, RegexOptions.IgnoreCase);
                Match m = regex.Match(d);
                if (m.Success) {
                    return key;
                }
            }
            return null;
        }

        private void onOpened() {
            var connectString = StompString("CONNECT", new Dictionary<string, string>()
                                            {
                {"accept-version", acceptVersion},
                {"heart-beat", heartBeat}
            });
            websocket.Send(connectString);
        }




        //Subscribe...
        public void Subscribe(string destination) {
            var subscribeString = StompString("SUBSCRIBE", new Dictionary<string, string>()                     {
                {"id", "sub-" + subNo},
                {"destination", destination}
            });

            websocket.Send(subscribeString);
            subNo++;
        }

        public void Subscribe(string destination, OnMessageListener act) {
            actionMap.Add(destination, act);
            this.Subscribe(destination);
        }


        //Send Message
        public void SendMessage(string destination, string message) {

            //string jsonMessage = JsonConvert.SerializeObject(new { content = message });
            string jsonMessage = message;
            string contentLength = jsonMessage.Length.ToString();
            jsonMessage = jsonMessage.Replace("\"", "\\\"");

            var sendString = "[\"SEND\\n" +
                "destination:" + destination + "\\n" +
                    "content-length:" + contentLength + "\\n\\n" +
                    jsonMessage + "\\u0000\"]";

            //websocket.SendAsync(sendString, result => Console.WriteLine(result));
            websocket.Send(sendString);
        }

        //Close 
        public void CloseWebSocket() {
            websocket.Close();
        }

        public static string StompString(string method, Dictionary<string, string> content) {

            string result;

            result = "[\"" + method + "\\n";

            foreach (var item in content) {
                result = result + item.Key + ":" + item.Value + "\\n";
            }

            result = result + "\\n\\u0000\"]";

            return result;
        }

        public string getSessionId() {
            return sessionId;
        }

        public void setOnError(Action<string> errorCb) {
            onErrorCb = errorCb;
        }
    }
}
