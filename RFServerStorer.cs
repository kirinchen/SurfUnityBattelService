using System;
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

        public void findPingBetter(string gameKindUid, Action<PingBundle> onDone, bool calcRoomScore = false, string assignRoomId = null) {
            List<PingBundle> pbs = new List<PingBundle>();
            foreach (URestApi a in uApis) {
                PingBundle pb = new PingBundle(gameKindUid, a, calcRoomScore, assignRoomId);
                pb.pingOne();
                pbs.Add(pb);
            }
            StartCoroutine(waitPings(onDone, pbs, assignRoomId));
        }

        public List<URestApi> listAll() {
            return uApis;
        }

        private IEnumerator waitPings(Action<PingBundle> onDone, List<PingBundle> pbs, string assignRoomId) {
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
            onDone(getBest(pbs, assignRoomId));
        }

        private PingBundle getBest(List<PingBundle> pbs, string assignRoomId) {
            PingBundle best = null;
            float score = 999999999;
            foreach (PingBundle pb in pbs) {
                if (pb.isMathAssignRoomId(assignRoomId)) return pb;
                if (pb.score < score) {
                    score = pb.score;
                    best = pb;
                }
            }
            return best;
        }

        public static RFServerStorer getInstance() {
            return instance;
        }

        public static void createInstance(Transform p,string loadName= "RFServerStorer") {
            RFServerStorer temp = Resources.Load<RFServerStorer>(loadName);
            Instantiate(temp, p);
        }
    }
}
