using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class PingBetter  {

        public delegate void OnDone(PingDtoFinder f);

        private List<URestApi> uApis = new List<URestApi>();
        private MonoBehaviour mono;
        private List<OnDone> onDones = new List<OnDone>();
        private Predicate<PingDto.RoomI> roomFilter =(i)=> { return true; };
        private object filter;
        private PingDtoFinder pingFinder;
        private Coroutine coroutine;

        internal PingBetter(PingDtoFinder p, MonoBehaviour m, List<URestApi> us ) {
            pingFinder = p;
            mono = m;
            uApis = us;
        }

        public PingBetter setFilter(object f) {
            filter = f;
            return this;
        }

        public PingBetter setRoomFilter(Predicate<PingDto.RoomI> p) {
            roomFilter = p;
            return this;
        }

        public PingBetter addOnDone(OnDone d) {
            onDones.Add(d);
            return this;
        }

        public void ping(string gameKindUid) {
            List<PingBundle> pbs = new List<PingBundle>();
            foreach (URestApi a in uApis) {
                PingBundle pb = new PingBundle(gameKindUid, filter, a, roomFilter);
                pb.pingOne();
                pbs.Add(pb);
            }
             coroutine = mono.StartCoroutine(waitPings( pbs));
        }

        public RoomFindOrCreater<T> getRoomFindOrCreater<T>(RoomService.CreateRoomData d) {
            RoomFindOrCreater<T> ans = new RoomFindOrCreater<T>(d,this);
            return ans;
        }

        private IEnumerator waitPings(List<PingBundle> pbs) {
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
            onDones.ForEach(d=> { d(pingFinder); });
        }

        internal void terminal() {
            mono.StopCoroutine(coroutine);
            uApis.ForEach(u => { u.abortAll(); });
        }
    }
}
