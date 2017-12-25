using Newtonsoft.Json;
using surfm.tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStomp;

namespace RFNEet {
    public class StompIniter {

        internal StompClient client { get; private set; }
        internal string roomId { get; private set; }
        private CallbackListT<string> onHandshakes = new CallbackListT<string>();
        private CallbackListT<object> onConnects = new CallbackListT<object>();
        private bool connectFetched = false;

        public StompIniter(StompClient c, string roomId) {
            client = c;
            this.roomId = roomId;
        }

        internal void connect() {
            if (connectFetched) return;
            connectFetched = true;
            client.StompConnect(onConnected);

        }

        public StompIniter addOnHandshake(Action<string> os) {
            onHandshakes.add(os);
            return this;
        }

        public StompIniter addOnConnects(Action<object> os) {
            onConnects.add(os);
            return this;
        }

        private void onConnected(object obj) {
            client.Subscribe("/app/" + roomId + "/joinBattle/" + Time.time, (message) => {
                onHandshakes.done(message);
            });
            send(client, "/app/" + roomId + "/ready", null);
            onConnects.done(obj);
        }

        public static void send(StompClient client, string path, object o) {
            string json = "";
            if (o != null) {
                json = JsonConvert.SerializeObject(o);
            }
            client.SendMessage(path, json);
        }
    }
}
