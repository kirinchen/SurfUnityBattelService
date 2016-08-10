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
                    base.onShooted(s);
                }
            }
        }

        public CommRemoteObject create(CommRemoteObject cro) {
            cro.setCreator(api.meId);
            string oid = UidUtils.getRandomString(SyncCenter.OID_SIZE);
            inject(oid, cro);
            cro.postInitDto();
            return cro;
        }

        internal override void setupNewObject(RemoteData s, RemoteObject ro) {
            CommRemoteObject cro = (CommRemoteObject)ro;
            cro.setCreator(s.sid);
        }


    }
}
