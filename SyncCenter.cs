using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace RFNEet {
    public class SyncCenter : MonoBehaviour {
        private LocalPlayerRepo localRepo;
        private Dictionary<string, RemotePlayerRepo> remoteRepos = new Dictionary<string, RemotePlayerRepo>();
        private RemoteApier api;
        

        void Awake() {

        }

        public void init(string url, string roomId) {
            api = new RemoteApier(url, roomId);
        }

        public void connect(Action<LocalPlayerRepo> handshakeCb) {
            api.connect((meid,list) => {
                localRepo = new LocalPlayerRepo(meid, api);
                createRemoteList(list);
                handshakeCb(localRepo);
            });
        }

        private void createRemoteList(List<string> ids) {
            foreach (string id in ids) {
                RemotePlayerRepo rpr = new RemotePlayerRepo(id,api);
                remoteRepos.Add(id,rpr);
            }
        }
    }
}
