using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class RFServerStorer : MonoBehaviour {

        private static RFServerStorer instance;
        private List<URestApi> uApis = new List<URestApi>();
        private PingDtoFinder pingFinder;

        void Awake() {
            instance = this;
            pingFinder = GetComponent<PingDtoFinder>();
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

        public void findPingBetter(string gameKindUid, object filter, Action<PingDtoFinder> onDone, Predicate<PingDto.RoomI> roomFilter=null) {
            roomFilter = roomFilter == null ? ri => { return true; } : roomFilter;
            List<PingBundle> pbs = new List<PingBundle>();
            foreach (URestApi a in uApis) {
                PingBundle pb = new PingBundle(gameKindUid, filter, a, roomFilter);
                pb.pingOne();
                pbs.Add(pb);
            }
            StartCoroutine(waitPings(onDone, pbs));
        }

        public List<URestApi> listAll() {
            return uApis;
        }

        private IEnumerator waitPings(Action<PingDtoFinder> onDone, List<PingBundle> pbs) {
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
            pingFinder.setup(pbs);
            onDone(pingFinder);
        }

        public static RFServerStorer getInstance() {
            return instance;
        }

        public static void createInstance(Transform p, string loadName = "RFServerStorer") {
            RFServerStorer temp = Resources.Load<RFServerStorer>(loadName);
            Instantiate(temp, p);
        }
    }
}
