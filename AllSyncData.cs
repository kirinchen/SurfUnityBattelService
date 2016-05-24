using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RFNEet {
    public class AllSyncData {

        public List<RemoteData> objectList = new List<RemoteData>();


    }

    internal class AllSyncDataResp : InboxData {
        
        public List<object> objectList = new List<object>();

        public AllSyncData toAllSyncData() {
            AllSyncData ad = new AllSyncData();
            foreach (object o in objectList) {
                string s = JsonConvert.SerializeObject(o);
                //TODO Opp perfomace
                RemoteData rd = JsonConvert.DeserializeObject<RemoteData>(s);
                rd.setSource(s);
                ad.objectList.Add(rd);
            }
            return ad;
        }

    }

}
