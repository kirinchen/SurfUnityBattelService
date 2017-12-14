using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.realtimeDB {
    public interface DBResult {

        IEnumerable<DBResult> children();
        string key();
        string getRawJsonValue();
        object GetValue();
    }
}
