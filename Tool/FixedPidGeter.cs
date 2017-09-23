using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStomp;

namespace RFNEet {
    public class FixedPidGeter : PidGeter {

        public static readonly string KEY_PID = "#FixedPidGeterPID";

        public static string getStoredPid() {
            return PlayerPrefs.GetString(KEY_PID);
        }

        public static void setStoredPid(string pid) {
            PlayerPrefs.SetString(KEY_PID, pid);
        }

        internal override string fetchPid() {
            string spid = getStoredPid();
            if (string.IsNullOrEmpty(spid)) {
                spid = UidUtils.getRandomString(12);
                setStoredPid(spid);
            }
            return spid;
        }
    }
}
