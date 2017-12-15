using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using UnityEngine;
using UnityStomp;
using System.Threading.Tasks;
using surfm.tool;

namespace RFNEet.realtimeDB {
    public class DemoManager : MonoBehaviour, RemoteApierHandler {

        private RealTimeDB db;
        void Start() {
            // StompClientAll sc = new StompClientAll("ws://127.0.0.1:7878") ;
            RoomService.CreateRoomData cd = genCreateRoomData();
            RFUtility.findAndCreateRoom<object>(cd, onOk, onFail);



        }

        private void onFail(RoomService.SurfMErrorDto sd, HTTPRequestStates s, HTTPResponse r, Exception e) {
            Debug.Log(string.Format("sd={0} s={1} r={2} e={3}", sd, s, r, e));
        }

        private void onOk(PingBundle arg1, RoomInfo<object> arg2) {
            Debug.Log("onOk=" + arg2);

            /*RemoteApier ra = new RemoteApier(arg1.genWsUrl(), arg2.roomId, this, false);
            ra.connect((d, l) => {
                Debug.Log("connect ed");
            });  */
            StompClientAll sc = new StompClientAll(arg1.genWsUrl(), PidGeter.getPid());
            sc.setOnErrorAndClose((s) => {
            }, (s) => { });
            db = new RealTimeDB(arg1.genWsUrl(), arg2.roomId);
            db.connect((d, l) => {
                Debug.Log("connect ed");
            });
            //db.init(s => { }, () => {
            //    db.createConnect();
            //    StartCoroutine(script(arg2));
            //});
        }

        private IEnumerator script(RoomInfo<object> arg2) {
            DBRefenece dr = db.createRootRef(arg2.roomId);
            addListener(dr);
            yield return new WaitForSeconds(3f);
            Task t = dr.SetValueAsync("AAAA");
            UnityUtils.setAsync(this, t, () => {
                Debug.Log("OK~");
            });
        }

        private void addListener(DBRefenece dr) {
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

        public void onNewPlayerJoined(RemoteBroadcastData broadcastData) {
            throw new NotImplementedException();
        }

        public void onRemoteFirstSync(string sid, AllSyncDataResp allData) {
            throw new NotImplementedException();
        }

        public void onPlayerLeaved(string sid, string hahverId) {
            throw new NotImplementedException();
        }

        public void onPlayerLeavedByIndex(string sid) {
            throw new NotImplementedException();
        }

        public void onRepairLostPlayer(string sid) {
            throw new NotImplementedException();
        }

        public void onErrorCb(ErrorBundle error) {
            throw new NotImplementedException();
        }

        public void onConnectClosedCb(string msg) {
            throw new NotImplementedException();
        }

        public void onBroadcast(RemoteBroadcastData data) {
            throw new NotImplementedException();
        }

        public void repairMissObject(string missWho, string moid) {
            throw new NotImplementedException();
        }

        public void onNewPlayerReadyed(PlayerDto pDto) {
            throw new NotImplementedException();
        }

        public void onRemotePlayTellMyObject(InboxTellObjectData iaod) {
            throw new NotImplementedException();
        }

        public void onServerShutdown(float cutTime) {
            throw new NotImplementedException();
        }
    }
}
