using UnityEngine;
using System.Collections;
using System;

namespace RFNEet {
    public class LocalObject : SyncObject<LocalObjectHandler> {

        public void post(RemoteData o) {
            api.shoot(o);
        }

        internal void postCreateData() {
            RemoteData o = new RemoteData(pid,oid);
            o = handler.getHandlerData(o);
            post(o);
        }
    }

    public interface LocalObjectHandler {
        RemoteData getHandlerData(RemoteData rd);
    }
}
