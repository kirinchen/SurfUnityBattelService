using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class PlayerQueuer : MonoBehaviour {
        public static PlayerQueuer instance { get; private set; }

        public delegate void OnTokenChange(string orgT, string newT);
        private List<OnTokenChange> tokenPlayerChangeListeners = new List<OnTokenChange>();
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

        public DataProvider data;

        void Awake() {
            instance = this;
        }

        public int getSize() {
            return data.playerIds().Count;
        }

        public void addPlayer(string id) {
            meId = id;
            if (data.playerIds().Contains(id)) return;
            data.addPlayer(id);
            playerIntoListeners.ForEach(a => { a(id); });
            if (data.playerIds().Count == 1) {
                setTokenChange(data.playerIds()[0], TokePost.FIELD);
            }
        }

        public bool isToken() {
            if (data.playerIds().Count <= 0) return false;
            if (string.IsNullOrEmpty(meId)) return false;
            return string.Equals(data.tokenPlayer(), meId);
        }

        public void addPlayerIntoListener(Action<string> a) {
            playerIntoListeners.Add(a);
        }

        internal void nextToke() {
            if (!isToken()) return;
            if (data.playerIds().Count <= 1) return;
            int ci = getTokenIdx();
            ci = (ci + 1) % getSize();
            setTokenChange(getTokenByIdx(ci), TokePost.ALL);
        }

        public string getTokenByIdx(int idx) {
            return data.playerIds()[idx];
        }

        public int getTokenIdx() {
            return data.playerIds().FindIndex(p => { return string.Equals(p, data.tokenPlayer()); });
        }

        public void addTokenPlayerChangeListener(OnTokenChange a) {
            tokenPlayerChangeListeners.Add(a);
        }

        public void triggerTokenChange(string id) {
            setTokenChange(id, TokePost.NONE);
        }

        public void setTokenChange(string id, TokePost p) {
            if (string.IsNullOrEmpty(id)) return;
            string orgT = data.tokenPlayer();
            if (string.Equals(id, orgT)) return;
            if (!data.playerIds().Contains(id)) throw new NullReferenceException("not find this id=" + id);
            data.setTokenPlayer(id, p);
            tokenPlayerChangeListeners.ForEach(a => { a(orgT, id); });
        }

        public int getMyIndex() {
            return data.playerIds().FindIndex(p => { return string.Equals(p, meId); });
        }

        public DataProvider getCurrentData() {
            return data;
        }
        internal string getTokenPlayer() {
            return data.tokenPlayer();
        }


        public void setDataProvider(DataProvider d) {
            data = d;
        }


        public enum TokePost {
            NONE, ALL, FIELD
        }

        public interface DataProvider {
            void addPlayer(string id);
            List<string> playerIds();
            void setTokenPlayer(string v, TokePost post);
            string tokenPlayer();
        }


    }
}
