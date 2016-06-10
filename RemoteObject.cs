using UnityEngine;
using System.Collections;
using System;

namespace RFNEet {
    public abstract class RemoteObject : SyncObject {


        internal void update(RemoteData s) {
            if (s.getSysTag() == RemoteData.SysTag.NONE) {
                onRemoteUpdate(s);
            } else if (s.getSysTag() == RemoteData.SysTag.DELETED) {
                destoryMe(true);
            }
        }

        internal abstract void onRemoved();

        internal abstract void onRemoteUpdate(RemoteData s);

        internal void destoryMe(bool isCallRemoveMe = false) {
            onRemoved();
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
