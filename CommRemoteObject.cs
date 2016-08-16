using UnityEngine;
using System.Collections;
using System;

namespace RFNEet {
    public abstract class CommRemoteObject : RemoteObject {
        internal string creator {
            get; private set;
        }


        public Action<string, string> onCreatorChnaged = (o, n) => { };
        internal void setCreator(string nc) {
            string orgC = creator;
            creator = nc;
            onCreatorChnaged(orgC, creator);
        }

        internal void postInitDto() {
            RemoteData rbd = genInitDto();
            rbd.setSysTag(RemoteData.SysCmd.NEW_OBJECT);
            post(rbd);
        }

        public void post(RemoteData rbd) {
            rbd.sid = api.meId;
            setup(rbd);
            api.shootWithPid(rbd, pid);
        }

        public override RemoteData setup(RemoteData rd) {
            try {
                rd.sid = api.meId;
                return base.setup(rd);
            } catch (Exception e) {
                return rd;
            }
        }

        public abstract RemoteData genInitDto();
    }
}
