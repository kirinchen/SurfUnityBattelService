using System.Collections.Generic;

namespace RFNEet {
    public interface SyncHandler {

        AllSyncData getCurrentInfoFunc(LocalPlayerRepo localRepo);

        void onRemoteFirstSync(RemotePlayerRepo rpr, AllSyncData msg);

        void onSelfInRoomAction(LocalPlayerRepo lpr);

        void onAllRemotePlayerReadyed(LocalPlayerRepo localRepo);
    }
}
