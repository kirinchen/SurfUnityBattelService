using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStomp;

namespace RFNEet {
    public abstract class PlayerRepo<T,H> where T : SyncObject<H>  {
        internal RemoteApier api;
        internal string pid;
        internal Dictionary<string, T> objectMap = new Dictionary<string, T>();

        internal PlayerRepo(string pid, RemoteApier api) {
            this.pid = pid;
            this.api = api;
        }

        public virtual T inject(GameObject go, H handler) {
            string oid = UidUtils.getRandomString(7);
            T ans = injectComponent(go);
            ans.init(pid, oid, api, handler);
            objectMap.Add(ans.oid, ans);
            return ans;
        }

        internal abstract T injectComponent(GameObject go);

    }
}
