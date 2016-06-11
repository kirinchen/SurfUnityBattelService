using UnityEngine;
using System.Collections;
using System;

namespace RFNEet {
    public abstract class RemoteObject : SyncObject {

        internal void update(RemoteData s) {
            if (s.getSysTag() == RemoteData.SysCmd.NONE) {
                onRemoteUpdate(s);
            } else if (s.getSysTag() == RemoteData.SysCmd.DELETED) {
                destoryMe(true,s);
            }
        }

        internal abstract void onRemoteUpdate(RemoteData s);

        public void postBroadcast(RemoteBroadcastData b) {
            setup(b);
            b.setSysTag(RemoteData.SysCmd.ObjectChnage);
            api.broadcastUpdate(b);
        }

        internal override void postRemoveData(object t) {
            RemoteBroadcastData b = null;
            if (t != null && t is RemoteBroadcastData) {
                b = (RemoteBroadcastData)t;
            } else {
                b = new RemoteBroadcastData();
            }
            setup(b);
            b.setSysTag(RemoteData.SysCmd.DELETED);
            api.broadcastUpdate(b);
        }

    }



}
