using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class RFQuaternion {
        public float w;

        public float x;

        public float y;

        public float z;

        public RFQuaternion(Quaternion q) {
            this.w = q.w;
            this.x = q.x;
            this.y = q.y;
            this.z = q.z;
        }

        public Quaternion toQuaternion() {
            Quaternion ans = new Quaternion(x, y, z, w);
            return ans;
        }
    }
}
