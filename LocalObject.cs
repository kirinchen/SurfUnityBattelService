using UnityEngine;
using System.Collections;
using System;

namespace RFNEet {
    public abstract class LocalObject : SyncObject {

        public void post(RemoteData o) {
            setup(o);
            api.shoot(o);
        }

        internal override void postRemoveData(object t) {
            RemoteData rd = null;
            if (t != null && t is RemoteData) {
                rd = (RemoteData)t;
            } else {
                 rd = new RemoteData();
            }
            rd.setSysTag(RemoteData.SysCmd.DELETED);
            post(rd);
        }

    }
}
