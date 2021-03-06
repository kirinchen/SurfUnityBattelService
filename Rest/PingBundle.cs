﻿using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System;
using BestHTTP;

namespace RFNEet {
    public class PingBundle {
        internal bool ok { get; private set; }
        internal bool done { get; private set; }

        internal RoomService service { get; private set; }
        internal PingDto result { get; private set; }

        internal Action doneAction = () => { };
        internal object filter { get; private set; }
        internal string gameKindUid { get; private set; }
        private Predicate<PingDto.RoomI> roomFilter;

        public void reset() {
            ok = false;
            done = false;
            filter = null;
            result = null;
        }


        public PingBundle() { }
        public PingBundle(string gameKindUid, object f, URestApi ua, Predicate<PingDto.RoomI> roomFilter = null) {
            this.filter = f;
            this.gameKindUid = gameKindUid;
            service = new RoomService(ua);
            this.roomFilter = roomFilter;
        }

        public int pingOne() {
            return service.listRoom(gameKindUid, filter, pd => {
                pd.list = pd.list.FindAll(roomFilter);
                result = pd;
                result.ping = Time.time - float.Parse(pd.timestamp);
                setDone(true);
            }, onFail);
        }

        private void onFail(RoomService.SurfMErrorDto sd, HTTPRequestStates s, HTTPResponse r, Exception e) {
            setDone(false);
        }

        public string genWsUrl() {
            return genWsUrl(service.api);
        }

        public static string genWsUrl(URestApi ua) {
            if (ua == null) return "NULL";
            return "ws://" + ua.host + ":" + ua.port + "/rfws";
        }


        private void setDone(bool _ok) {
            ok = _ok;
            done = true;
            doneAction();
        }
    }
}
