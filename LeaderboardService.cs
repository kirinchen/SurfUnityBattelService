using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RFNEet {
    public class LeaderboardService : MonoBehaviour {
        private URestApi rest;

        void Awake() {
            rest = GetComponent<URestApi>();
        }

        public void register(DeviceDto dto, Action cb, URestApi.OnError one) {
            rest.postJson("/play/register", dto, (s) => {
                cb();
            }, one);
        }

        public void isRegistered(string id, Action<bool> cb, URestApi.OnError one) {
            rest.get("/play/registered/"+ id, (s) => {
                cb(bool.Parse(s));
            }, one);
        }

        public void addRecord(RecordDto rd, Action<long> cb, URestApi.OnError one) {
            rest.postJson("/play/record", rd, (s) => {
                cb(long.Parse(s));
            }, one);
        }

        public void listSortScore(string leaderkind, int startIndex, int pageSize, Action<List<RankDto>> cb, URestApi.OnError one) {
            string url = string.Format("/query/{0}/records?startIndex={1}&pageSize={2}", leaderkind, startIndex, pageSize);
            rest.get(url, (s) => {
                List<RankDto> ans = JsonConvert.DeserializeObject<List<RankDto>>(s);
                cb(ans);
            }, one);
        }

        public void getScoreLank(string leaderkind, double score, Action<double> cb, URestApi.OnError one) {
            string url = string.Format("/query/{0}/records/rank?score={1}", leaderkind, score);
            rest.get(url, (s) => {
                cb(long.Parse(s));
            }, one);

        }

        public void listCountOfKind(List<KindInfoQueryDto> dto, Action<List<KindResultQueryDto>> cb, URestApi.OnError one) {
            string url = "/query/leaderkind/count";
            rest.postJson(url, dto, s => {
                List<KindResultQueryDto> ans = JsonConvert.DeserializeObject<List<KindResultQueryDto>>(s);
                cb(ans);
            }, one);
        }

        public class KindInfoQueryDto {
            public string leaderkindId;
            public double score;
        }

        public class KindResultQueryDto {
            public string leaderkindId;
            public long rank;
            public long count;
        }

        public class DeviceDto {
            public string deviceId;
            public string type;
            public string model;
            public string name;
        }

        public class RankDto {
            public string deviceUid;
            public double score;
            public string name;
        }

        public class RecordDto {
            public string deviceUid;
            public string leaderKindUid;
            public double score;
            public LeaderKindDto createData;
        }

        public class LeaderKindDto {

            public string uid;
            public string name;
            public string game;
            public bool scoreAsc;
        }

    }
}
