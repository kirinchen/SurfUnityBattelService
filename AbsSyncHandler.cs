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
        internal Action<PingBundle> onPingError = (p) => { };

        public void Awake() {
            state = Status.Idle;
            sc = GetComponent<SyncCenter>();
        }

        public void connect(PingBundle r, bool localDebug = false) {
            state = Status.Connecting;
            try {
                if (r.ok) {
                    Debug.Log(r.bestRoom.wsUrl + " connected");
                    sc.init(r.bestRoom.wsUrl, r.bestRoom.roomId, this, localDebug);
                    sc.connect(onConnected);
                } else {
                    onPingError(r);
                }
            } catch (Exception e) {
                onPingError(r);
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
            inRoomMark();
            onSelfInRoomCbs.ForEach(a => { a(); });
            onSelfInRoomCbs = null;
            Debug.Log("onSelfInRoom inject ");
        }

        public void addOnAllInRoomCb(Action a) { if (onAllInRoomCbs == null) a(); else onAllInRoomCbs.Add(a); }
        private List<Action> onAllInRoomCbs = new List<Action>();
        public virtual void onAllRemotePlayerReadyed(LocalPlayerRepo localRepo) {
            onAllInRoomCbs.ForEach(a => { a(); });
            onSelfInRoomCbs = null;
        }

        public bool isAllInRoomed() {
            return onAllInRoomCbs == null;
        }

        public abstract RemoteObject onNewRemoteObjectCreated(RemotePlayerRepo rpr, RemoteData rd);

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


    }
}
