using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public abstract class PidGeter : MonoBehaviour {

        public static PidGeter instance;

        private string pid = null;

        private string _getPid() {
            if (string.IsNullOrEmpty(pid)) {
                pid = fetchPid();
            }
            return pid;
        }

        internal abstract string fetchPid();

        public void setPid(string p) {
            pid = p;
        }

        public static string getPid() {
            return getInstance()._getPid();
        }

        public static PidGeter getInstance() {
            if (instance == null) {
                instance = FindObjectOfType<PidGeter>();
            }
            if (instance == null) {
                GameObject go = new GameObject("#PidGeterAuto");
                instance = go.AddComponent<PidGeterAuto>();
            }
            return instance;
        }
    }

    public class PidGeterAuto : PidGeter {

        internal override string fetchPid() {
            return null;
        }
    }

}
