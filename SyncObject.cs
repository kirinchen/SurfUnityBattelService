using UnityEngine;
using System.Collections;
namespace RFNEet {
    public class SyncObject : MonoBehaviour {

        internal string pid;
        internal string oid;
        internal RemoteApier api;

        internal  void init(string pid,string oid,RemoteApier api) {
            this.api = api;
            this.pid = pid;
            this.oid = oid;
        }


    }

}