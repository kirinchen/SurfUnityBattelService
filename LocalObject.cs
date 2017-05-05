using UnityEngine;
using System.Collections;
using System;

namespace RFNEet {
    public abstract class LocalObject : SyncObject {

        private float _lastPostInitDtoAt = -9999;

        internal void postInitDto() {
            _lastPostInitDtoAt = Time.time;
            RemoteData rbd = genInitDto();
            rbd.setSysTag(RemoteData.SysCmd.NEW_OBJECT);
            post(rbd);
        }

        internal bool postInitDtoSafe() {
            if ((Time.time - _lastPostInitDtoAt) > 1) {
                postInitDto();
                return true;
            } else {
                return false;
            }
        }

        public float getLastPostInitDtoAt() {
            return _lastPostInitDtoAt;
        }

        internal abstract RemoteData genInitDto();

        public void post(RemoteData o) {
            setup(o);
            if (api != null) {
                api.shoot(o);
            }
        }

        internal override void postRemoveData(object t) {
            RemoteData rd = null;
            if (t != null && t is RemoteData) {
                rd = (RemoteData)t;
            } else {
                 rd = new RemoteData();
            }
            rd.setSysTag(RemoteData.SysCmd.DELETED);
            post(rd);
        }

        public virtual void onRemoteTellSelf(InboxTellObjectData iaod) {
        }
    }
}
