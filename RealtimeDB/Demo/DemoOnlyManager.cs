using BestHTTP;
using surfm.tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityStomp;

namespace RFNEet.realtimeDB {
    public class DemoOnlyManager : MonoBehaviour {
        private RealTimeDB db;
        // Use this for initialization
        void Start() {
            RFServerStorer.getInstance().
                genPingBetter().
                getRoomFindOrCreater<object>(DemoManager.genCreateRoomData()).
                setOnDone(onOk).
                setOnFail(onFail).
                findOrCreate();
        }

        private void onOk(PingBundle pb, RoomInfo<object> ri) {
            StompClientAll sc = new StompClientAll(pb.genWsUrl(), PidGeter.getPid());
            StompIniter si = new StompIniter(sc, ri.roomId);
            db = new RealTimeDB(pb.service.api, si);

            db.init(s => { }, () => {
                db.createConnect();
                StartCoroutine(script(ri));
            });
        }


        private void onFail(RoomService.SurfMErrorDto sd, HTTPRequestStates s, HTTPResponse r, Exception e) {
            Debug.Log(string.Format("sd={0} s={1} r={2} e={3}", sd, s, r, e));
        }

        private IEnumerator script(RoomInfo<object> arg2) {
            DBRefenece dr = db.createRootRef(this,arg2.roomId);
            addListener(dr);
            yield return new WaitForSeconds(3f);
            dr.SetValueAsync("AAAA", (d, o) => {
                Debug.Log("OK~");
            });
        }

        private void addListener(DBRefenece dr) {
            dr.addValueChanged(r => {
                Debug.Log("test=" + r.getRawJsonValue());
            });
        }
    }
}
