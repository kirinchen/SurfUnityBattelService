using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace RFNEet {
    public class RemoteData : Dictionary<string, object> {
        private static readonly string KEY_OID = "oid";
        private static readonly string KEY_PID = "pid";

        protected RemoteData(int pid, int oid) {
            Add(KEY_OID, oid);
            Add(KEY_PID, pid);
        }

        public int getOid() {
            return int.Parse(this[KEY_OID].ToString());
        }


    }
}
