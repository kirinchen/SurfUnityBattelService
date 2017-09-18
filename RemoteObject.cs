using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace RFNEet {
    public abstract class RemoteObject : SyncObject {

        private List<Action<RemoteData>> onRemoteUpdateCbs = new List<Action<RemoteData>>();
        public void addOnRemoteUpdateCbs(Action<RemoteData> cb) { onRemoteUpdateCbs.Add(cb); }
        public void removeOnRemoteUpdateCbs(Action<RemoteData> cb) { onRemoteUpdateCbs.Remove(cb); }

        private List<Func<RemoteData, bool>> onUpdateOnceCalls = new List<Func<RemoteData, bool>>();
        public void addOnUpdateOnceCall(Func<RemoteData, bool> f) {
            onUpdateOnceCalls.Add(f);
        }

        //RemoteObjectCreaterSimple call 
        internal virtual void initAtCreated(RemoteData rd) {        }

        internal void update(RemoteData s) {
            try {
                if (s.getSysTag() == RemoteData.SysCmd.NONE) {
                    onRemoteUpdate(s);
                    onRemoteUpdateCbs.ForEach(cb => { cb(s); });
                    onUpdateOnceCalls.RemoveAll(f => { return f(s); });
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
            try {
                RemoteBroadcastData b = null;
                if (t != null && t is RemoteBroadcastData) {
                    b = (RemoteBroadcastData)t;
                } else {
                    b = new RemoteBroadcastData();
                }
                setup(b);
                b.setSysTag(RemoteData.SysCmd.DELETED);
                api.broadcastUpdate(b);
            } catch (Exception e) {
                Debug.Log(e);
            }
        }

    }



}
