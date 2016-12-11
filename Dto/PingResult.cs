using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RFNEet {
    public class PingResult {

        public string timestamp;
        public List<RoomInfo> list = new List<RoomInfo>();
        public float p;

        public class RoomInfo {
            internal string wsUrl = "NONE";
            public string roomId = "NONE";
            public string gameUid = "NONE";
            public int currentCount;
            public int maxPlayerCount;
        }

    }
}




