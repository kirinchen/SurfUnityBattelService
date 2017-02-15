using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RFNEet {
    public class RemoteData {

        public enum SysCmd {
            NONE, NEW_OBJECT, DELETED

        }

        public string pid;
        public string sid; //sender id it`s only for comm object use
        public string oid;
        public string tag;
        public SysCmd cmd = SysCmd.NONE;
        private string _source;
        //internal Cmd cmd;

        internal void setSource(string s) {
            _source = s;
        }

        internal void setSysTag(SysCmd st) {
            cmd = st;
        }

        internal SysCmd getSysTag() {
            return cmd;
        }

        public string getSource() {
            return _source;
        }

        public T to<T>() where T : RemoteData {
            T ans = JsonConvert.DeserializeObject<T>(_source);
            ans.setSource(_source);
            return ans;
        }

    }
}
