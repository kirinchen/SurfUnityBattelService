using UnityEngine;
using System.Collections;
using System;

namespace RFNEet {
    public abstract class CommRemoteObject : RemoteObject {
        internal string creator;

        internal void postInitDto() {
            RemoteData rbd = genInitDto();
            post(rbd);
        }

        private void post(RemoteData rbd) {
            rbd.sid = api.meId;
            setup(rbd);
            api.shootWithPid(rbd,pid);
        }

        public abstract RemoteData genInitDto();
    }
}
