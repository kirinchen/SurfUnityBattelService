using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using surfm.tool;

namespace RFNEet {
    public abstract class AbsSyncHandler : MonoBehaviour, SyncHandler {
        public enum Status {
            Idle, Connecting
        }

        public Status state { get; private set; }
        internal SyncCenter sc
        {
            get; private set;
        }
        public delegate void OnPingError(string wsUrl, string roomId);
        internal OnPingError onPingError = (w, r) => { };
        private RemoteObjectCreaterAbs remoteObjectCreater;

        public void Awake() {
            state = Status.Idle;
            sc = GetComponent<SyncCenter>();
            remoteObjectCreater = GetComponent<RemoteObjectCreaterAbs>();
            sc.initHandler(this, onError);
        }

        public void connect(bool ok, URestApi api, string roomId, bool localDebug = false) {
            state = Status.Connecting;
            try {
                if (ok) {
                    Debug.Log(PingBundle.genWsUrl(api) + " connected ");
                    sc.init(api, roomId, localDebug);
                    sc.connect(onConnected);
                } else {
                    onPingError(PingBundle.genWsUrl(api), roomId);
                }
            } catch (Exception e) {
                onPingError(PingBundle.genWsUrl(api), roomId);
            }
        }

        private CallbackList onConnectCbs = new CallbackList();
        public void addOnConnectedCb(Action a) {
            onConnectCbs.add(a);
        }
        internal HandshakeDto handshakeDto { get; private set; }
        internal virtual void onConnected(HandshakeDto hd, LocalPlayerRepo lpr) {
            try {
                handshakeDto = hd;
                onConnectCbs.done();
            } catch (Exception e) {
                Debug.Log(e);
            }
        }


        public virtual AllSyncData getCurrentInfoFunc(LocalPlayerRepo localRepo, bool hasCommData) {
            AllSyncData ad = new AllSyncData();
            if (hasCommData) {
                foreach (CommRemoteObject lo in sc.getCommRemoteRepo().objectMap.Values) {
                    ad.commList.Add(lo.genInitDto());
                }
            }
            return ad;
        }

        public virtual void onRemoteFirstSync(RemotePlayerRepo rpr, AllSyncData msg) {
        }

        private List<Action> onSelfInRoomCbs = new List<Action>();
        internal void addOnSelfInRoomCb(Action a) { if (onSelfInRoomCbs == null) a(); else onSelfInRoomCbs.Add(a); }
        public virtual void onSelfInRoom(LocalPlayerRepo lpr, Action inRoomMark) {
            Debug.Log("onSelfInRoom inject ");
            inRoomMark();
            onSelfInRoomCbs.ForEach(a => { a(); });
            onSelfInRoomCbs = null;
        }

        public void addOnAllInRoomCb(Action a) { onAllInRoomCbs.add(a); }
        private CallbackList onAllInRoomCbs = new CallbackList();
        public virtual void onAllRemotePlayerReadyed(LocalPlayerRepo localRepo) {
            onAllInRoomCbs.done();
            Debug.Log("onAllRemotePlayerReadyed");
        }

        public bool isAllInRoomed() {
            return onAllInRoomCbs.isDone();
        }

        public virtual RemoteObject onNewRemoteObjectCreated(RemotePlayerRepo rpr, RemoteData rd) {
            if (remoteObjectCreater == null) throw new NotImplementedException();
            return remoteObjectCreater.onNewRemoteObjectCreated(rpr, rd);
        }

        public LocalObject handoverToMe(RemoteObject ro) {
            return null;
        }

        public virtual bool handoverToOther(RemoteObject ro) {
            return false;
        }

        public virtual void onConnectedClose(string msg) {
        }

        public virtual void onServerShutdown(float cutTime) {
        }

        internal virtual void onError(ErrorBundle str) {
            Debug.LogError("onError=" + str.ToString());
            Destroy(gameObject);
        }


    }
}
