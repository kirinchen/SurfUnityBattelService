using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RFNEet {
    [System.Serializable]
    public class RemoteData : ValueCheck {


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

        public RemoteData to(Type dataType) {
            RemoteData ans =(RemoteData) JsonConvert.DeserializeObject(_source, dataType);
            ans.setSource(_source);
            return ans;
        }

        public FieldInfo[] listFields() {
            Type myType = GetType();
            return myType.GetFields();
        }



        public static bool isValueSame(ValueCheck m, ValueCheck o) {
            if (m == o) return true;
            if ((m == null) != (o == null)) return false;
            if (!m.GetType().Equals(o.GetType())) return false;
            FieldInfo[] fis = m.GetType().GetFields();
            foreach (FieldInfo fi in fis) {
                if ("_source".Equals(fi.Name)) continue;
                if ("cmd".Equals(fi.Name)) continue;
                if ("tag".Equals(fi.Name)) continue;
                object myO = fi.GetValue(m);
                object oO = fi.GetValue(o);
                if (myO == oO) continue;
                if ((myO == null) != (oO == null)) return false;
                if (myO is IList) {
                    if (!eqaulList((IList)myO, (IList)oO)) return false;
                } else if (myO is IDictionary) {
                    if (!eqaulMap((IDictionary)myO, (IDictionary)oO)) return false;
                } else {
                    if (!_isValueSame(myO, oO)) {
                        return false;
                    }
                }

            }
            return true;
        }



        private static bool _isValueSame(object a, object b) {
            if (a == b) return true;
            if ((a == null) != (b == null)) return false;
            if (!a.GetType().Equals(b.GetType())) return false;
            if (a is ValueCheck) {
                return isValueSame((ValueCheck)a, (ValueCheck)b);
            } else {
                return object.Equals(a, b);
            }

        }

        private static bool eqaulMap(IDictionary a, IDictionary b) {
            if (a == b) return true;
            if ((a == null) != (b == null)) return false;
            if (a.Count != b.Count) return false;
            foreach (object k in a.Keys) {
                if (!b.Contains(k)) return false;
                object ao = a[k];
                object bo = b[k];
                if (!_isValueSame(ao, bo)) return false;
            }
            return true;
        }

        private static bool eqaulList(IList a, IList b) {
            if (a == b) return true;
            if ((a == null) != (b == null)) return false;
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++) {
                if (!_isValueSame(a[i], b[i])) return false;
            }
            return true;
        }

    }
}
