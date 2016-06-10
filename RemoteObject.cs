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

        internal abstract void onRemoteUpdate(RemoteData s);

        public void postBroadcast(RemoteBroadcastData b) {
            setup(b);
            b.setSysTag(RemoteData.SysTag.ObjectChnage);
            api.broadcastUpdate(b);
        }

        internal override void postRemoveData() {
            RemoteBroadcastData b = new RemoteBroadcastData();
            setup(b);
            b.setSysTag(RemoteData.SysTag.DELETED);
            api.broadcastUpdate(b);
        }

    }



}
