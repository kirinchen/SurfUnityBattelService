using System.Collections.Generic;

namespace RFNEet {
    public interface SyncHandler {

        AllSyncData getCurrentInfoFunc();

        void onRemoteFirstSync(RemotePlayerRepo rpr, AllSyncData msg);

        void onSelfInRoomAction(LocalPlayerRepo lpr);
    }
}
