using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class DefaultPingScoreCalc : PingDto.ScoreCalc {
        public int getRoomScore(PingDto.RoomI i) {
            if (i.currentCount >= i.maxPlayerCount) return 0;
            return i.currentCount;
        }

        public int getServerScore( PingDto d) {
            int p = (int)(10 - d.ping);
            int c = (int)(10-(d.p/10));
            return p + c;
        }
    }
}
