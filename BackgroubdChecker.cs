using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RFNEet {
    public class BackgroubdChecker : MonoBehaviour {

        private RemoteApier api;
        private SyncCenter center;

        internal void init(SyncCenter cc) {
            center = cc;
            api = cc.api;
            InvokeRepeating("routineCheckPlayerList", 11, 11);
            InvokeRepeating("routineCheckServerTime", 17, 17);
        }

        void routineCheckServerTime() {
            api.syncTime();
        }

        void routineCheckPlayerList() {
            List<string> l = new List<string>(center.remoteRepos.Keys);
            l.Remove(CommRemoteRepo.COMM_PID);
            api.checkPlayerList(l);
        }

        internal static void start(SyncCenter sc) {
            BackgroubdChecker bc = sc.gameObject.AddComponent<BackgroubdChecker>();
            bc.init(sc);
        }

    }
}
