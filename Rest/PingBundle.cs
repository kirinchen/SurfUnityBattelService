using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System;
namespace RFNEet {
    public class PingBundle {
        internal float endTime;
        internal bool ok;
        internal bool done;
        internal string serverName;
        internal URestApi ua { get; private set; }
        internal PingDto result;
        internal float score;
        internal PingDto.MyRoomInfo bestRoom = null;
        internal Action doneAction = () => { };
        private bool calcRoomScore = true;
        private string gameKindUid;

        public void reset() {
            ok = false;
            done = false;
            serverName = string.Empty;
            result = null;
            bestRoom = null;
        }

        public PingBundle(string gameKindUid, URestApi ua, bool calcRoomScore = true) {
            this.gameKindUid = gameKindUid;
            this.calcRoomScore = calcRoomScore;
            this.ua = ua;
            serverName = ua.name;
        }

        public void pingOne() {
            string url = "/api/v1/rf/ping/" + gameKindUid + "?timestamp=" + Time.time;
            Debug.Log(ua.host + " GO");
            ua.postJson(url, null,
             (s) => {
                 correct(Time.time, s);
             },
            (m, s, r, e) => {
                Debug.Log(ua.host + " No");
                error(m);
            }
            );
        }

        internal void correct(float time, string s) {
            ok = true;
            endTime = time;
            result = JsonConvert.DeserializeObject<PingDto>(s);
            foreach (PingDto.MyRoomInfo r in result.list) {
                r.wsUrl = "ws://" + ua.host + ":" + ua.port + "/rfws";
            }
            setDone();
            float ping = (endTime - float.Parse(result.timestamp));
            Debug.Log("p=" + ping + " " + ua.name + "/" + ua.host + ":" + ua.port + " score=" + score + " OK  s=" + s);
        }

        public string genWsUrl() {
            return "ws://" + ua.host + ":" + ua.port + "/rfws";
        }

        public static string genWsUrl(URestApi ua) {
            return "ws://" + ua.host + ":" + ua.port + "/rfws";
        }

        public void calcScore() {
            if (ok) {
                float st = (endTime - float.Parse(result.timestamp)) * 1000;
                score = st;
                if (calcRoomScore) {
                    score += Mathf.Pow(result.list.Count, 2);
                    score += getBestRoomScore();
                }
                score *= 1 + result.p;
            } else {
                score = 9999999;
            }
        }

        private float getBestRoomScore() {
            float s = 999999;
            foreach (PingDto.MyRoomInfo r in result.list) {
                float _s = getRoomScore(r);
                if (_s < s) {
                    s = _s;
                    bestRoom = r;
                }
            }
            return s;
        }

        private float getRoomScore(PingDto.MyRoomInfo r) {
            int alertMax = (int)(r.maxPlayerCount * 0.8f);
            if (r.currentCount > alertMax) {
                return r.currentCount * 150;
            } else {
                int half = r.maxPlayerCount / 2;
                return Mathf.Abs(r.currentCount - half);
            }
        }

        internal void error(string s) {
            ok = false;
            setDone();
        }

        private void setDone() {
            calcScore();
            done = true;
            doneAction();
        }
    }
}
