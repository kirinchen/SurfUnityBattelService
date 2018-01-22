using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.realtimeDB {
    public interface DBInit {
        void init(Action<string> onFailInitializeFirebase, Action initializeFirebase);
        void createConnect();

        DBRefenece createRootRef(MonoBehaviour mb,string roomId);
        bool isOK();
    }
}
