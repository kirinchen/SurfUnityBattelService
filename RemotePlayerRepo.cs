using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RFNEet {
    public class RemotePlayerRepo : PlayerRepo<RemoteObject> {

        private Func<RemotePlayerRepo, RemoteData, RemoteObject> onNewRemoteObjectCreated;
        public bool handshaked {
            get; private set;
        }

        internal RemotePlayerRepo(string pid, RemoteApier api, Func<RemotePlayerRepo, RemoteData, RemoteObject> oca) : base(pid, api) {
            Debug.Log("subscribeShooted pid=" + pid);
            onNewRemoteObjectCreated = oca;
            api.subscribeShooted(pid, onShooted);
        }

        private void onShooted(RemoteData s) {
            Debug.Log("onShooted=" + s.getSource());
            if (objectMap.ContainsKey(s.oid)) {
                objectMap[s.oid].onRemoteUpdate(s);
            } else {
                createNewObject(s);
            }
        }

        internal void createNewObject(RemoteData s) {
                Loom.QueueOnMainThread(() => {
                    RemoteObject ro = onNewRemoteObjectCreated(this, s);
                    inject(s.oid, ro);
                });
        }

        internal void handshake() {
            handshaked = true;
        }

        internal void sendToInbox(object o) {
            api.sendToInbox(pid, o);
        }
    }
}
