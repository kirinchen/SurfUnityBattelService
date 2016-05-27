using UnityEngine;
using System.Collections;
namespace RFNEet {
    public class SyncObject : MonoBehaviour {

        internal string pid;
        internal string oid;
        internal RemoteApier api;
        internal SyncObjectListener listener;

        internal  void init(string pid,string oid,RemoteApier api, SyncObjectListener listener) {
            this.api = api;
            this.pid = pid;
            this.oid = oid;
            this.listener = listener;
        }

        public RemoteData setup(RemoteData rd) {
            rd.oid = oid;
            rd.pid = pid;
            return rd;
        }

        public virtual void removeMe() {
           // listener.onRemoveMe(this);
        }


    }

    public interface SyncObjectListener {
         void onRemoveMe(SyncObject so);
    }

}