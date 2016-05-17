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

        public T inject(GameObject go) {
            string oid = UidUtils.getRandomString(7);
            T ans = injectComponent(go);
            ans.init(pid, oid, api);
            objectMap.Add(ans.oid, ans);
            return ans;
        }

        internal abstract T injectComponent(GameObject go);

    }
}
