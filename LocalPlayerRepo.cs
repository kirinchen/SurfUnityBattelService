using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityStomp;

namespace RFNEet {
    public class LocalPlayerRepo : PlayerRepo<LocalObject> {


        internal LocalPlayerRepo(string pid, RemoteApier api) : base(pid, api) {
        }

        
    }
}