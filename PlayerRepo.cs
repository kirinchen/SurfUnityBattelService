using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStomp;

namespace RFNEet {
    public abstract class PlayerRepo<T> where T : SyncObject  {
        internal RemoteApier api;
        internal string pid;
        internal Dictionary<string, T> objectMap = new Dictionary<string, T>();

        internal PlayerRepo(string pid, RemoteApier api) {
            this.pid = pid;
            this.api = api;
        }

        public virtual T inject(T o) {
            string oid = UidUtils.getRandomString(7);
            o.init(pid, oid, api);
            objectMap.Add(o.oid, o);
            return o;
        }


    }
}
