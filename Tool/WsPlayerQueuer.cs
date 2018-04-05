using surfm.tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class WsPlayerQueuer : CommRemoteObject, PlayerQueuer.DataProvider {
        //public static WsPlayerQueuer instance { get; private set; }
        private PlayerQueuer ceneter;
        public static readonly string KEY_OID = "@WsPlayerQueuer";
        public static readonly string KEY_TAG = "@FPQ";
        public string meId { get { return SyncCenter.getInstance().api.meId; } }
        public Data data = new Data();
        public List<string> debugIds = new List<string>();

        void Awake() {
            specifyOid = KEY_OID;
            //instance = this;
            ceneter = gameObject.AddComponent<PlayerQueuer>();
            ceneter.setDataProvider(this);
        }



        public void addPlayer(string id) {
            data.token = SyncCenter.getInstance().queryUitls.getEnterFirst();
            post(genInitDto());
        }

        public override RemoteData genInitDto() {
            RemoteData ans = data;
            ans.tag = KEY_TAG;
            return ans;
        }

        public List<string> playerIds() {
            SyncCenter sc = SyncCenter.getInstance();

            List<string> realPids = new List<string>(sc.remoteRepos.Keys);
            realPids.RemoveAll(s => { return CommRemoteRepo.COMM_PID.Equals(s); });
            realPids.Add(sc.localRepo.pid);


            realPids.Sort((a, b) => {
                float aAt = sc.queryUitls.getStartAt(a);
                float bAt = sc.queryUitls.getStartAt(b);
                return aAt.CompareTo(bAt);
            });
            debugIds = realPids;
            return realPids;
        }

        public void setTokenPlayer(string v, PlayerQueuer.TokePost postD) {
            data.token = v;
            if (postD == PlayerQueuer.TokePost.POST) {
                post(genInitDto());
            }
        }

        public string tokenPlayer() {
            return data.token;
        }

        internal override void onRemoteUpdate(RemoteData s) {
            Data _d = s.to<Data>();
            Debug.Log(s.getSource());
            if (!string.Equals(_d.token, data.token)) {
                ceneter.setTokenChange(_d.token, PlayerQueuer.TokePost.NONE);
            }
            data = _d;
        }

        internal override void onRemoved(RemoteData rd) {
            Debug.Log("onRemoved");
        }

        public string getMeId() {
            return SyncCenter.getInstance().api.meId;
        }

        [System.Serializable]
        public class Data : RemoteData {
            public string token;
        }

    }
}
