using UnityEngine;
using System.Collections;
using System;

namespace RFNEet {
    public class LocalObject : SyncObject<LocalObjectHandler> {

        public void post(RemoteData o) {
            api.shoot(o);
        }


    }

    public interface LocalObjectHandler {
    }
}
