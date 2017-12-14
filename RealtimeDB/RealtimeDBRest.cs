using Newtonsoft.Json;
using surfm.tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace RFNEet.realtimeDB {
    public class RealtimeDBRest : MonoBehaviour {

        public static readonly string URL_ROOT = "/api/v1/db/";
        public static readonly string URL_SUBSCRIBE_INT = "{0}/fetch/{1}";

        internal URestApi api { get; private set; }
        internal string roomId { get; private set; }

        public RealtimeDBRest(string roomId, URestApi a) {
            this.roomId = roomId;
            api = a;
        }

        public void fetchNode(string path,  Action<DBResult> a) {
            byte[] bytes = Encoding.UTF8.GetBytes(path);
            string bp = Convert.ToBase64String(bytes);
            string url = string.Format(URL_ROOT + URL_SUBSCRIBE_INT, roomId, bp);
            api.get(url, s => {
                a(parseDBResult(s));
            }, (msg, b, s, d) => {
                SendEventArgs se = new SendEventArgs();
                se.databaseError = new URestApi.ErrorBundle() {
                    s = b,
                    e = d,
                    resp = s,
                    error = msg
                };
                a(se);
            });
        }

        public static DBResult parseDBResult(string s) {
            return JsonConvert.DeserializeObject<SendEventArgs>(s);
        }
    }
}
