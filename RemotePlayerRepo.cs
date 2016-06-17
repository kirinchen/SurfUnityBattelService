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
            onNewRemoteObjectCreated = oca;
            api.subscribeShooted(pid, onShooted);
        }

        internal virtual void onShooted(RemoteData s) {

            if (objectMap.ContainsKey(s.oid)) {
                objectMap[s.oid].update(s);
            } else if(s.getSysTag() == RemoteData.SysCmd.NEW_OBJECT) {
                createNewObject(s);
            }
        }

        internal void createNewObject(RemoteData s) {
            if (!hasObjectById(s.oid)) {
                RemoteObject ro = onNewRemoteObjectCreated(this, s);
                setupNewObject(s, ro);
                if (ro != null) {
                    inject(s.oid, ro);
                }
            }
        }

        internal virtual void setupNewObject(RemoteData s, RemoteObject ro) {
        }

        internal void handshake() {
            handshaked = true;
        }

        internal void sendToInbox(object o) {
            api.sendToInbox(pid, o);
        }

        internal void destoryAll() {
            foreach (string k in objectMap.Keys) {
                RemoteObject ro = objectMap[k];
                ro.destoryMe();
            }
            objectMap.Clear();
        }

        internal void addAll(RemotePlayerRepo rpr, Func<RemoteObject, bool> cf) {
            foreach (RemoteObject ro in rpr.objectMap.Values) {
                if (cf(ro)) {
                    inject(ro.oid, ro);
                } else {
                    ro.destoryMe();
                }
            }
        }
    }
}
