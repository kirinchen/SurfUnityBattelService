using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RFNEet {
    public class QueryUtils {
        private SyncCenter sc;
        internal QueryUtils(SyncCenter sc) {
            this.sc = sc;
        }

        public string firstSortPid() {
            List<string> l = new List<string>(sc.remoteRepos.Keys);
            l.Remove(CommRemoteRepo.COMM_PID);
            l.Add(sc.api.meId);
            l.Sort((a, b) => { return a.GetHashCode().CompareTo(b.GetHashCode()); });
            return l[0];
        }

        public int getRealRemoteRepoCount() {
            List<string> l = new List<string>(sc.remoteRepos.Keys);
            l.Remove(CommRemoteRepo.COMM_PID);
            return l.Count;
        }

        public bool isFirstBySelf() {
            return firstSortPid().Equals(sc.api.meId);
        }

    }
}
