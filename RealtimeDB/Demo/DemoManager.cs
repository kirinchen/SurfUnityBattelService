using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using UnityEngine;
using UnityStomp;
using System.Threading.Tasks;
using surfm.tool;

namespace RFNEet.realtimeDB {
    public class DemoManager : AbsSyncHandler {

        private RealTimeDB db;
        private RoomInfo<object> roomI;
        void Start() {
            // StompClientAll sc = new StompClientAll("ws://127.0.0.1:7878") ;
            RoomService.CreateRoomData cd = genCreateRoomData();
            //RFUtility.findAndCreateRoom<object>(cd, onOk, onFail);
            RFServerStorer.getInstance().
                genPingBetter().
                getRoomFindOrCreater<object>(cd).
                setOnDone(onOk).
                setOnFail(onFail).
                findOrCreate();

        }

        private void onFail(RoomService.SurfMErrorDto sd, HTTPRequestStates s, HTTPResponse r, Exception e) {
            Debug.Log(string.Format("sd={0} s={1} r={2} e={3}", sd, s, r, e));
        }

        private void onOk(PingBundle arg1, RoomInfo<object> arg2) {
            Debug.Log("onOk=" + arg2);
            roomI = arg2;
            connect(true, arg1.service.api, arg2.roomId);
        }


        public override void onAllRemotePlayerReadyed(LocalPlayerRepo localRepo) {
            base.onAllRemotePlayerReadyed(localRepo);
            db = SyncCenter.getInstance().repo;
            db.init(s => { }, () => {
                db.createConnect();
                StartCoroutine(script(roomI));
            });
        }

        private IEnumerator script(RoomInfo<object> arg2) {
            DBRefenece dr = db.createRootRef(arg2.roomId);
            addListener(dr);
            yield return new WaitForSeconds(3f);
            dr.SetValueAsync("AAAA", (b, o) => {
                Debug.Log("OK~");
            });
        }

        private void addListener(DBRefenece dr) {
            dr.addValueChanged(r => {
                Debug.Log("test=" + r.getRawJsonValue());
            });
        }

        public static RoomService.CreateRoomData genCreateRoomData() {
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
