using UnityEngine;
using System.Collections;

namespace RFNEet {
    public class InboxData {

        public enum Type {
            AllData,MissObject
        }

        public string senderId;
        public Type type;
    }
}
