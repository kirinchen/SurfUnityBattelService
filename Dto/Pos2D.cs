using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class Pos2D  {

        public float x, y;

        public Pos2D() {
        }

        public Pos2D(Vector2 v) {
            this.x = v.x;
            this.y = v.y;
        }

        public Pos2D(Vector3 v) {
            this.x = v.x;
            this.y = v.z;
        }

        public Pos2D(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public Vector2 toVector2() {
            Vector2 ans = new Vector2(x, y);
            return ans;
        }

        public override bool Equals(object obj) {
            if (obj == null) return false;
            if ((obj is Pos2D)) {
                Pos2D _p = obj as Pos2D;
                return _p.x == x && _p.y == y;
            } else if (obj is Vector2) {
                Vector2 _p = (Vector2)obj;
                return _p.x == x && _p.y == y;
            }   
            return false;
        }

        public Vector3 toVector3(float org3dY) {
            Vector3 ans = new Vector3(x, org3dY, y);
            return ans;
        }

        public override string ToString() {
            return x + "-" + y;
        }

        public override int GetHashCode() {
            int prime = 31;
            int result = 1;
            result = prime * result + x.GetHashCode();
            result = prime * result + y.GetHashCode();
            return result;
        }

        public static List<Pos2D> convert(Vector2[] vector2) {
            List<Pos2D> ans = new List<Pos2D>();
            foreach (Vector2 v in vector2) {
                ans.Add(new Pos2D(v));
            }
            return ans;
        }

        public static Vector2[] conver2Array(List<Pos2D> l) {
            Vector2[] vs = new Vector2[l.Count];
            for (int i = 0; i < vs.Length; i++) {
                vs[i] = l[i].toVector2();
            }
            return vs;
        }

    }
}
