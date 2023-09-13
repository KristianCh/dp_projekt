using UnityEngine;

namespace Entities.Utils
{
    public static class VectorExtensions
    {
#region Casting
        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }
        
        public static Vector3 ToVector3(this Vector2 vector)
        {
            return new Vector3(vector.x, vector.y, 0);
        }
#endregion
        
#region Component Overrides
        public static Vector2 WithX(this Vector2 vector, float value)
        {
            return new Vector2(value, vector.y);
        }
        
        public static Vector2 WithY(this Vector2 vector, float value)
        {
            return new Vector2(vector.x, value);
        }
        
        public static Vector3 WithX(this Vector3 vector, float value)
        {
            return new Vector3(value, vector.y, vector.z);
        }
        
        public static Vector3 WithY(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, value, vector.z);
        }
        
        public static Vector3 WithZ(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, vector.y, value);
        }

        public static Vector3 WithXY(this Vector4 vector, float x, float y)
        {
            return new Vector3(x, y, vector.z);
        }

        public static Vector3 WithXZ(this Vector4 vector, float x, float z)
        {
            return new Vector3(x, vector.y, z);
        }
        
        public static Vector3 WithYZ(this Vector4 vector, float y, float z)
        {
            return new Vector3(vector.x, y, z);
        }
        
        #endregion
    }
}