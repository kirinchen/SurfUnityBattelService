using surfm.tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class PingDtoFinder : MonoBehaviour {

        public string scoreCalcClassName = "RFNEet.DefaultPingScoreCalc";
        private PingDto.ScoreCalc calc;
        private List<PingBundle> pings;

        void Awake() {
            calc = CommUtils.newInstance<PingDto.ScoreCalc>(scoreCalcClassName);
        }

        internal void setup(List<PingBundle> p) {
            pings = p;
        }

        public virtual int getRoomScore(PingBundle b, string rid) {
            PingDto pd = b.result;
            if (pd == null) return 0;
            int ss = pd.getScore(calc);
            int rs = pd.list.Find(r => { return string.Equals(r.roomId, rid); }).getScore(calc);
            return ss + rs;
        }

        public virtual RoomScore getBestRoom() {
            List<RoomScore> list = getRoomScores();
            list.Sort((a, b) => {
                return a.score.CompareTo(b.score);
            });
            if (list.Count <= 0) return null;
            return list[list.Count - 1];
        }

        public virtual List<RoomScore> getRoomScores() {
            List<RoomScore> ans = new List<RoomScore>();
            pings.ForEach(p => {
                ans.AddRange(getRoomScores(p));
            });
            return ans;
        }

        public virtual List<RoomScore> getRoomScores(PingBundle pb) {
            if (!pb.ok) return new List<RoomScore>();
            if (pb.result == null) return new List<RoomScore>();
            if (pb.result.list == null || pb.result.list.Count <= 0) return new List<RoomScore>();
            return pb.result.list.ConvertAll(r => {
                RoomScore rs = new RoomScore();
                rs.api = pb.service.api;
                rs.pingBundle = pb;
                rs.dto = pb.result;
                rs.room = r;
                rs.score = getRoomScore(pb, r.roomId);
                return rs;
            });
        }

        public virtual ServerScore getBestServer() {
            List<ServerScore> l = listServerScore();
            l.Sort((a, b) => {
                return a.score.CompareTo(b.score);
            });
            if (l.Count <= 0) return null;
            return l[l.Count - 1];
        }

        public virtual List<ServerScore> listServerScore() {
            return pings.ConvertAll(p => { return getServerScore(p); });
        }

        public virtual ServerScore getServerScore(PingBundle pb) {
            ServerScore ans = new ServerScore();
            ans.pingBundle = pb;
            ans.dto = pb.result;
            ans.api = pb.service.api;
            ans.score = pb.ok ? pb.result.getScore(calc) : 0;
            return ans;
        }

        public class ServerScore {
            public PingBundle pingBundle;
            public PingDto dto;
            public URestApi api;
            public int score;
        }

        public class RoomScore {
            public PingBundle pingBundle;
            public PingDto dto;
            public URestApi api;
            public PingDto.RoomI room;
            public int score;
        }



    }
}