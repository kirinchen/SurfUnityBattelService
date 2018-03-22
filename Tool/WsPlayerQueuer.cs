using surfm.tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class WsPlayerQueuer : CommRemoteObject, PlayerQueuer.DataProvider {
        public static WsPlayerQueuer instance { get; private set; }
        private PlayerQueuer ceneter;
        public static readonly string KEY_OID = "@WsPlayerQueuer";
        public static readonly string KEY_TAG = "@FPQ";
        public string meId { get; private set; }
        public Data data = new Data();
        public List<string> debugIds = new List<string>();

        void Awake() {
            specifyOid = KEY_OID;
            instance = this;
            meId = PidGeter.getPid();
            ceneter = gameObject.AddComponent<PlayerQueuer>();
            ceneter.setDataProvider(this);
        }

        public void addPlayer(string id) {
            DateTime d = NistService.getTime();
            data.intoMap.Add(id, d);
            post(genInitDto());
        }

        public override RemoteData genInitDto() {
            RemoteData ans = data;
            ans.tag = KEY_TAG;
            return ans;
        }

        public List<string> playerIds() {
            List<string> realPids = new List<string>(data.intoMap.Keys);
            realPids.Sort((a, b) => {
                return data.intoMap[a].CompareTo(data.intoMap[b]);
            });
            debugIds = realPids;
            return realPids;
        }

        public void setTokenPlayer(string v, PlayerQueuer.TokePost postD) {
            data.token = v;
            post(genInitDto());
        }

        public string tokenPlayer() {
            return data.token;
        }

        internal override void onRemoteUpdate(RemoteData s) {
            Data _d = s.to<Data>();
            data = _d;
        }

        internal override void onRemoved(RemoteData rd) {
            Debug.Log("onRemoved");
        }

        [System.Serializable]
        public class Data : RemoteData {
            public Dictionary<string, DateTime> intoMap = new Dictionary<string, DateTime>();
            public string token;
        }

    }
}
