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
            api.onPlayerLeaved = onPlayerLeaved;
        }

        public void setErrorCb(Action<ErrorBundle> ecb) {
            api.onErrorCb = ecb;
        }

        public void connect(Action<LocalPlayerRepo> handshakeCb) {
            api.connect((meid, list) => {
                localRepo = new LocalPlayerRepo(meid, api);
                createRemoteList(meid, list);
                handshakeCb(localRepo);
            });
        }

        private void createRemoteList(string meId, List<string> ids) {
            bool hasRemoteRepo = false;
            foreach (string id in ids) {
                if (!id.Equals(meId)) {
                    addRemoteRepo(id);
                    hasRemoteRepo = true;
                }
            }
            if (hasRemoteRepo) {
                StartCoroutine(waitSomeRemoteHandShakeThanSend());
            }
        }

        private IEnumerator waitSomeRemoteHandShakeThanSend() {
            foreach (RemotePlayerRepo rpr in remoteRepos.Values) {
                int waitTimes = 0;
                while (!rpr.handshaked && waitTimes < 5) {
                    yield return new WaitForSeconds(0.35f);
                    waitTimes++;
                }
            }
            hanlder.onAllRemotePlayerReadyed(localRepo);
        }

        private RemotePlayerRepo addRemoteRepo(string sid) {
            RemotePlayerRepo rpr = new RemotePlayerRepo(sid, api, hanlder.onNewRemoteObjectCreated);
            remoteRepos.Add(sid, rpr);
            return rpr;
        }

        private void onPlayerLeaved(string sid) {
            RemotePlayerRepo rpr = remoteRepos[sid];
            rpr.destoryAll();
            remoteRepos.Remove(sid);
        }

        private bool _localObjectSetuped = false;
        private void onNewPlayerJoined(string sid) {
            if (sid.Equals(api.meId)) {
                Loom.QueueOnMainThread(() => {
                    Action inRoomToken = () => {
                        _localObjectSetuped = true;
                    };
                    hanlder.onSelfInRoom(localRepo, inRoomToken);
                });
            } else {
                StartCoroutine(addRemoteRepoDependsSelfInRoom(sid));
            }
        }

        private IEnumerator addRemoteRepoDependsSelfInRoom(string sid) {
            while (!_localObjectSetuped) {
                yield return new WaitForSeconds(0.5f);
                Debug.Log("wait for onSelfInRoom _localObjectSetuped="+ _localObjectSetuped);
            }
            RemotePlayerRepo rpr = addRemoteRepo(sid);
            tellNewPlayerMyInfo(rpr);
        }

        private void tellNewPlayerMyInfo(RemotePlayerRepo rpr) {
            object co = hanlder.getCurrentInfoFunc(localRepo);
            Debug.Log("tellNewPlayerMyInfo " + co);
            rpr.sendToInbox(co);
        }

        private void onRemoteFirstSync(string sid, AllSyncDataResp asdr) {
            RemotePlayerRepo rpr = remoteRepos[sid];
            AllSyncData asd = asdr.toAllSyncData();
            foreach (RemoteData rd in asd.objectList) {
                rpr.createNewObject(rd);
            }
            hanlder.onRemoteFirstSync(rpr, asd);
            rpr.handshake();
        }
    }
}
