
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RFNEet {
    public class RemotePlayerRepo : PlayerRepo<RemoteObject> {

        private List<string> suspendFindMissObjs = new List<string>();
        private List<string> _buffMissObjs = new List<string>();
        private Func<RemotePlayerRepo, RemoteData, RemoteObject> onNewRemoteObjectCreated;
        public bool handshaked
        {
            get; private set;
        }

        internal RemotePlayerRepo(string pid, RemoteApier api, Func<RemotePlayerRepo, RemoteData, RemoteObject> oca) : base(pid, api) {
            onNewRemoteObjectCreated = oca;
            api.subscribeShooted(pid, onShooted);
        }

        internal void addSuspendFindMissObjs(string s) {
            suspendFindMissObjs.Add(s);
        }

        internal virtual void onShooted(RemoteData s) {
            if (objectMap.ContainsKey(s.oid)) {
                objectMap[s.oid].update(s);
            } else if (s.getSysTag() == RemoteData.SysCmd.NEW_OBJECT) {
                createNewObject(s);
            } else {
                handleMissObject(s);
            }
        }

        private void handleMissObject(RemoteData s) {
            if (!handshaked || suspendFindMissObjs.Contains(s.oid) || checkBuffMissObj(s.oid)) return;
            string missOid = s.oid;
            InboxMissData imd = new InboxMissData(missOid, api.meId);
            api.sendToInbox(s.pid, imd);
        }

        private bool checkBuffMissObj(string oid) {
            if (_buffMissObjs.Contains(oid)) return true;
            bool cb = _buffMissObjs.Count == 0;
            _buffMissObjs.Add(oid);
            if (cb) {
                SyncCenter.getInstance().StartCoroutine(clearBuffMissObjList());
            }
            return false;
        }

        private IEnumerator clearBuffMissObjList() {
            while (_buffMissObjs.Count > 0) {
                yield return new WaitForSeconds(1.25f);
                _buffMissObjs.Clear();
            }
            Debug.Log("clearBuffMissObjList");
        }

        public class NotRefindObjException : Exception {
            Exception e;
            public NotRefindObjException(string msg, Exception e) : base(msg, e) { }
        }
        internal void createNewObject(RemoteData s) {
            if (hasObjectById(s.oid)) {
                objectMap[s.oid].update(s);
            } else if (!string.IsNullOrEmpty(s.pid) && !string.IsNullOrEmpty(s.oid)) {
                try {
                    RemoteObject ro = onNewRemoteObjectCreated(this, s);
                    if (ro != null) {
                        setupNewObject(s, ro);
                        inject(s.oid, ro);
                    }
                } catch (NotRefindObjException re) {
                    addSuspendFindMissObjs(s.oid);
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

        internal void destoryAll(bool unSubscribe = true) {
            foreach (string k in objectMap.Keys) {
                RemoteObject ro = objectMap[k];
                ro.destoryMe();
            }
            objectMap.Clear();
            if (unSubscribe) {
                api.unSubscribeShooted(pid);
            }
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
