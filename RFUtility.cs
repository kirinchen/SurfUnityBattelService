using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class RFUtility  {

        public static void findAndCreateRoom<T>(bool existJoin,RoomService.CreateRoomData ans, Action<PingBundle, RoomInfo<T>> okcb, RoomService.OnFail onFail = null) {
            RFServerStorer.getInstance().findPingBetter(ans.gameKindUid, (pb) => {
                if (existJoin && pb.bestRoom != null) {
                    okcb(pb, pb.bestRoom.to<T>());
                    return;
                }
                RoomService rs = new RoomService(pb.ua);
                createRoom<T>(rs, ans, onFail, rinfo=> {
                    okcb(pb, rinfo);
                });
            },existJoin);
        }

        private static void createRoom<T>(RoomService rs, RoomService.CreateRoomData ans, RoomService.OnFail onFail, Action<RoomInfo<T>> onOK) {
            rs.create<T>(ans, onOK, (a, s, d, f) => {
                if (onFail == null) {
                    Debug.Log("fail");
                } else {
                    onFail(a, s, d, f);
                }
            });
        }
    }
}
