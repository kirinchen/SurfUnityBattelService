using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public abstract class RemoteObjectCreaterAbs : MonoBehaviour {
        internal RemoteObject onNewRemoteObjectCreated(RemotePlayerRepo rpr, RemoteData rd) {
            RemoteObject ro = create(rd.tag, rd);
            ro.update(rd);
            return ro;
        }

        internal abstract RemoteObject create(string tag, RemoteData rd);

    }
}
