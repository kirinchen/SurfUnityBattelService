using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class RFUtility {

        public static void findAndCreateRoom<T>(RoomService.CreateRoomData ans, Action<PingBundle, RoomInfo<T>> okcb, RoomService.OnFail onFail = null) {
            RFServerStorer.getInstance().findPingBetter(ans.gameKindUid, null, (pb) => {
                PingDtoFinder.RoomScore rsore = pb.getBestRoom();
                if (rsore != null) {
                    okcb(rsore.pingBundle, rsore.room.to<T>());
                    return;
                }
                PingDtoFinder.ServerScore servs = pb.getBestServer();
                RoomService rs = new RoomService(servs.api);
                createRoom<T>(rs, ans, onFail, rinfo => {
                    okcb(servs.pingBundle, rinfo);
                });
            }, r => {
                return string.Equals(ans.roomId, r.roomId);
            });
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
