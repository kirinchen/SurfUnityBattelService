using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class RFUtility {

        public static void findAndCreateRoom<T>(RoomService.CreateRoomData ans, Action<PingBundle, RoomInfo<T>> okcb, RoomService.OnFail onFail = null) {
            RFServerStorer.getInstance().findPingBetter(ans.gameKindUid, (pb) => {
                if (pb.bestRoom != null) {
                    okcb(pb, pb.bestRoom.to<T>());
                    return;
                }
                RoomService rs = new RoomService(pb.ua);
                createRoom<T>(rs, ans, onFail, rinfo => {
                    okcb(pb, rinfo);
                });
            }, true, ans.roomId);
        }

        private static void createRoom<T>(RoomService rs, RoomService.CreateRoomData ans, RoomService.OnFail onFail, Action<RoomInfo<T>> onOK) {
            rs.create<T>(ans, onOK, (a, s, d, f) => {
                if (onFail == null) {
                    Debug.Log(string.Format("a={0} s={1} d={2} f={3}", a, s, d, f));
                } else {
                    onFail(a, s, d, f);
                }
            });
        }
    }
}
