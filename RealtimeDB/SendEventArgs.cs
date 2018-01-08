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
            return snapshot.listChildren().ConvertAll(c => {
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

            public List<NodeData> listChildren() {
                if (value is Dictionary<string, object>) {
                    return listChildren((Dictionary<string, object>)value);
                } else if (value is List<object>) {
                    return listChildren((List<object>)value);
                }
                return new List<NodeData>();
            }

            private static List<NodeData> listChildren(List<object> value) {
                List<NodeData> ans = new List<NodeData>();
                for (int i = 0; i < value.Count; i++) {
                    NodeData nd = new NodeData();
                    nd.key = i + "";
                    nd.value = value[i];
                    ans.Add(nd);
                }
                return ans;
            }

            private static List<NodeData> listChildren(Dictionary<string, object> value) {
                List<NodeData> ans = new List<NodeData>();
                foreach (string k in value.Keys) {
                    NodeData nd = new NodeData();
                    nd.key = k;
                    nd.value = value[k];
                    ans.Add(nd);
                }
                return ans;
            }
        }
    }
}
