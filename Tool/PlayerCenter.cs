using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class PlayerCenter : MonoBehaviour {
        public static PlayerCenter isntance { get; private set; }
        private List<string> playerIds = new List<string>();
        public string tokenPlayer { get; private set; }
        private List<Action<string>> tokenPlayerChangeListeners = new List<Action<string>>();
        private List<Action<string>> playerIntoListeners = new List<Action<string>>();

        void Awake() {
            isntance = this;
        }

        public void addPlayer(string id) {
            playerIds.Add(id);
            playerIntoListeners.ForEach(a=> { a(id); });
        }

        public void addPlayerIntoListener(Action<string> a) {
            playerIntoListeners.Add(a);
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
