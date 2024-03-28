using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Scripts.Static
{

    
    public static class VectorConverter
    {
        private const float unicodeOverDeg = 181.9444444444f;
        private const float degOverUnicode = 0.005496183206f;
        
       
        public static Vector3[] CodeToVector3RotArray(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            
            // count chars == !

            int length = (value.Length + 2 * value.Count(c => c == '!')) / 3;
            
            Vector3[] jArr = new Vector3[length];
            
            var j = 0;
            
            for (int i = 0; i < length; i++)
            {
                if(j >= value.Length)
                    break;
                
                if (value[j] == '!')
                {
                    jArr[i] = Vector3.zero;
                    j++;
                    continue;
                }

                jArr[i] = CodeToVec3Rot(value[j], value[j+1], value[j+2]);
                j += 3;
            }

            return jArr;
        }
        public static Quaternion[] CodeToQuaternionArray(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            
            var vec3s = CodeToVector3RotArray(value);
            var quaternions = new Quaternion[vec3s.Length];
            for (int i = 0; i < vec3s.Length; i++)
            {
                quaternions[i] = Quaternion.Euler(vec3s[i]);
            }

            return quaternions;
        }

        public static Quaternion[] TransfArrayToQuaternionArray(in Transform[] points)
        {
            if( points == null || points.Length == 0)
            {
                return null;
            }
            var jArr = new Quaternion[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                jArr[i] = points[i].localRotation;
            }

            return jArr;
        }
        
        public static string Vec3RotArrayToCode(Vector3[] value)
        {
            
            if (value == null || value.Length == 0)
            {
                return null;
            }
            string s = "";
            for (int i = 0; i < value.Length; i++)
            {
                s += VecToCodeRot(value[i]);
            }

            return s;
        }
        public static string QuaternionArrayToCode(Quaternion[] value)
        {
            
            if (value == null || value.Length == 0)
            {
                return "";
            }

            string s = "";
            for (int i = 0; i < value.Length; i++)
            {
                s += VecToCodeRot(value[i].eulerAngles);
            }

            return s;
        }

        private static Vector3 checkAngles(Vector3 vec)
        {
            if (vec.x < 0) vec.x = 360 + vec.x;
            if (vec.y < 0) vec.y = 360 + vec.y;
            if (vec.z < 0) vec.z = 360 + vec.z;
            return vec;
        }

        
       // private static string Round(float value) => string.Format("{0:N"+$"{quality}"+"}", value);

       public static Vector3[] TransfToPos(in Transform[] transf)
        {
            Vector3[] positions = new Vector3[transf.Length];
            for (int i = 0; i < transf.Length; i++)
            {
                if (transf[i] == null) continue;
                
                positions[i] = transf[i].position;
            } 

            return positions;
        }
       public static Vector3 CodeToVec3Rot(char x, char y, char z) // 36, 65536
        {
            if (x == '!' || y == '!' || z == '!')
                throw new CharToVec3Exception('!');

            var vec = new Vector3();
            
            vec.x = (x-36) * degOverUnicode;
            vec.y = (y-36) * degOverUnicode;
            vec.z = (z-36) * degOverUnicode;
            return vec;
        }

        public static Vector3 CodeToVec3Pos(string s) => s == "!" || s == "" ? Vector3.zero : CodeToVec3Pos(s[0], s[1], s[2]);
        public static Vector3 CodeToVec3Pos(char x, char y, char z)
        {
            if(x == '!' || y == '!' || z == '!')
                throw new CharToVec3Exception('!');
            var vec = new Vector3();
            vec.x = (x - 32750) / 10000f;
            vec.y = (y - 32750) / 10000f;
            vec.z = (z - 32750) / 10000f;
            return vec;
        }

    
        public static string VecToCodeRot(Vector3 vec)
        { 
            if(isZeroVector(vec))
                return "!";
            return ""+FloatToChar(vec.x) + FloatToChar(vec.y) + FloatToChar(vec.z);
        }

        public static bool isZeroVector(Vector3 vec)
        {
            return vec == Vector3.zero ||
                   Math.Abs(vec.x) < 0.004f && Math.Abs(vec.y) < 0.004f && Math.Abs(vec.z) < 0.004f;
        }
        public static string VecToCodePos(Vector3 vec)
        {
            if(vec == Vector3.zero)
                return "!";
            
            var s = "";
            s += UnsignedFloatToChar(vec.x);
            s += UnsignedFloatToChar(vec.y);
            s += UnsignedFloatToChar(vec.z);
            return s;
        }

        private static char FloatToChar(float f) // range [0, 360]
        {
            if (f < 0)
                throw new FloatToCharException(f);
            f *= unicodeOverDeg;
            f += 36;
            return (char)((int)f);
        }
        
        private static char UnsignedFloatToChar(float f) // range [-3.2750, 3.2750] - optimal way for save v3 position
        {
            if (f < -3.2750 || f > 3.2750)
                throw new FloatToCharException(f); 
            

            var rounded = Math.Round(f, 4) * 10000 + 32750;
            if (rounded is >= 0 and < 36)
                rounded = 36;
            return (char)(Math.Clamp(rounded, 36, 65536));
        }

    }
}