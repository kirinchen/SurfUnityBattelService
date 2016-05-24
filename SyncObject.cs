using UnityEngine;
using System.Collections;
namespace RFNEet {
    public class SyncObject<T> : MonoBehaviour {

        internal string pid;
        internal string oid;
        internal RemoteApier api;
        internal T handler;

        internal  void init(string pid,string oid,RemoteApier api,T h) {
            this.api = api;
            this.pid = pid;
            this.oid = oid;
            this.handler = h;
        }

        public RemoteData setup(RemoteData rd) {
            rd.oid = oid;
            rd.pid = pid;
            return rd;
        }



    }

}