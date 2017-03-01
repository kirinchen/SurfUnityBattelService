using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStomp;
using System;

namespace RFNEet {
    public abstract class PlayerRepo<T> : SyncObjectListener where T : SyncObject  {
        private long startAt;
        internal RemoteApier api;
        internal string pid;
        internal Dictionary<string, T> objectMap = new Dictionary<string, T>();

        internal PlayerRepo(string pid, RemoteApier api) {
            this.pid = pid;
            this.api = api;
        }

        public virtual T inject(string oid,T o) {
            //string oid = UidUtils.getRandomString(7);
            o.initInject(pid, oid, api,this);
            objectMap.Add(oid, o);
            return o;
        }

        public bool hasObjectById(string oid) {
            return objectMap.ContainsKey(oid);
        }

        public void onRemoveMe(SyncObject so) {
            objectMap.Remove(so.oid);
        }

        public Dictionary<string, T> getMap() {
            return new Dictionary<string, T>(objectMap) ;
        }

        public long getStartAt() {
            return startAt;
        }

        public void setStartAt(long startAt) {
            this.startAt = startAt;
        }

    }

}
