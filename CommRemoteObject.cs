using UnityEngine;
using System.Collections;
using System;
using System.Threading;

namespace RFNEet {
    public abstract class CommRemoteObject : RemoteObject {

        public bool autoInjectToRepo = false;
        public string specifyOid;
        private bool _injected = false;

        public void Start() {
            if (autoInjectToRepo) {
                SyncCenter.getInstance().addOnConnectedCb(injectToRepo);
            }
        }

        private void injectToRepo(CommRemoteRepo repo) {
            if (!string.IsNullOrEmpty(oid)) return;
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (SyncCenter.getInstance()._lockThreadId.Contains(threadId)) return;
            string soid = string.IsNullOrEmpty(specifyOid) ? null : specifyOid;
            repo.create(this, specifyOid);
        }

        internal void postInitDto() {
            RemoteData rbd = genInitDto();
            rbd.setSysTag(RemoteData.SysCmd.NEW_OBJECT);
            post(rbd);
        }

        public void post(RemoteData rbd) {
            rbd.sid = api.meId;
            setup(rbd);
            api.shootWithPid(rbd, pid);
        }

        public override RemoteData setup(RemoteData rd) {
            try {
                rd.sid = api.meId;
                return base.setup(rd);
            } catch (Exception e) {
                return rd;
            }
        }

        public abstract RemoteData genInitDto();

        public bool isOwner() {
            return SyncCenter.getInstance().queryUitls.isEnterFirstSelf();
        }

    }
}
