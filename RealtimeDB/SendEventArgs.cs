using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.realtimeDB {
    public class SendEventArgs : DBResult {

        public NodeData snapshot;
        public object databaseError;

        public SendEventArgs() { }

        public SendEventArgs(NodeData r) {
            snapshot = r;
        }

        public IEnumerable<DBResult> children() {
            return snapshot.children.ConvertAll(c => {
                return (DBResult)new SendEventArgs(c);
            });
        }

        public string key() {
            return snapshot.key;
        }

        public string getRawJsonValue() {
            if (snapshot == null) return null;
            return JsonConvert.SerializeObject(snapshot.value);
        }

        public object GetValue() {
            return snapshot.value;
        }

        public class NodeData {

            public string key;
            public object value;
            public List<NodeData> children;
        }
    }
}
