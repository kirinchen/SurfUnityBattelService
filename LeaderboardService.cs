using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class LeaderboardService : MonoBehaviour {
        private URestApi rest;

        void Awake() {
            rest = GetComponent<URestApi>();
        }

        public void isRegistered() {
            rest.get("/registered/{deviceId}",(s)=> {

            },(e,s,r)=> {

            });
        }

    }
}
