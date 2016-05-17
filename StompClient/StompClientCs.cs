using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using WebSocketSharp;	//https://github.com/sta/websocket-sharp
using Newtonsoft.Json;
using System.Text.RegularExpressions;




namespace UnityStomp {
    public delegate void OnMessageListener(string msg);
    public class StompClientCs : StompClient {
        public WebSocket websocket;
        public static string acceptVersion = "1.1,1.0";
        public static string heartBeat = "10000,10000";

        private Dictionary<string, OnMessageListener> actionMap = new Dictionary<string, OnMessageListener>();
        public int subNo;


        public StompClientCs(string connectString) {
            connectString += "/" + UidUtils.getRandomNumber(3) + "/" + UidUtils.getRandomString(8) + "/websocket";
            websocket = new WebSocket(connectString);
            subNo = 0;
        }

        public void SetCookie(string name, string value) {
            websocket.SetCookie(new WebSocketSharp.Net.Cookie(name, value));
        }

        //Stomp Connect...
        public void StompConnect(Action<object> openAction) {

            websocket.OnOpen += (sender, e) => {
                Debug.Log("WebSocket connect...... ");
                onOpened();
                openAction(sender);
            };

            websocket.OnMessage += (sender, e) => {
                if (e.Type == Opcode.Text) {
                    WsResponse resp = new WsResponse(e.Data);
                    if (resp.parse()) {
                        string key = getKeyByDestination(resp.getDestination());
                        if (key != null) {
                            OnMessageListener a = actionMap[key];
                            a(resp.getMessage());
                        }
                    }
                    return;
                }

                if (e.Type == Opcode.Binary) {
                    var data = e.RawData;
                    return;
                }
            };

            websocket.OnError += (sender, e) => {
                Debug.Log("Error message : " + e.Message);
            };

            websocket.OnClose += (sender, e) => {
                Debug.Log("Server close code " + e.Code + " reason : " + e.Reason);
            };
            websocket.Connect();
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

    }
}

