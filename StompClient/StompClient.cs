﻿using UnityEngine;
using System.Collections;
using System;

namespace UnityStomp {
    public interface StompClient {
        void SetCookie(string name, string value);

        void StompConnect(Action<object> openAction);

        void Subscribe(string destination, OnMessageListener act);

        void SendMessage(string destination, string message);

        void CloseWebSocket();
    }
}
