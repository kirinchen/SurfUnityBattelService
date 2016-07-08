using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RFNEet {
    public class SysInboxDto  {
        public enum Type {
            SurplusPlayerList, General, LostPlayerList
        }

        public Type type;
        public bool system;
        public List<string> surplusList;
        public string lostPlayerId;
    }
}
