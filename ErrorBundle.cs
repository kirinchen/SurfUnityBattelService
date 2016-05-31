using UnityEngine;
using System.Collections;

namespace  RFNEet  {
    public class ErrorBundle  {
        public enum Type {
            HandShake,
            Runtime,
            SeverError
        }
        public Type type {
            get;private set;
        }
        public string message;

        public ErrorBundle(Type t) {
            type = t;
        }




    }
}
