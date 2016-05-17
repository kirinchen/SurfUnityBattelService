using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityStomp;

namespace RFNEet {
    public class LocalPlayerRepo {
        private RemoteApier api;
        private string pid;
        private Dictionary<string, LocalObject> objectMap = new Dictionary<string, LocalObject>();


        internal LocalPlayerRepo(string pid, RemoteApier api) {
            this.pid = pid;
            this.api = api;
        }


        public LocalObject inject(GameObject go) {
            string oid = UidUtils.getRandomString(7);
            LocalObject ans = go.AddComponent<LocalObject>();
            ans.init(pid, oid, api);
            objectMap.Add(ans.oid, ans);
            return ans;
        }


    }
}