using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RFNEet {
    public class RemotePlayerRepo : PlayerRepo<RemoteObject, RemoteObjectHandler> {


        internal RemotePlayerRepo(string pid, RemoteApier api) : base(pid, api) {
            Debug.Log("subscribeShooted pid="+pid);
            api.subscribeShooted(pid, onShooted);
        }

        private void onShooted(RemoteData s) {

        }

        internal override RemoteObject injectComponent(GameObject go) {
            return go.AddComponent<RemoteObject>();
        }

        internal void sendToInbox(object o) {
            api.sendToInbox(pid,o);
        }
    }
}
