using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RFNEet {
    public class PlayerListChecker {

        private static readonly int defaultCheckCount = 2;

        private List<string> otherPlayerList;
        internal List<string> reslut {
            get; private set;
        }
        internal Dictionary<string, string> reslutMap {
            get; private set;
        }

        internal PlayerListChecker(List<string> otherPlayerList) {
            reslutMap = new Dictionary<string, string>();
            reslut = new List<string>();
            this.otherPlayerList = otherPlayerList;
        }

        internal PlayerListChecker clac() {
            otherPlayerList.Sort((a, b) => { return a.GetHashCode().CompareTo(b.GetHashCode()); });
            foreach (string s in otherPlayerList) {
                if (!s.Equals(CommRemoteRepo.COMM_PID)) {
                    string k = calcNewIndex(s);
                    reslut.Add(k);
                    reslutMap.Add(k,s);
                }
            }
            return this;
        }

        private string calcNewIndex(string s) {
            int length = defaultCheckCount;
            while (reslut.Contains(s.Substring(0, length))) {
                length++;
                if (length > s.Length) {
                    throw new Exception(s + " calcNewIndex lenth s>" + s.Length);
                }
            }
            return s.Substring(0, length);
        }
    }
}
