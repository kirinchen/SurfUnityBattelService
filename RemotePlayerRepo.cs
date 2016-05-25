using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RFNEet {
    public class RemotePlayerRepo : PlayerRepo<RemoteObject> {

        public bool handshaked {
            get;private set;
        }

        internal RemotePlayerRepo(string pid, RemoteApier api) : base(pid, api) {
            Debug.Log("subscribeShooted pid="+pid);
            api.subscribeShooted(pid, onShooted);
        }

        private void onShooted(RemoteData s) {

        }

        internal void handshake() {
            handshaked = true;
        }

        internal void sendToInbox(object o) {
            api.sendToInbox(pid,o);
        }
    }
}
