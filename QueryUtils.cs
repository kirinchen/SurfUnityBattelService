﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

        public SyncObject findObject(string pid,string oid) {
            if (sc.api.meId.Equals(pid)) {
                if (sc.localRepo.objectMap.ContainsKey(oid)) {
                    return sc.localRepo.objectMap[oid];
                } else {
                    return null;
                }
            } else if (sc.remoteRepos.ContainsKey(pid)) {
                RemotePlayerRepo rpr = sc.remoteRepos[pid];
                Dictionary<string, RemoteObject> m = rpr.getMap();
                if (m.ContainsKey(oid)) {
                    return m[oid];
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }
    }
}
