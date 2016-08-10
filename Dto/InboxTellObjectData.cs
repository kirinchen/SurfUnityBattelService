using UnityEngine;
using System.Collections;

namespace RFNEet {
    public class InboxTellObjectData : InboxData {

        public string oid;
        public string tag;
        private string _source;

        public InboxTellObjectData() {
            type = Type.ObjectMsg;
        }

        public string getSource() {
            return _source;
        }

        internal void setSource(string s) {
            _source = s;
        }

    }

}