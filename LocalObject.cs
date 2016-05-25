using UnityEngine;
using System.Collections;
using System;

namespace RFNEet {
    public class LocalObject : SyncObject {

        public void post(RemoteData o) {
            api.shoot(o);
        }


    }
}
