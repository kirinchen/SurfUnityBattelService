using UnityEngine;
using System.Collections;

namespace RFNEet {
    public class InboxMissData : InboxData {
        public string moid;
        public string missWho;

        public InboxMissData() {
        }

        public InboxMissData(string missOid,string missWhoPid) {
            moid = missOid;
            missWho = missWhoPid;
            type = Type.MissObject;
        }
    }
}
