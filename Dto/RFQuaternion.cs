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

        public override bool Equals(object obj) {
            if (obj == null) return false;
            if (obj == this) return true;
            if ((obj is RFQuaternion)) {
                RFQuaternion q = obj as RFQuaternion;
                return q.w == w && q.x == x && q.y == y && q.z == z;
            } else if (obj is Quaternion) {
                Quaternion q = (Quaternion)obj;
                return q.w == w && q.x == x && q.y == y && q.z == z;
            }
            return false;
        }

        public override string ToString() {
            return string.Format("x={0} y={1} z={2} w={3}", x, y, z, w);
        }
    }
}
