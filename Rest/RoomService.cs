using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;
using BestHTTP;
using System.Collections.Generic;

namespace RFNEet {
    public class RoomService {

        private static readonly string KEY_CPU = "p";

        public delegate void QueryResult<T>(List<RoomInfo<T>> r);
        public delegate void OnFail(SurfMErrorDto sd, HTTPRequestStates s, HTTPResponse r, Exception e = null);

        public static readonly string SUFFIX_CREATE_ROON = "/api/v1/room/";
        public static readonly string SUFFIX_QUERY_ROON = "/api/v1/room/{0}/querymultiple";
        internal URestApi api { get; private set; }

        public RoomService(URestApi a) {
            api = a;
        }

        public void create(CreateRoomData data, Action<string> onOK, OnFail eCb) {
            api.postJson(SUFFIX_CREATE_ROON, data, onOK, (m, s, r,e) => {
                handleError(m, s, r, eCb);
            });
        }

        public int query<T>(List<string> gameKindUids, object filter, QueryResult<T> qr, OnFail eCb) {
            float queryAt = Time.time;
            string path = string.Format(SUFFIX_QUERY_ROON, JsonConvert.SerializeObject(gameKindUids));
            return api.postJsonForHttpResp(path, filter, (resp) => {
                string msg = resp.DataAsText;
                List<RoomInfo<T>> list = JsonConvert.DeserializeObject<List<RoomInfo<T>>>(msg);
                float cpu = .5f;
                try { cpu = float.Parse(resp.GetHeaderValues(KEY_CPU)[0]); } catch (Exception e) { Debug.Log(e); }
                list.ForEach(r => {
                    r.ping = Time.time - queryAt;
                    r.cpu = cpu;
                });
                qr(list);
            }, (m, s, r,e) => {
                handleError(m, s, r, eCb);
            });
        }

        private void handleError(string m, HTTPRequestStates s, HTTPResponse r, OnFail eCb) {
            try {
                SurfMErrorDto d = SurfMErrorDto.parse(r.DataAsText);
                eCb(d, s, r);
            } catch (Exception e) {
                eCb(null, s, r, e);
            }
        }

        public class CreateRoomData {
            public string gameKindUid;
            public int maxPlayerCount;
        }

        public class SurfMErrorDto {
            public long timestamp;
            public int status;
            public string error;
            public string exception;
            public string message;
            public string path;

            public enum SurfMError {
                NONE
            }

            public bool isSuccess() {
                return status >= 200 && status < 300;
            }

            public SurfMError getSurfMError() {
                return SurfMError.NONE;
            }

            public static SurfMErrorDto parse(string s) {
                return JsonConvert.DeserializeObject<SurfMErrorDto>(s);
            }

            public override string ToString() {
                return JsonConvert.SerializeObject(this);
            }

        }

    }
}