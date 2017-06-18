using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RFNEet {
    public class QueryUtils {
        private SyncCenter sc;
        internal QueryUtils(SyncCenter sc) {
            this.sc = sc;
        }

        public List<string> sortPids(List<string> skips = null) {
            List<string> l = new List<string>(sc.remoteRepos.Keys);
            l.Remove(CommRemoteRepo.COMM_PID);
            if (skips != null) {
                l.RemoveAll((p) => {
                    return skips.Contains(p);
                });
            }
            l.Add(sc.api.meId);
            l.Sort((a, b) => { return a.GetHashCode().CompareTo(b.GetHashCode()); });
            return l;
        }



        public bool isCommDataTeller(int count) {
            List<string> l = sortPids();
            count = count > l.Count ? l.Count : count;
            for (int i = 0; i < count; i++) {
                if (l[i].Equals(sc.api.meId)) {
                    return true;
                }
            }
            return false;
        }

        public int getRealRemoteRepoCount() {
            List<string> l = new List<string>(sc.remoteRepos.Keys);
            l.Remove(CommRemoteRepo.COMM_PID);
            return l.Count;
        }

        public bool isIdFirstSelf() {
            return sortPids()[0].Equals(sc.api.meId);
        }



        public bool isEnterFirstSelf() {
            return isEnterFirst(sc.localRepo.pid);
        }

        public SyncObject findObject(string pid, string oid) {
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

        public string getEnterFirst() {
            if (isEnterFirst(sc.localRepo.pid)) return sc.localRepo.pid;
            foreach (RemotePlayerRepo rpr in sc.remoteRepos.Values) {
                if (!string.Equals(rpr.pid, CommRemoteRepo.COMM_PID) && isEnterFirst(rpr.pid)) return rpr.pid;
            }
            throw new NullReferenceException("not find getEnterFirst");
        }

        public bool isEnterFirst(string pid) {
            float meStartAt = -1;
            bool self = false;
            if (string.Equals(sc.localRepo.pid, pid)) {
                meStartAt = sc.localRepo.getStartAt();
                self = true;
            } else {
                meStartAt = sc.remoteRepos[pid].getStartAt();
            }
            foreach (RemotePlayerRepo rpr in sc.remoteRepos.Values) {
                if (!string.Equals(rpr.pid, CommRemoteRepo.COMM_PID) && !string.Equals(rpr.pid, pid)) {
                    if (rpr.getStartAt() < meStartAt) {
                        return false;
                    }
                }
            }
            if (!self && sc.localRepo.getStartAt() < meStartAt) {
                return false;
            }
            return true;
        }
    }
}
