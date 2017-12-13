using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace RFNEet.realtimeDB {
    public interface DBRefenece {

        //Action<DBResult> childAdded { get; set; }
        //Action<DBResult> childRemoved { get; set; }
        //Action<DBResult> ValueChanged { get; set; }

        void addChildAdded(Action<DBResult> a);
        void removeChildAdded(Action<DBResult> a);

        void addChildRemoved(Action<DBResult> a);
        void removeChildRemoved(Action<DBResult> a);

        void addValueChanged(Action<DBResult> a);
        void removeValueChanged(Action<DBResult> a);


        void fetchValue(Action<DBResult> a);

        DBRefenece parent();
        DBRefenece Child(string pid);
        Task SetValueAsync(object value);
        Task SetRawJsonValueAsync(string s);
        void removeMe();
    }
}
