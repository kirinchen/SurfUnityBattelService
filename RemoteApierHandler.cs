using UnityEngine;
using System.Collections;
namespace RFNEet {
    public interface RemoteApierHandler {

        void onNewPlayerJoined(RemoteBroadcastData broadcastData);
        void onRemoteFirstSync(string sid, AllSyncDataResp allData);
        void onPlayerLeaved(string sid, string hahverId);
        void onPlayerLeavedByIndex(string sid);
        void onRepairLostPlayer(string sid);
        void onErrorCb(ErrorBundle error);
        void onConnectClosedCb(string msg);
        void onBroadcast(RemoteBroadcastData data);
        void repairMissObject(string missWho, string moid);
        void onNewPlayerReadyed(string sid);
        void onRemotePlayTellMyObject(InboxTellObjectData iaod);
        void onServerShutdown(float cutTime);
    }
}
