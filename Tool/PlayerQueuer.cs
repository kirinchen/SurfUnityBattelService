using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class PlayerQueuer : MonoBehaviour {
        public static PlayerQueuer instance { get; private set; }

        //private List<string> playerIds = new List<string>();
        //public string tokenPlayer { get; private set; }
        private List<Action<string>> tokenPlayerChangeListeners = new List<Action<string>>();
        private List<Action<string>> playerIntoListeners = new List<Action<string>>();
        public string _meId;
        public string meId
        {
            get { return _meId; }
            set
            {
                _meId = value;
            }
        }

        public Data data = new Data();

        void Awake() {
            instance = this;
        }

        public int getSize() {
            return data.playerIds.Count;
        }

        public void addPlayer(string id) {
            meId = id;
            if (data.playerIds.Contains(id)) return;
            data.playerIds.Add(id);
            playerIntoListeners.ForEach(a => { a(id); });
            if (data.playerIds.Count == 1) {
                setTokenPlayer(data.playerIds[0]);
            }
        }

        public bool isToken() {
            if (data.playerIds.Count <= 0) return false;
            if (string.IsNullOrEmpty(meId)) return false;
            return string.Equals(data.tokenPlayer, meId);
        }

        public void addPlayerIntoListener(Action<string> a) {
            playerIntoListeners.Add(a);
        }

        internal void nextToke() {
            if (!isToken()) return;
            if (data.playerIds.Count <= 1) return;
            int ci = getTokenIdx();
            ci = (ci + 1) % getSize();
            setTokenPlayer(getTokenByIdx(ci));
        }

        public string getTokenByIdx(int idx) {
            return data.playerIds[idx];
        }

        public int getTokenIdx() {
            return data.playerIds.FindIndex(p => { return string.Equals(p, data.tokenPlayer); });
        }

        public void addTokenPlayerChangeListener(Action<string> a) {
            tokenPlayerChangeListeners.Add(a);
        }

        public void setTokenPlayer(string id) {
            if (!data.playerIds.Contains(id)) throw new NullReferenceException("not find this id=" + id);
            data.tokenPlayer = id;
            tokenPlayerChangeListeners.ForEach(a => { a(id); });
        }

        public int getMyIndex() {
            return data.playerIds.FindIndex(p => { return string.Equals(p, meId); });
        }

        public Data getCurrentData() {
            return data;
        }
        internal string getTokenPlayer() {
            return data.tokenPlayer;
        }


        public void setByData(Data d) {
            string oldT = data.tokenPlayer;
            data = d;
            if (!string.Equals(oldT, d.tokenPlayer)) {
                setTokenPlayer(d.tokenPlayer);
            }
        }

        [System.Serializable]
        public class Data : RemoteData {
            public List<string> playerIds = new List<string>();
            public string tokenPlayer;
        }


    }
}
