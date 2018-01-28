using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class RoomFindOrCreater<T> {

        public delegate void OnDone(PingBundle pb, RoomInfo<T> ri);

        private PingBetter pingBetter;
        private RoomService.CreateRoomData createData;
        private bool findExist = true;
        private OnDone onDone;
        private RoomService.OnFail onFail = (a, s, d, f) => {
            Debug.Log(string.Format("a={0} s={1} d={2} f={3}", a, s, d, f));
        };

        public RoomFindOrCreater(RoomService.CreateRoomData d, PingBetter p) {
            createData = d;
            pingBetter = p;
        }

        public RoomFindOrCreater<T> setOnDone(OnDone od) {
            onDone = od;
            return this;
        }

        public RoomFindOrCreater<T> setOnFail(RoomService.OnFail f) {
            onFail = f;
            return this;
        }

        public RoomFindOrCreater<T> disableFindExist() {
            findExist = false;
            return this;
        }

        public void create() {
            disableFindExist();
            pingBetter.addOnDone(onFinded).ping(createData.gameKindUid);
        }

        public void findOrCreate() {
            pingBetter.addOnDone(onFinded).ping(createData.gameKindUid);
        }

        private void onFinded(PingDtoFinder pb) {
            if (findExist) {
                PingDtoFinder.RoomScore rsore = pb.getBestRoom();
                if (rsore != null) {
                    if (onDone != null) onDone(rsore.pingBundle, rsore.room.to<T>());
                    return;
                }
            }
            PingDtoFinder.ServerScore servs = pb.getBestServer();
            RoomService rs = new RoomService(servs.api);
            createRoom(rs, servs.pingBundle);
        }

        private void createRoom(RoomService rs, PingBundle pingBundle) {
            if (onFail == null) {
                onFail = (a, s, d, f) => {
                    Debug.Log(string.Format("a={0} s={1} d={2} f={3}", a, s, d, f));
                };
            }
            rs.create<T>(createData, rinfo => {
                onDone(pingBundle, rinfo);
            }, onFail);
        }

    }
}
