using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

namespace RFNEet {
    public class RoomInfo<T>  {
        public string roomId;
        public string gameUid;
        public int currentCount;
        public int maxPlayerCount;
        public string ownerToken;
        public T data;

        [JsonIgnore]
        public float ping;
        [JsonIgnore]
        public float cpu;

    }
}
