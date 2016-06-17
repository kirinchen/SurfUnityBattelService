using UnityEngine;
using System.Collections;
using System;

namespace RFNEet {
    public abstract class CommRemoteObject : RemoteObject {
        internal string creator;

        internal void postInitDto() {
            RemoteData rbd = genInitDto();
            rbd.setSysTag(RemoteData.SysCmd.NEW_OBJECT);
            post(rbd);
        }

        public void post(RemoteData rbd) {
            rbd.sid = api.meId;
            setup(rbd);
            api.shootWithPid(rbd,pid);
        }

        public override RemoteData setup(RemoteData rd) {
            rd.sid = api.meId;
            return base.setup(rd);
        }

        public abstract RemoteData genInitDto();
    }
}
