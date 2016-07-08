using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RFNEet {
    public class RemoteBroadcastData : RemoteData {
        public string type;
        public string senderId;
        public string btag;
        //Just for new player joined
        public List<string> tellerIds;

    }
}
