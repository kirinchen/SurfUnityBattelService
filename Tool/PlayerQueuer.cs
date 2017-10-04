using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class PlayerQueuer : MonoBehaviour {
        public static PlayerQueuer instance { get; private set; }
        private List<string> playerIds = new List<string>();
        public string tokenPlayer { get; private set; }
        private List<Action<string>> tokenPlayerChangeListeners = new List<Action<string>>();
        private List<Action<string>> playerIntoListeners = new List<Action<string>>();
        public string meId { get; private set; }


        void Awake() {
            instance = this;
        }

        public int getSize() {
            return playerIds.Count;
        }

        public void addPlayer(string id) {
            meId = id;
            if (playerIds.Contains(id)) return;
            playerIds.Add(id);
            playerIntoListeners.ForEach(a => { a(id); });
            if (playerIds.Count == 1) {
                setTokenPlayer(playerIds[0]);
            }
        }

        public bool isToken() {
            if (playerIds.Count <= 0) return false;
            if (string.IsNullOrEmpty(meId)) return false;
            return string.Equals(tokenPlayer, meId);
        }

        public void addPlayerIntoListener(Action<string> a) {
            playerIntoListeners.Add(a);
        }

        internal void nextToke() {
            if (!isToken()) return;
            if (playerIds.Count <= 1) return;
            int ci = getTokenIdx();
            ci = (ci + 1) % getSize();
            setTokenPlayer(getTokenByIdx(ci));
        }

        public string getTokenByIdx(int idx) {
            return playerIds[idx];
        }

        public int getTokenIdx() {
            return playerIds.FindIndex(p => { return string.Equals(p, tokenPlayer); });
        }

        public void addTokenPlayerChangeListener(Action<string> a) {
            tokenPlayerChangeListeners.Add(a);
        }

        public void setTokenPlayer(string id) {
            if (!playerIds.Contains(id)) throw new NullReferenceException("not find this id=" + id);
            tokenPlayer = id;
            tokenPlayerChangeListeners.ForEach(a => { a(id); });
        }

        public Data getCurrentData() {
            Data d = new Data();
            d.playerIds = playerIds;
            d.tokenPlayer = tokenPlayer;
            return d;
        }

        public void setByData(Data d) {
            playerIds = d.playerIds;
            tokenPlayer = d.tokenPlayer;
        }


        public class Data : RemoteData {
            public List<string> playerIds = new List<string>();
            public string tokenPlayer;
        }


    }
}
