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

        public LocalObject create(LocalObject cro) {
            string oid = UidUtils.getRandomString(7);
            inject(oid, cro);
            cro.postInitDto();
            return cro;
        }

        public virtual LocalObject inject(LocalObject o) {
            string oid = UidUtils.getRandomString(7);
            return inject(oid, o);
        }

        internal void addAll(RemotePlayerRepo rpr, Func<RemoteObject, LocalObject> convert) {
            foreach (RemoteObject ro in rpr.objectMap.Values) {
                LocalObject lo = convert(ro);
                if (lo != null) {
                    inject(ro.oid, lo);
                } else {
                    ro.destoryMe();
                }
            }
        }
    }
}