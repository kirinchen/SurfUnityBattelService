using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class RemoteObjectCreaterSimple : RemoteObjectCreaterAbs {

        [System.Serializable]
        public class Bundle {
            public string name;
            public string id;
            public RemoteObject prefab;
        }

        public List<Bundle> list = new List<Bundle>();

        internal override RemoteObject create(string tag, RemoteData rd) {
            Bundle bundle = list.Find(b=> { return string.Equals(tag, b.id); });
            RemoteObject ans = Instantiate(bundle.prefab);
            ans.initAtCreated(rd);
            return ans;
        }
    }
}
