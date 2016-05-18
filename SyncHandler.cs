using UnityEngine;
using System.Collections;
namespace RFNEet {
    public interface SyncHandler {

        object getCurrentInfoFunc();

        void onRemoteFirstSync(RemotePlayerRepo rpr, string msg);

        void onSelfInRoomAction(LocalPlayerRepo lpr);
    }
}
