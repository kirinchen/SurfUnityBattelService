using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace RFNEet {
    public class SyncCenter : MonoBehaviour {
        private LocalPlayerRepo localRepo;
        private Dictionary<string, RemotePlayerRepo> remoteRepos = new Dictionary<string, RemotePlayerRepo>();
        private RemoteApier api;
        private SyncHandler hanlder;

        public void init(string url, string roomId, SyncHandler sh) {
            hanlder = sh;
            api = new RemoteApier(url, roomId);
            api.onNewPlayerJoined = onNewPlayerJoined;
            api.onRemoteFirstSync = onRemoteFirstSync;
        }

        public void connect(Action<LocalPlayerRepo> handshakeCb) {
            api.connect((meid, list) => {
                localRepo = new LocalPlayerRepo(meid, api);
                createRemoteList(meid, list);
                handshakeCb(localRepo);
            });
        }

        private void createRemoteList(string meId, List<string> ids) {
            foreach (string id in ids) {
                if (!id.Equals(meId)) {
                    addRemoteRepo(id);
                }
            }
        }

        private IEnumerator sendCreatSelfObjectsData() {

        }

        private RemotePlayerRepo addRemoteRepo(string sid) {
            RemotePlayerRepo rpr = new RemotePlayerRepo(sid, api);
            remoteRepos.Add(sid, rpr);
            return rpr;
        }

        private void onNewPlayerJoined(string sid) {
            if (sid.Equals(api.meId)) {
                hanlder.onSelfInRoomAction(localRepo);
            } else {
                RemotePlayerRepo rpr = addRemoteRepo(sid);
                tellNewPlayerMyInfo(rpr);
            }
        }

        private void tellNewPlayerMyInfo(RemotePlayerRepo rpr) {
            object co = hanlder.getCurrentInfoFunc();
            Debug.Log("tellNewPlayerMyInfo "+co);
            rpr.sendToInbox(co);
        }

        private void onRemoteFirstSync(string sid, AllSyncDataResp asdr) {
            RemotePlayerRepo rpr = remoteRepos[sid];
            hanlder.onRemoteFirstSync(rpr, asdr.toAllSyncData());
        }
    }
}
