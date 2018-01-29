using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RFNEet {
    public class PingDto {

        public string timestamp;
        public List<RoomI> list = new List<RoomI>();
        public float p;

        [JsonIgnore]
        internal float ping;
        [JsonIgnore]
        internal bool failed;

        public int getScore(ScoreCalc c) {
            return c.getServerScore( this);
        }

        public class RoomI : RoomInfo<object> {


            public RoomInfo<T> to<T>() {
                string p = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<RoomInfo<T>>(p);
            }

            public T convert<T>() {
                string p = JsonConvert.SerializeObject(this);
                return JsonConvert.DeserializeObject<T>(p);
            }

            public int getScore(ScoreCalc c) {
                return c.getRoomScore(this);
            }

        }


        public interface ScoreCalc {

            int getServerScore( PingDto d);

            int getRoomScore(RoomI i);

        }

    }
}




