using UnityEngine;
using System.Collections;
using System;
using UnityStomp;

namespace RFNEet {
    public class CommRemoteRepo : RemotePlayerRepo {

        public static readonly string COMM_PID = "@C";

        internal CommRemoteRepo(RemoteApier api, Func<RemotePlayerRepo, RemoteData, RemoteObject> oca) : base(COMM_PID, api, oca) {
            pid = COMM_PID;
        }

        internal override void onShooted(RemoteData s) {
            if (!api.meId.Equals(s.sid)) {
                try {
                    base.onShooted(s);
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
        }

        public CommRemoteObject create(CommRemoteObject cro, string specifyOid = null) {
            bool noneSpecify = string.IsNullOrEmpty(specifyOid);
            if (noneSpecify) {
                specifyOid = UidUtils.getRandomString(SyncCenter.OID_SIZE);
            } else {
                if (hasObjectById(specifyOid)) return (CommRemoteObject)get(specifyOid);
            }

            inject(specifyOid, cro);
            if (noneSpecify) {
                cro.postInitDto();
            }
            return cro;
        }

    }
}
