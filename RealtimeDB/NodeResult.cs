using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.realtimeDB {
    public class NodeResult : DBResult {

        public string key;
        public object value;
        public List<NodeResult> children;

        public IEnumerable<DBResult> lisChildren() {
            return children;
        }

        public string getKey() {
            return key;
        }

        public string getRawJsonValue() {
            return JsonConvert.SerializeObject(value);
        }

        public object GetValue() {
            return value;
        }
    }
}
