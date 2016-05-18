using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace RFNEet {
    public class RemoteData : Dictionary<string, object> {
        private static readonly string KEY_OID = "oid";
        private static readonly string KEY_PID = "pid";

        internal RemoteData(string pid, string oid) {
            Add(KEY_OID, oid);
            Add(KEY_PID, pid);
        }

        public string getOid() {
            return this[KEY_OID].ToString();
        }


    }
}
