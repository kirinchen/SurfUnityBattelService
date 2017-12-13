using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using UnityEngine;
using UnityStomp;

namespace RFNEet.realtimeDB {
    public class DemoManager : MonoBehaviour {

        private RealTimeDB db;
        void Start() {
            // StompClientAll sc = new StompClientAll("ws://127.0.0.1:7878") ;
            RoomService.CreateRoomData cd = genCreateRoomData();
            RFUtility.findAndCreateRoom<object>(cd, onOk, onFail);

            // db = new RealTimeDB(sc);

        }

        private void onFail(RoomService.SurfMErrorDto sd, HTTPRequestStates s, HTTPResponse r, Exception e) {
            Debug.Log(string.Format("sd={0} s={1} r={2} e={3}", sd, s, r, e));
        }

        private void onOk(PingBundle arg1, RoomInfo<object> arg2) {
            Debug.Log("onOk=" + arg2);
            StompClientAll sc = new StompClientAll(arg1.genWsUrl());
            db = new RealTimeDB(sc);
            db.init(s => { }, () => {
                db.createConnect();
                DBRefenece dr = db.createRootRef(arg2.roomId);
                test(dr);
            });
        }

        private void test(DBRefenece dr) {
            dr.addValueChanged(r => {
                Debug.Log("test=" + r.getRawJsonValue());
            });
        }

        private RoomService.CreateRoomData genCreateRoomData() {
            RoomService.CreateRoomData ans = new RoomService.CreateRoomData();
            RoomService.GameKindDto gameKind = new RoomService.GameKindDto();
            gameKind.name = "Test";
            gameKind.uid = "Test";
            gameKind.readyCount = 1;
            ans.gameKind = gameKind;
            ans.maxPlayerCount = 4;
            return ans;
        }
    }
}
