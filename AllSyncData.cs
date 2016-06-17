using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RFNEet {
    public class AllSyncData {

        public List<RemoteData> objectList = new List<RemoteData>();
        public List<RemoteData> commList = new List<RemoteData>();


    }

    internal class AllSyncDataResp : InboxData {
        
        public List<object> objectList = new List<object>();
        public List<object> commList = new List<object>();

        public AllSyncData toAllSyncData() {
            AllSyncData ad = new AllSyncData();
            ad.objectList = convert(objectList);
            ad.commList = convert(commList);
            return ad;
        }

        private List<RemoteData> convert(List<object> ol) {
            List<RemoteData> ans = new List<RemoteData>();
            foreach (object o in ol) {
                string s = JsonConvert.SerializeObject(o);
                //TODO Opp perfomace
                RemoteData rd = JsonConvert.DeserializeObject<RemoteData>(s);
                rd.setSource(s);
                ans.Add(rd);
            }
            return ans;
        }

    }

}
