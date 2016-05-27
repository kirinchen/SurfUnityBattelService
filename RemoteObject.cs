using UnityEngine;
using System.Collections;
using System;

namespace RFNEet {
    public abstract class RemoteObject : SyncObject {


        internal void update(RemoteData s) {
            onRemoteUpdate(s);
        }

        internal abstract void onRemoved();

        internal abstract void onRemoteUpdate(RemoteData s);
    }

}
