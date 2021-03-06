﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityStomp;
using RFNEet.realtimeDB;

namespace RFNEet {

    public delegate void PlayerLeaved(string leavedId, string handoverId);

    public class SyncCenter : MonoBehaviour, RemoteApierHandler {
        private static SyncCenter instance = null;
        internal static readonly int OID_SIZE = 4;
        internal LocalPlayerRepo localRepo
        {
            get; private set;
        }
        internal Dictionary<string, RemotePlayerRepo> remoteRepos = new Dictionary<string, RemotePlayerRepo>();
        internal RemoteApier api { get; private set; }
        public RealTimeDB repo { get; private set; }
        private int commDataTellerNum = 3;
        private SyncHandler hanlder;
        public bool connected { get; private set; }
        private Action<ErrorBundle> errorCb;
        public QueryUtils queryUitls { get; private set; }
        public readonly List<int> _lockThreadId = new List<int>();

        public void initHandler(SyncHandler sh, Action<ErrorBundle> ecb) {
            hanlder = sh;
            errorCb = ecb;
        }

        public void init(URestApi ua, string roomId, bool localDebug = false) {
            string wsurl = PingBundle.genWsUrl(ua);
            StompClient sc = localDebug ? (StompClient)new StompClientDebug(wsurl) : (StompClient)new StompClientAll(wsurl, PidGeter.getPid());
            StompIniter si = new StompIniter(sc, roomId);
            api = new RemoteApier(si, this);
            repo = new RealTimeDB(ua, si);
            queryUitls = new QueryUtils(this);
        }

        public CommRemoteRepo getCommRemoteRepo() {
            if (!remoteRepos.ContainsKey(CommRemoteRepo.COMM_PID)) {
                CommRemoteRepo crr = new CommRemoteRepo(api, hanlder.onNewRemoteObjectCreated);
                remoteRepos.Add(crr.pid, crr);
            }
            return (CommRemoteRepo)remoteRepos[CommRemoteRepo.COMM_PID];
        }

        public SyncObject findSyncObject(string pid, string oid) {
            if (api.meId.Equals(pid)) {
                return localRepo.objectMap[oid];
            } else {
                return remoteRepos[pid].objectMap[oid];
            }
        }

        public void addOnConnectedCb(Action<CommRemoteRepo> cb) { if (connected) cb(getCommRemoteRepo()); else onConnectedCbs.Add(cb); }
        private List<Action<CommRemoteRepo>> onConnectedCbs = new List<Action<CommRemoteRepo>>();
        public void connect(Action<HandshakeDto, LocalPlayerRepo> handshakeCb) {

            api.connect((hd, list) => {
                connected = true;
                localRepo = new LocalPlayerRepo(hd.meId, api);
                localRepo.setStartAt(hd.meStartAt);
                createRemoteList(hd.meId, list);
                handshakeCb(hd, localRepo);
                CommRemoteRepo crr = getCommRemoteRepo();
                onConnectedCbs.ForEach(cb => cb(crr));
                foreach (RemotePlayerRepo rpr in remoteRepos.Values) {
                    rpr.setRemoteApier(api);
                }
            });
        }

        private void createRemoteList(string meId, List<string> ids) {
            foreach (string id in ids) {
                if (!id.Equals(meId)) {
                    addRemoteRepo(id);
                }
            }
            StartCoroutine(waitAllHandShakeReadyed());
        }

        private IEnumerator waitAllHandShakeReadyed() {
            List<string> keys = new List<string>(remoteRepos.Keys);
            while (!localObjectInjected) {
                yield return new WaitForSeconds(0.35f);
            }
            foreach (string key in keys) {
                if (!key.Equals(CommRemoteRepo.COMM_PID) && remoteRepos.ContainsKey(key)) {
                    RemotePlayerRepo rpr = remoteRepos[key];
                    float wAt = Time.time;
                    while ((!rpr.handshaked && (Time.time - wAt) < 5f)) {
                        yield return new WaitForSeconds(0.35f);
                    }
                }
            }
            hanlder.onAllRemotePlayerReadyed(localRepo);
        }

        internal RemotePlayerRepo addRemoteRepo(string sid) {
            if (remoteRepos.ContainsKey(sid)) {
                return remoteRepos[sid];
            } else {
                RemotePlayerRepo rpr = new RemotePlayerRepo(sid, api, hanlder.onNewRemoteObjectCreated);
                remoteRepos.Add(sid, rpr);
                return rpr;
            }
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
            if (remoteRepos.ContainsKey(sid)) {
                RemotePlayerRepo rpr = remoteRepos[sid];
                string firstId = handleLeavedObjects(rpr);
                rpr.destoryAll();
                remoteRepos.Remove(sid);
                onPlayerLeavedCb(sid, firstId);
            }
        }

        private string handleLeavedObjects(RemotePlayerRepo rpr) {
            List<string> l = new List<string>();
            l.Add(rpr.pid);
            string firstIds = queryUitls.getEnterFirst();
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

        /*New Player Joined*/
        public bool localObjectInjected { get; private set; }
        public void onNewPlayerJoined(RemoteBroadcastData rbd) {
            if (rbd.senderId.Equals(api.meId)) {
                Action inRoomToken = () => {
                    localObjectInjected = true;
                    BackgroubdChecker.start(this);
                };
                hanlder.onSelfInRoom(localRepo, inRoomToken);
            } else {
                if (!remoteRepos.ContainsKey(rbd.senderId)) {
                    StartCoroutine(addRemoteRepoDependsSelfInRoom(rbd));
                }
            }
        }

        private IEnumerator addRemoteRepoDependsSelfInRoom(RemoteBroadcastData rbd) {
            while (!localObjectInjected) {
                yield return new WaitForSeconds(0.5f);
                Debug.Log("wait for onSelfInRoom _localObjectSetuped=" + localObjectInjected);
            }
            Debug.Log("addRemoteRepoDependsSelfInRoom=" + rbd.senderId + " tellerids=" + rbd.tellerIds);
            RemotePlayerRepo rpr = addRemoteRepo(rbd.senderId);
        }

        public void onNewPlayerReadyed(PlayerDto pdto) {
            RemotePlayerRepo rpr = remoteRepos.ContainsKey(pdto.sessionId) ? remoteRepos[pdto.sessionId] : addRemoteRepo(pdto.sessionId);
            rpr.setStartAt(pdto.startAt);
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
            if (!string.Equals(rbd.senderId, api.meId)) {
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
                bool b = lo.postInitDtoSafe();
                Debug.Log("repairMissObject=" + b);
            }
        }

        public void close() {
            api.close();
        }

        void OnDestroy() {
            close();
        }

        public void onRemotePlayTellMyObject(InboxTellObjectData iaod) {
            localRepo.objectMap[iaod.oid].onRemoteTellSelf(iaod);
        }

        public void onConnectClosedCb(string msg) {
            hanlder.onConnectedClose(msg);
        }

        public void onServerShutdown(float cutTime) {
            hanlder.onServerShutdown(cutTime);
        }

        public static SyncCenter getInstance() {
            if (instance == null || !instance.isActiveAndEnabled) {
                instance = FindObjectOfType<SyncCenter>();
            }
            return instance;
        }

    }
}
