using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RFNEet {
    public class RemoteData  {

        public enum SysTag {
            NONE,DELETED
        }

        public string pid;
        public string oid;
        public string tag;
        private string _source;
        //internal Cmd cmd;

        internal void setSource(string s) {
            _source = s;
        }

        internal void setSysTag(SysTag st) {
            tag = "#"+st.ToString();
        }

        internal SysTag getSysTag() {
            if (tag == null || tag.Length<=0) {
                return SysTag.NONE;
            }
            string[] ss= tag.Split('#');
            if (ss.Length == 2) {
                return (SysTag)Enum.Parse(typeof(SysTag), ss[1]);
            } else {
                return SysTag.NONE;
            }
        }

        public string getSource() {
            return _source;
        }

        public T to<T>() {
            return JsonConvert.DeserializeObject<T>(_source);
        }

    }
}
