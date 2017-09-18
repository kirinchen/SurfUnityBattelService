using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class Pos3D {

        public float x, y, z;

        public Pos3D() {
        }


        public Pos3D(Vector3 v) {
            this.x = v.x;
            this.y = v.z;
            z = v.z;
        }

        public override bool Equals(object obj) {
            if (obj == null) return false;
            if ((obj is Pos3D)) {
                Pos3D _p = obj as Pos3D;
                return _p.x == x && _p.y == y && _p.z == z;
            } else if (obj is Vector3) {
                Vector3 _p = (Vector3)obj;
                return _p.x == x && _p.z == y && _p.z == z;
            }
            return false;
        }

        public Vector3 toVector3() {
            Vector3 ans = new Vector3(x, y, z);
            return ans;
        }

        public override string ToString() {
            return "x=" + x + " y=" + y;
        }
    }
}
