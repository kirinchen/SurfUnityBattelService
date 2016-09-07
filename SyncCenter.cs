﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace RFNEet {

    public delegate void PlayerLeaved(string leavedId, string handoverId);

    public class SyncCenter : MonoBehaviour, RemoteApierHandler {
        internal static readonly int OID_SIZE = 4;
        internal LocalPlayerRepo localRepo {
            get; private set;
        }
        internal Dictionary<string, RemotePlayerRepo> remoteRepos = new Dictionary<string, RemotePlayerRepo>();
        internal RemoteApier api {
            get; private set;
        }
        private int commDataTellerNum = 3;
        private SyncHandler hanlder;
        private bool connected = false;
        private Action<ErrorBundle> errorCb;
        public QueryUtils queryUitls {
            get; private set;
        }

        public void init(string url, string roomId, SyncHandler sh, bool localDebug = false) {
            hanlder = sh;
            api = new RemoteApier(url, roomId, this, localDebug);
            queryUitls = new QueryUtils(this);
        }

        public Dictionary<string, RemotePlayerRepo> getRemoteRepos() {
            return new Dictionary<string, RemotePlayerRepo>(remoteRepos);
        }

        private void initCommRepo() {
            CommRemoteRepo crr = new CommRemoteRepo(api, hanlder.onNewRemoteObjectCreated);
            remoteRepos.Add(crr.pid, crr);
        }

        public bool isConnected() {
            return connected;
        }

        public CommRemoteRepo getCommRemoteRepo() {
            return (CommRemoteRepo)remoteRepos[CommRemoteRepo.COMM_PID];
        }

        public void setErrorCb(Action<ErrorBundle> ecb) {
            errorCb = ecb;
        }

        public SyncObject findSyncObject(string pid, string oid) {
            if (api.meId.Equals(pid)) {
                return localRepo.objectMap[oid];
            } else {
                return remoteRepos[pid].objectMap[oid];
            }
        }

        public void connect(Action<LocalPlayerRepo> handshakeCb) {
            api.connect((meid, list) => {
                connected = true;
                initCommRepo();
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
            StartCoroutine(waitSomeRemoteHandShakeThanSend());
        }

        private IEnumerator waitSomeRemoteHandShakeThanSend() {
            List<string> keys = new List<string>(remoteRepos.Keys);
            foreach (string key in keys) {
                if (!key.Equals(CommRemoteRepo.COMM_PID)) {
                    RemotePlayerRepo rpr = remoteRepos[key];
                    float wAt = Time.time;
                    while (!rpr.handshaked && (Time.time - wAt) < 5f) {
                        yield return new WaitForSeconds(0.35f);
                    }
                }
            }
            hanlder.onAllRemotePlayerReadyed(localRepo);
        }

        private RemotePlayerRepo addRemoteRepo(string sid) {
            RemotePlayerRepo rpr = new RemotePlayerRepo(sid, api, hanlder.onNewRemoteObjectCreated);
            remoteRepos.Add(sid, rpr);
            return rpr;
        }

        /*some player leaved*/
        public void onPlayerLeaved(string sid, string handoverId) {
            removeRemotePlayerRepo(sid);
        }

        public void onPlayerLeavedByIndex(string sidIndex) {
            removeRemotePlayerRepo(sidIndex);
        }

        public PlayerLeaved onPlayerLeavedCb = (lid, hid) => { };
        private void removeRemotePlayerRepo(string sid) {
            RemotePlayerRepo rpr = remoteRepos[sid];
            string firstId = handleLeavedObjects(rpr);
            rpr.destoryAll();
            transferCreatorForCommObjects(sid, firstId);
            remoteRepos.Remove(sid);
            onPlayerLeavedCb(sid, firstId);
        }

        private string handleLeavedObjects(RemotePlayerRepo rpr) {
            List<string> l = new List<string>();
            l.Add(rpr.pid);
            string firstIds = queryUitls.sortPids(l)[0];
            if (firstIds.Equals(api.meId)) {
                localRepo.addAll(rpr, hanlder.handoverToMe);
            } else {
                RemotePlayerRepo orpr = remoteRepos[firstIds];
                orpr.addAll(rpr, hanlder.handoverToOther);
            }
            return firstIds;
        }

        /*void Update() {
            if (Input.GetKeyDown(KeyCode.L)) {
                List<string> ids = new List<string>();
                foreach (string sid in remoteRepos.Keys) {
                    if (!CommRemoteRepo.COMM_PID.Equals(sid)) {
                        RemotePlayerRepo rpr = remoteRepos[sid];
                        rpr.destoryAll(false);
                        ids.Add(sid);
                    }
                }
                foreach (string id in ids) {
                    remoteRepos.Remove(id);
                }

            }
        }*/



        private void transferCreatorForCommObjects(string sid, string handoverId) {
            Dictionary<string, RemoteObject> m = getCommRemoteRepo().objectMap;
            List<string> keys = new List<string>(m.Keys);
            foreach (string k in keys) {
                CommRemoteObject co = (CommRemoteObject)m[k];
                if (co.creator.Equals(sid)) {
                    co.setCreator(handoverId);
                }
            }
        }

        /*New Player Joined*/
        private bool localObjectInjected = false;
        public void onNewPlayerJoined(RemoteBroadcastData rbd) {
            if (rbd.senderId.Equals(api.meId)) {
                Action inRoomToken = () => {
                    localObjectInjected = true;
                    InvokeRepeating("routineCheckPlayerList", 11, 11);
                    InvokeRepeating("routineCheckServerTime", 17, 17);
                };
                hanlder.onSelfInRoom(localRepo, inRoomToken);
            } else {
                if (!remoteRepos.ContainsKey(rbd.senderId)) {
                    StartCoroutine(addRemoteRepoDependsSelfInRoom(rbd));
                }
            }
        }

        //TODO
        void routineCheckServerTime() {
            api.syncTime();
        }

        void routineCheckPlayerList() {
            List<string> l = new List<string>(remoteRepos.Keys);
            l.Remove(CommRemoteRepo.COMM_PID);
            api.checkPlayerList(l);
        }

        private IEnumerator addRemoteRepoDependsSelfInRoom(RemoteBroadcastData rbd) {
            while (!localObjectInjected) {
                yield return new WaitForSeconds(0.5f);
                Debug.Log("wait for onSelfInRoom _localObjectSetuped=" + localObjectInjected);
            }
            Debug.Log("addRemoteRepoDependsSelfInRoom=" + rbd.senderId + " tellerids=" + rbd.tellerIds);
            RemotePlayerRepo rpr = addRemoteRepo(rbd.senderId);
        }

        public void onNewPlayerReadyed(string sid) {
            RemotePlayerRepo rpr = remoteRepos.ContainsKey(sid) ? remoteRepos[sid] : addRemoteRepo(sid);
            bool hasCommData = queryUitls.isCommDataTeller(commDataTellerNum);
            tellNewPlayerMyInfo(rpr, hasCommData);
        }

        private void tellNewPlayerMyInfo(RemotePlayerRepo rpr, bool hasCommData) {
            AllSyncData co = hanlder.getCurrentInfoFunc(localRepo, hasCommData);
            co.type = InboxData.Type.AllData;
            rpr.sendToInbox(co);
        }

        public void onRepairLostPlayer(string lostPis) {
            RemotePlayerRepo rpr = remoteRepos.ContainsKey(lostPis) ? remoteRepos[lostPis] : addRemoteRepo(lostPis);
            tellNewPlayerMyInfo(rpr, false);
        }



        /* old player tell self it`s information */
        public void onRemoteFirstSync(string sid, AllSyncDataResp asdr) {
            try {
                RemotePlayerRepo rpr = remoteRepos.ContainsKey(sid) ? remoteRepos[sid] : addRemoteRepo(sid);
                CommRemoteRepo crr = getCommRemoteRepo();
                AllSyncData asd = asdr.toAllSyncData();
                foreach (RemoteData rd in asd.objectList) {
                    rpr.createNewObject(rd);
                }
                foreach (RemoteData rd in asd.commList) {
                    crr.createNewObject(rd);
                }
                hanlder.onRemoteFirstSync(rpr, asd);
                rpr.handshake();
            } catch (Exception e) {
                Debug.Log(e);
            }
        }

        /*server broadcast*/
        public void onBroadcast(RemoteBroadcastData rbd) {
            if (rbd.senderId != api.meId) {
                updateObjectByBroadcast(rbd);
            }
        }

        private void updateObjectByBroadcast(RemoteBroadcastData rbd) {
            if (rbd.pid == api.meId && localRepo.objectMap.ContainsKey(rbd.oid)) {
                localRepo.objectMap[rbd.oid].updateByBroadcast(rbd);
            } else if (remoteRepos.ContainsKey(rbd.pid)) {
                RemotePlayerRepo rpr = remoteRepos[rbd.pid];
                if (rpr.objectMap.ContainsKey(rbd.oid)) {
                    rpr.objectMap[rbd.oid].updateByBroadcast(rbd);
                }
            }

        }

        public void onErrorCb(ErrorBundle error) {
            errorCb(error);
        }

        public void repairMissObject(string missWho, string moid) {
            Dictionary<string, LocalObject> map = localRepo.getMap();
            if (map.ContainsKey(moid)) {
                LocalObject lo = map[moid];
                lo.postInitDto();
            }
        }

        public void close() {
            api.close();
        }

        void OnDestroy() {
            api.close();
        }

        public void onRemotePlayTellMyObject(InboxTellObjectData iaod) {
            localRepo.objectMap[iaod.oid].onRemoteTellSelf(iaod);
        }

        public void onConnectClosedCb(string msg) {
            hanlder.onConnectedClose(msg);
        }
    }
}
