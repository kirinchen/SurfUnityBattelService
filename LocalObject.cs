using UnityEngine;
using System.Collections;
using System;

namespace RFNEet {
    public abstract class LocalObject : SyncObject {

        public void post(RemoteData o) {
            setup(o);
            api.shoot(o);
        }

        internal override void postRemoveData() {
            RemoteData rd = new RemoteData();
            rd.setSysTag(RemoteData.SysTag.DELETED);
            post(rd);
        }

    }
}
