using UnityEngine;
using System.Collections;
namespace RFNEet {
    public class RoomInfo<T>  {
        public string roomId;
        public string gameUid;
        public int currentCount;
        public int maxPlayerCount;
        public T data;
    }
}
