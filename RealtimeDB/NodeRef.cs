using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace RFNEet.realtimeDB {
    public class NodeRef : DBRefenece {

        private RealTimeDB db;
        private List<Action<DBResult>> valueListeners;
        public string path { get; private set; }
        public NodeRef(RealTimeDB d,string k) {
            path = k;
            db = d;
        }

        public void addValueChanged(Action<DBResult> a) {
            if (valueListeners == null) {
                valueListeners= db.listen(ListenType.ValueChange, path);
            }
            valueListeners.Add(a);
        }

        public void removeValueChanged(Action<DBResult> a) {
            throw new NotImplementedException();
        }

        public void addChildAdded(Action<DBResult> a) {
            throw new NotImplementedException();
        }

        public void addChildRemoved(Action<DBResult> a) {
            throw new NotImplementedException();
        }



        public void removeChildAdded(Action<DBResult> a) {
            throw new NotImplementedException();
        }

        public void removeChildRemoved(Action<DBResult> a) {
            throw new NotImplementedException();
        }

        public DBRefenece Child(string pid) {
            throw new NotImplementedException();
        }

        public void fetchValue(Action<DBResult> a) {
            throw new NotImplementedException();
        }

        public DBRefenece parent() {
            throw new NotImplementedException();
        }



        public void removeMe() {
            throw new NotImplementedException();
        }



        public Task SetRawJsonValueAsync(string s) {
            throw new NotImplementedException();
        }

        public Task SetValueAsync(object value) {
            throw new NotImplementedException();
        }
    }
}
