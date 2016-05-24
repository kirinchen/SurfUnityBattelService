using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityStomp;

namespace RFNEet {
    public class LocalPlayerRepo : PlayerRepo<LocalObject, LocalObjectHandler> {


        internal LocalPlayerRepo(string pid, RemoteApier api) : base(pid, api) {
        }

        public override LocalObject inject(GameObject go, LocalObjectHandler h) {
            LocalObject ans= base.inject(go,h);
            return ans;
        }

        internal override LocalObject injectComponent(GameObject go) {
            return go.AddComponent<LocalObject>();
        }

        
    }
}