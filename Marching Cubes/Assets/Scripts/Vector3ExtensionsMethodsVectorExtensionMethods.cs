using UnityEngine;

namespace ExtensionMethods {
    public static class VectorExtensionMethods
    {
        public static float SumDimensions(this Vector3 v) { return v.x + v.y + v.z; }
        public static float SumDimensions(this Vector3Int v) { return v.x + v.y + v.z; }

        public static float ProductDimensions(this Vector3 v) { return v.x * v.y * v.z; }
        public static float ProductDimensions(this Vector3Int v) { return v.x * v.y * v.z; }

        public static float MinDimension(this Vector3 v) { return Mathf.Min(v.x, Mathf.Min(v.y, v.z)); }
        public static float MinDimension(this Vector3Int v) { return Mathf.Min(v.x, Mathf.Min(v.y, v.z)); }

        public static Vector3 Multiply(this Vector3 v, Vector3 v2) { return new Vector3(v.x * v2.x, v.y * v2.y, v.z * v2.z); }
    }
}
