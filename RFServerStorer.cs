﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class RFServerStorer : MonoBehaviour {

        private static RFServerStorer instance;
        private List<URestApi> uApis = new List<URestApi>();


        void Awake() {
            instance = this;
            injectUaBundles();
        }

        private void injectUaBundles() {
            URestApi[] uapis = GetComponentsInChildren<URestApi>();
            foreach (URestApi api in uapis) {
                if (api.enabled) {
                    uApis.Add(api);
                }
            }
        }

        public void findPingBetter(string gameKindUid, Action<PingBundle> onDone, bool calcRoomScore = false) {
            List<PingBundle> pbs = new List<PingBundle>();
            foreach (URestApi a in uApis) {
                PingBundle pb = new PingBundle(gameKindUid, a, calcRoomScore);
                pb.pingOne();
                pbs.Add(pb);
            }
            StartCoroutine(waitPings(onDone, pbs));
        }

        public List<URestApi> listAll() {
            return uApis;
        }

        private IEnumerator waitPings(Action<PingBundle> onDone, List<PingBundle> pbs) {
            int doneCount = 0;
            float startAt = Time.time;
            while (doneCount < uApis.Count && (Time.time - startAt) < 10) {
                yield return new WaitForSeconds(0.35f);
                doneCount = 0;
                foreach (PingBundle ob in pbs) {
                    if (ob.done) {
                        doneCount++;
                    }
                }
            }
            PingBundle best = null;
            float score = 999999999;
            foreach (PingBundle pb in pbs) {
                if (pb.score < score) {
                    score = pb.score;
                    best = pb;
                }
            }
            onDone(best);
        }

        public static RFServerStorer getInstance() {
            return instance;
        }

        public static void createInstance(Transform p) {
            RFServerStorer temp = Resources.Load<RFServerStorer>("RFServerStorer");
            Instantiate(temp, p);
        }
    }
}