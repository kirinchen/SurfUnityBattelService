using UnityEngine;
using System.Collections;
using System;

namespace UnityStomp {
    public delegate void OnMessageListener(string msg);

    public interface StompClient {
        void SetCookie(string name, string value);

        void StompConnect(Action<object> openAction);

        void setOnError(Action<string> errorCb);

        void Subscribe(string destination, OnMessageListener act);

        void unSubscribe(string destination);

        void SendMessage(string destination, string message);

        void CloseWebSocket();

        string getSessionId();

    }
}
