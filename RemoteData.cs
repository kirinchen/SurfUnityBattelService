using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace RFNEet {
    public class RemoteData  {

        public string pid;
        public string oid;
        public string tag;
        internal string _source;

        internal void setSource(string s) {
            _source = s;
        }

        public T to<T>() {
            Debug.Log(_source);
            return JsonConvert.DeserializeObject<T>(_source);
        }

    }
}
