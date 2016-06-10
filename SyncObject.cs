using UnityEngine;
using System.Collections;
using System;

namespace RFNEet {
    public abstract class SyncObject : MonoBehaviour {

        internal string pid;
        internal string oid;
        internal RemoteApier api;
        internal SyncObjectListener listener;
        private bool _destoryedMe = false;

        internal void init(string pid, string oid, RemoteApier api, SyncObjectListener listener) {
            this.api = api;
            this.pid = pid;
            this.oid = oid;
            this.listener = listener;
        }

        public RemoteData setup(RemoteData rd) {
            rd.oid = oid;
            rd.pid = pid;
            return rd;
        }

        public virtual void removeMe() {
            listener.onRemoveMe(this);
        }

        public void updateByBroadcast(RemoteBroadcastData rbd) {
            if (gameObject != null) {
                if (rbd.getSysTag() == RemoteData.SysTag.NONE) {
                    updateByBroadcast(rbd.btag, rbd);
                } else if (rbd.getSysTag() == RemoteData.SysTag.DELETED) {
                    destoryMe(true);
                }
            }
        }

        internal abstract void onRemoved();

        internal void destoryMe(bool isCallRemoveMe = false) {
            if (!_destoryedMe) {
                _destoryedMe = true;
                onRemoved();
                if (isCallRemoveMe) {
                    removeMe();
                }
            }
        }

        void OnDestroy() {
            postRemoveSelf();
        }

        public void postRemoveSelf() {
            if (!_destoryedMe) {
                _destoryedMe = true;
                postRemoveData();
                removeMe();
            }
        }

        public virtual void updateByBroadcast(string btag, RemoteBroadcastData rbd) {
        }

        internal abstract void postRemoveData();
    }

    public interface SyncObjectListener {
        void onRemoveMe(SyncObject so);
    }

}