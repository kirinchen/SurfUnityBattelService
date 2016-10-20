using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;
using BestHTTP;

namespace RFNEet {
    public class RoomService  {
        
        public delegate void OnFail(SurfMErrorDto sd, HTTPRequestStates s, HTTPResponse r, Exception e = null);

        public static readonly string SUFFIX_CREATE_ROON = "/api/v1/room/";
        public URestApi api;


        public void createRoom(CreateRoomData data, Action<string> onOK, OnFail eCb) {
            api.postJson(SUFFIX_CREATE_ROON, data, onOK, (m, s, r) => {
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