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

        internal virtual void initInject(string pid, string oid, RemoteApier api, SyncObjectListener listener) {
            this.api = api;
            this.pid = pid;
            this.oid = oid;
            this.listener = listener;
        }

        public virtual RemoteData setup(RemoteData rd) {
            rd.oid = oid;
            rd.pid = pid;
            return rd;
        }

        public virtual void removeMe() {
            listener.onRemoveMe(this);
        }

        public void updateByBroadcast(RemoteBroadcastData rbd) {
            if (gameObject != null) {
                if (rbd.getSysTag() == RemoteData.SysCmd.NONE) {
                    updateByBroadcast(rbd.btag, rbd);
                } else if (rbd.getSysTag() == RemoteData.SysCmd.DELETED) {
                    destoryMe(true, rbd);
                }
            }
        }

        internal abstract void onRemoved(RemoteData rd);

        internal void destoryMe(bool isCallRemoveMe = false, RemoteData rd = null) {
            if (!_destoryedMe) {
                _destoryedMe = true;
                onRemoved(rd);
                if (isCallRemoveMe) {
                    removeMe();
                }
            }
        }

        /*void OnDestroy() {
            try {
                postRemoveSelf();
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }*/

        public void postRemoveSelf(object target = null) {
            if (!_destoryedMe) {
                _destoryedMe = true;
                postRemoveData(target);
                removeMe();
            }
        }

        public virtual void updateByBroadcast(string btag, RemoteBroadcastData rbd) {
        }

        internal abstract void postRemoveData(object target);
    }

    public interface SyncObjectListener {
        void onRemoveMe(SyncObject so);
    }

}