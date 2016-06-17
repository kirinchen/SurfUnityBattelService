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
            cro.creator = api.meId;
            string oid = UidUtils.getRandomString(7);
            inject(oid, cro);
            cro.postInitDto();
            return cro;
        }

        internal override void setupNewObject(RemoteData s, RemoteObject ro) {
            CommRemoteObject cro = (CommRemoteObject)ro;
            cro.creator = s.sid;
            Debug.Log("setupNewObject  cro.creator : "+ cro.creator+" sid="+ s.sid);
        }


    }
}
