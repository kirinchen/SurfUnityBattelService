using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace RFNEet {
    public abstract class RemoteObject : SyncObject {

        private List<Action<RemoteData>> onRemoteUpdateCbs = new List<Action<RemoteData>>();
        public void addOnRemoteUpdateCbs(Action<RemoteData> cb) { onRemoteUpdateCbs.Add(cb); }
        public void removeOnRemoteUpdateCbs(Action<RemoteData> cb) { onRemoteUpdateCbs.Remove(cb); }

        internal void update(RemoteData s) {
            try {
                if (s.getSysTag() == RemoteData.SysCmd.NONE) {
                    onRemoteUpdate(s);
                    onRemoteUpdateCbs.ForEach(cb => { cb(s); });
                } else if (s.getSysTag() == RemoteData.SysCmd.DELETED) {
                    destoryMe(true, s);
                }
            } catch (Exception e) {
                Debug.LogWarning(e);
            }
        }

        internal abstract void onRemoteUpdate(RemoteData s);

        public void postBroadcast(RemoteBroadcastData b) {
            setup(b);
            api.broadcastUpdate(b);
        }

        public void tellLocalObject(InboxTellObjectData iod) {
            iod.oid = oid;
            api.sendToInbox(pid, iod);
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
