using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;
using BestHTTP;
using System.Collections.Generic;

namespace RFNEet {
    public class RoomService {
        public delegate void QueryResult<T>(List<RoomInfo<T>> r);
        public delegate void OnFail(SurfMErrorDto sd, HTTPRequestStates s, HTTPResponse r, Exception e = null);

        public static readonly string SUFFIX_CREATE_ROON = "/api/v1/room/";
        public static readonly string SUFFIX_QUERY_ROON = "/api/v1/room/{0}/query?pageIndex={1}&pageSize={2}";
        internal URestApi api { get; private set; }

        public RoomService(URestApi a) {
            api = a;
        }

        public void create(CreateRoomData data, Action<string> onOK, OnFail eCb) {
            api.postJson(SUFFIX_CREATE_ROON, data, onOK, (m, s, r) => {
                handleError(m, s, r, eCb);
            });
        }

        public int query<T>(string gameKindUid, int pageIndex, int pageSize, object filter, QueryResult<T> qr, OnFail eCb) {
            string path = string.Format(SUFFIX_QUERY_ROON, gameKindUid, pageIndex, pageSize);
            return api.postJson(path, filter, (msg) => {
                List<RoomInfo<T>> list = JsonConvert.DeserializeObject<List<RoomInfo<T>>>(msg);
                qr(list);
            }, (m, s, r) => {
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