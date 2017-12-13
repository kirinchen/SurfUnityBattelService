using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.realtimeDB {
    public interface DBResult {

        IEnumerable<DBResult> lisChildren();
        string getKey();
        string getRawJsonValue();
        object GetValue();
    }
}
