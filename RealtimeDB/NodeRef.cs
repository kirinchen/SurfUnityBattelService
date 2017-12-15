﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace RFNEet.realtimeDB {
    public class NodeRef : DBRefenece {

        private RealTimeDB db;
        //private List<Action<DBResult>> valueListeners;
        //private List<Action<DBResult>> childAddListeners;
        //private List<Action<DBResult>> childRemoveListeners;
        private Dictionary<ListenType, List<Action<DBResult>>> listenMap = new Dictionary<ListenType, List<Action<DBResult>>>();
        public string path { get; private set; }
        public NodeRef(RealTimeDB d, string k) {
            path = k;
            db = d;
        }

        public void addValueChanged(Action<DBResult> a) {
            addListener(ListenType.ValueChange, a);
        }

        public void removeValueChanged(Action<DBResult> a) {
            removeListener(ListenType.ValueChange, a);
        }

        public void addChildAdded(Action<DBResult> a) {
            addListener(ListenType.ChildAdded, a);
        }


        public void removeChildAdded(Action<DBResult> a) {
            removeListener(ListenType.ChildAdded, a);
        }

        public void addChildRemoved(Action<DBResult> a) {
            addListener(ListenType.ChildRemoved, a);
        }


        public void removeChildRemoved(Action<DBResult> a) {
            removeListener(ListenType.ChildRemoved, a);
        }

        public DBRefenece Child(string pid) {
            return db.getNode(path + "/" + pid);
        }

        public void fetchValue(Action<DBResult> a) {
            db.rest.fetchNode(path, a);
        }

        public DBRefenece parent() {
            List<string> ss = RealTimeDB.getPathDirs(path);
            if (ss.Count == 1) {
                return null;
            }
            ss.RemoveAt(ss.Count - 1);
            return db.getNode(RealTimeDB.getPathByDirs(ss));
        }



        public void removeMe() {
            throw new NotImplementedException();
        }



        public Task SetRawJsonValueAsync(string s) {
            throw new NotImplementedException();
        }

        public Task SetValueAsync(object value) {
            var t = Task<object>.Run(() => {
                return db.rest.setValue(path,value);
            });
            return t;
        }

        private void addListener(ListenType t, Action<DBResult> a) {
            if (listenMap.ContainsKey(t)) {
                listenMap[t].Add(a);
            } else {
                listenMap.Add(t, db.listen(ListenType.ValueChange, path, a));
            }
        }

        private void removeListener(ListenType t, Action<DBResult> a) {
            if (listenMap.ContainsKey(t)) {
                listenMap[t].Remove(a);
                if (listenMap[t].Count == 0) {
                    listenMap.Remove(t);
                    db.unlisten(t, path);
                }
            }
        }
    }
}
