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
        public static readonly string SUFFIX_QUERY_ROON_MULTIPLE = "/api/v1/room/{0}/querymultiple";
        public static readonly string SUFFIX_QUERY_ROON = "/api/v1/room/{0}/query?timestamp={1}";
        internal URestApi api { get; private set; }

        public RoomService(URestApi a) {
            api = a;
        }

        public void create<T>(CreateRoomData data, Action<RoomInfo<T>> onOK, OnFail eCb) {
            api.postJson(SUFFIX_CREATE_ROON, data, (s) => {
                RoomInfo<T> r = JsonConvert.DeserializeObject<RoomInfo<T>>(s);
                onOK(r);
            }, (m, s, r, e) => {
                handleError(m, s, r, eCb);
            });
        }

        public int listRoom(string gameKindUid, object filter, Action<PingDto> cb, OnFail eCb) {
            float queryAt = Time.time;
            string path = string.Format(SUFFIX_QUERY_ROON, gameKindUid, queryAt);
            return api.postJson(path, filter, s => {
                PingDto d = JsonConvert.DeserializeObject<PingDto>(s);
                cb(d);
            }, (m, s, r, e) => {
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
            public string roomId;
            public string gameKindUid;
            public int maxPlayerCount;
            public GameKindDto gameKind;
            public object data;
        }

        public class GameKindDto {
            public string uid;
            public string name;
            public Dictionary<string, object> information;
            public int readyCount;
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