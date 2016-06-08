using System;
using System.Collections.Generic;

namespace RFNEet {
    public interface SyncHandler {

        AllSyncData getCurrentInfoFunc(LocalPlayerRepo localRepo);

        void onRemoteFirstSync(RemotePlayerRepo rpr, AllSyncData msg);

        void onSelfInRoom(LocalPlayerRepo lpr,Action inRoomMark);

        void onAllRemotePlayerReadyed(LocalPlayerRepo localRepo);

        RemoteObject onNewRemoteObjectCreated(RemotePlayerRepo rpr, RemoteData rd);

        LocalObject handoverToMe(RemoteObject ro);

        bool handoverToOther(RemoteObject ro);
    }
}
