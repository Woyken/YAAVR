using UnityEngine;

namespace ExtensionMethods
{
    public static class VectorExtensions
    {
        public static Vector3 WithX(this Vector3 vec, float x)
        {
            var vec2 = vec;
            vec2.x = x;
            return vec2;
        }

        public static Vector3 WithY(this Vector3 vec, float y)
        {
            var vec2 = vec;
            vec2.y = y;
            return vec2;
        }

        public static Vector3 WithZ(this Vector3 vec, float z)
        {
            var vec2 = vec;
            vec2.z = z;
            return vec2;
        }
    }   
}