using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RFNEet {
    public class PingDto {

        public string timestamp;
        public List<MyRoomInfo> list = new List<MyRoomInfo>();
        public float p;

        public class MyRoomInfo : RoomInfo<object> {
            [JsonIgnore]
            internal string wsUrl = "NONE";
            //    public string roomId = "NONE";
            //    public string gameUid = "NONE";
            //    public int currentCount;
            //    public int maxPlayerCount;

            public RoomInfo<T> to<T>() {
                string p = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<RoomInfo<T>>(p);
            }
        }

    }
}




