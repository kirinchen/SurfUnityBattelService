using UnityEngine;
using System.Collections;
using System;

namespace RFNEet {
    public abstract class RemoteObject : SyncObject {


        internal void update(RemoteData s) {
            if (s.getSysTag() == RemoteData.SysTag.NONE) {
                onRemoteUpdate(s);
            } else if (s.getSysTag() == RemoteData.SysTag.DELETED) {
                onRemoved();
                removeMe();
            }
        }

        internal abstract void onRemoved();

        internal abstract void onRemoteUpdate(RemoteData s);

        internal void destoryMe(bool isCallRemoveMe = false) {
            onRemoved();
            if (gameObject != null) {
                Destroy(gameObject);
            }
            if (isCallRemoveMe) {
                removeMe();
            }
        }

        public void postBroadcast(RemoteBroadcastData b) {
            setup(b);
            b.setSysTag(RemoteData.SysTag.ObjectChnage);
            api.broadcastUpdate(b);
        }

    }



}
