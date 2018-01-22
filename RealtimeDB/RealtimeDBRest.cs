using Newtonsoft.Json;
using surfm.tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
namespace RFNEet.realtimeDB {
    public class RealtimeDBRest : MonoBehaviour {

        public static readonly string URL_ROOT = "/api/v1/db/";
        public static readonly string URL_SUBSCRIBE_INT = "{0}/fetch/{1}";
        public static readonly string URL_SET_VALUE = "set";

        internal URestApi api { get; private set; }
        internal string roomId { get; private set; }

        public RealtimeDBRest(string roomId, URestApi a) {
            this.roomId = roomId;
            api = a;
        }

        public void fetchNode(string path, Action<DBResult> a) {
            path = RealTimeDB.ROOT_KEY + path;
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
                Debug.Log("fetchNode=" + se.databaseError);
                a(se);
            });
        }

        public static SendEventArgs parseDBResult(string s) {
            SendEventArgs ans = JsonConvert.DeserializeObject<SendEventArgs>(s);
            return ans;
        }

        internal void setValue(string path, object value, Action<bool, object> cb) {
            Debug.Log("setValue=" + value);
            path = RealTimeDB.ROOT_KEY + path;
            string url = URL_ROOT + URL_SET_VALUE;
            SetValueDto dto = new SetValueDto();
            dto.roomId = roomId;
            dto.path = path;
            dto.value = value;
            api.postJson(url, dto, s => {
                if (cb != null) cb(true, s);
            }, new URestApi.OnErrorB(b => { if (cb != null) cb(false, b); }).onError);
        }
    }
}
