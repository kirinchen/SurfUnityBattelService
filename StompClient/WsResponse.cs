using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using WebSocketSharp;	//https://github.com/sta/websocket-sharp
using Newtonsoft.Json;
using System.Text.RegularExpressions;


namespace UnityStomp {
    public class WsResponse {

        private string plain;
        private string destination;
        private string messageId;
        private string contentType;
        private string subscription;
        private int contentLength;
        private string message;

        public WsResponse(string data) {
            plain = Regex.Unescape(data);
        }

        public bool parse() {
            if (plain.IndexOf("a[\"MESSAGE") == 0) {
                parseData();
                return true;
            }
            return false;
        }

        private void parseData() {
            string[] sArray = plain.Split('\n');
            int i = 0;
            foreach (string s in sArray) {
                if (s.IndexOf("a") == 0) {
                    //DO noting
                } else if (s.IndexOf("destination:") == 0) {
                    destination = s.Replace("destination:", "");
                } else if (s.IndexOf("message-id:") == 0) {
                    messageId = s.Replace("message-id:", "");
                } else if (s.IndexOf("content-type:") == 0) {
                    contentType = s.Replace("content-type:", "");
                } else if (s.IndexOf("subscription:") == 0) {
                    subscription = s.Replace("subscription:", "");
                } else if (s.IndexOf("content-length:") == 0) {
                    contentLength = Convert.ToInt32(s.Replace("content-length:", ""));
                } else if (i == (sArray.Length - 1)) {
                    message = s;
                }
                i++;
            }
        }

        public string getMessage() {
            return message;
        }

        public string getDestination() {
            return destination;
        }

    }
}

