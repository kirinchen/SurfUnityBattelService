using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace RFNEet {
    public class RemoteData  {

        public enum Cmd {
            NONE,deleted
        }

        public string pid;
        public string oid;
        public string tag;
        private string _source;
        //internal Cmd cmd;

        internal void setSource(string s) {
            _source = s;
        }

        public string getSource() {
            return _source;
        }

        public T to<T>() {
            return JsonConvert.DeserializeObject<T>(_source);
        }

    }
}
