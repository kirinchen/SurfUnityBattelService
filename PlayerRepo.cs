using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStomp;

namespace RFNEet {
    public abstract class PlayerRepo<T> where T : SyncObject {

        private string pid;
        private Dictionary<string, T> objectMap = new Dictionary<string, T>();

        protected PlayerRepo(string pid) {
            this.pid = pid;
        }
        
        public void create() {
            T t = newOne();
            t.pid = pid;
            t.oid = UidUtils.getRandomString(8);
        }

        public abstract T newOne();



    }
}
