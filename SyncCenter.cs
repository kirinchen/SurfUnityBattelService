using UnityEngine;
using System.Collections;
using System;

namespace RFNEet {
    public class SyncCenter : MonoBehaviour {
        private LocalPlayerRepo localRepo;
        private RemoteApier api;

        void Awake() {
            
        }

        public void init(string url, string roomId) {
            api = new RemoteApier(url,roomId);
        }

        public void connect(Action<LocalPlayerRepo> handshakeCb) {
            api.connect(()=> {
                localRepo = new LocalPlayerRepo(api.meId,api);
                handshakeCb(localRepo);
            });
        }

    }
}
