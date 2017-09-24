using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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

        public void connect(bool ok, string wsUrl, string roomId, bool localDebug = false) {
            state = Status.Connecting;
            try {
                if (ok) {
                    Debug.Log(wsUrl + " connected ");
                    sc.init(wsUrl, roomId, localDebug);
                    sc.connect(onConnected);
                } else {
                    onPingError(wsUrl, roomId);
                }
            } catch (Exception e) {
                onPingError(wsUrl, roomId);
            }
        }

        private List<Action> onConnectCbs = new List<Action>();
        public void addOnConnectedCb(Action a) {
            if (onConnectCbs == null) a(); else onConnectCbs.Add(a);
        }
        internal HandshakeDto handshakeDto { get; private set; }
        internal virtual void onConnected(HandshakeDto hd, LocalPlayerRepo lpr) {
            try {
                handshakeDto = hd;
                onConnectCbs.ForEach(a => { a(); });
                onConnectCbs = null;
            } catch (Exception e) {
                Debug.Log(e);
            }
        }

        public abstract AllSyncData getCurrentInfoFunc(LocalPlayerRepo localRepo, bool hasCommData);

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

        public void addOnAllInRoomCb(Action a) { if (onAllInRoomCbs == null) a(); else onAllInRoomCbs.Add(a); }
        private List<Action> onAllInRoomCbs = new List<Action>();
        public virtual void onAllRemotePlayerReadyed(LocalPlayerRepo localRepo) {
            onAllInRoomCbs.ForEach(a => { a(); });
            onAllInRoomCbs = null;
            Debug.Log("onAllRemotePlayerReadyed");
        }

        public bool isAllInRoomed() {
            return onAllInRoomCbs == null;
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
