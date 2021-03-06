﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class BuffAndConnect : MonoBehaviour {
        public string gameKindUid;
        public delegate void OnCustomPing(Action<PingBundle> ocb);
        internal OnCustomPing onCustomPing;

        private List<PingBundle> bundles = new List<PingBundle>();
        private Action<PingBundle> onConnted;


        public void connect(Action<PingBundle> a) {
            onConnted = a;
            StartCoroutine(buffFPSToConnect(() => {

                runOnBuffAready();
            }));
        }



        private void runOnBuffAready() {
            if (onCustomPing == null) {
                PingBetter pb= RFServerStorer.getInstance().genPingBetter();
                pb.addOnDone(f=> {
                    onConnted(f.getBestRoom().pingBundle);
                });
                pb.ping(gameKindUid);

            } else {
                onCustomPing(onConnted);
            }
        }


        internal static IEnumerator buffFPSToConnect(Action a) {
            float wAt = Time.time;
            while ((Time.time - wAt) < 12f) {
                yield return new WaitForSeconds(0.5f);
                float fps = 1 / Time.smoothDeltaTime;
                if (fps > 27f) {
                    break;
                }
            }
            a();
        }


    }
}
