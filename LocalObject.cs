using UnityEngine;
using System.Collections;
using System;

namespace RFNEet {
    public class LocalObject : SyncObject {

        public void post(RemoteData o) {
            setup(o);
            api.shoot(o);
        }

        void OnDestroy() {
            RemoteData rd = new RemoteData();
            rd.setSysTag(RemoteData.SysTag.DELETED);
            post(rd);
            removeMe();
        }


    }
}
