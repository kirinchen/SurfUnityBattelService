using UnityEngine;
using System.Collections;
using System;

namespace RFNEet {
    public abstract class RemoteObject : SyncObject {


        internal abstract void onRemoteUpdate(RemoteData s);
    }

}
