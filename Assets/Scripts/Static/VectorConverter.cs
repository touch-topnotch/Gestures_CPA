using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Static
{

    
    public class VectorConverter : JsonConverter<Vector3>
    {
        private const float unicodeOverDeg = 181.9444444444f;
        private const float degOverUnicode = 0.005496183206f;
        
       
        public static Vector3[] CodeToVector3RotArray(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            int length = value.Length / 3;
            Vector3[] jArr = new Vector3[length];
            for (int i = 0; i < length; i++)
            {
         
                jArr[i] = CodeToVec3Rot(value[i*3], value[i*3+1], value[i*3+2]);
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
                return null;
            }

            string s = "";
            for (int i = 0; i < value.Length; i++)
            {
                s += VecToCodeRot(value[i].eulerAngles);
            }

            return s;
        }

        
       // private static string Round(float value) => string.Format("{0:N"+$"{quality}"+"}", value);

        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        { ;
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WritePropertyName("z");
            writer.WriteValue(value.z);
            writer.WriteEndObject();
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            // Load the JSON object from the reader
            JObject obj = JObject.Load(reader);

            // Extract the x, y, and z values from the JSON object
            float x = obj.GetValue("x")!.ToObject<float>();
            float y = obj.GetValue("y")!.ToObject<float>();
            float z = obj.GetValue("z")!.ToObject<float>();

            // Create and return a new Vector3 object with the extracted values
            return new Vector3(x, y, z);
        }

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

        public static Vector3 CodeToVec3Rot(string s) => CodeToVec3Rot(s[0], s[1], s[2]);
        public static Vector3 CodeToVec3Rot(char x, char y, char z) // 36, 65536
        {
            var vec = new Vector3();
            
            vec.x = (x-36) * degOverUnicode;
            vec.y = (y-36) * degOverUnicode;
            vec.z = (z-36) * degOverUnicode;
            return vec;
        }
        public static Vector3 CodeToVec3Pos(string s) => CodeToVec3Pos(s[0], s[1], s[2]);
        public static Vector3 CodeToVec3Pos(char x, char y, char z)
        {
            var vec = new Vector3();
            vec.x = (x - 32750) / 10000f;
            vec.y = (y - 32750) / 10000f;
            vec.z = (z - 32750) / 10000f;
            return vec;
        }

        public static string VecToCodeRot(Vector3 vec)
        {
            var s = "";
            s += FloatToChar(vec.x);
            s += FloatToChar(vec.y);
            s += FloatToChar(vec.z);
            return s;
        }
        public static string VecToCodePos(Vector3 vec)
        {
            var s = "";
            s += UnsignedFloatToChar(vec.x);
            s += UnsignedFloatToChar(vec.y);
            s += UnsignedFloatToChar(vec.z);
            return s;
        }

        private static char FloatToChar(float f)
        {
            f *= unicodeOverDeg;
            f += 36;
            return (char)((int)f);
        }
        
        private static char UnsignedFloatToChar(float f) // range [-3.2750, 3.2750] - optimal way for save v3 position
        {
            if (f < -3.2750 || f > 3.2750)
            {
                Debug.LogError("UnsignedFloatToChar: float out of range [-3.2750, 3.2750]");
            }

            var rounded = Math.Round(f, 4) * 10000 + 32750;
            if (rounded is >= 0 and < 36)
                rounded = 36;
            return (char)(Math.Clamp(rounded, 36, 65536));
        }
        
        public static Quaternion[] OldCodeToQuat(string[] vec)
        {
            if(vec == null || vec.Length == 0)
                return null;
            
            var jArr = new Quaternion[vec.Length];
            // String example: 
            for(int i = 0; i < vec.Length; i++)
            {
                jArr[i] = Quaternion.Euler(OldCodeToVec(vec[i]));
            }
            return jArr;
        }

        public static Vector3 OldCodeToVec(string vec)
        {
            // "0.0000, 0.0000, 0.0000" to vector;
            if (vec == null)
                return Vector3.zero;
            
            var s = vec.Split(", ");
            return new Vector3(
                float.Parse(s[0],
                    CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(s[1],
                    CultureInfo.InvariantCulture.NumberFormat), 
                float.Parse(s[2],
                    CultureInfo.InvariantCulture.NumberFormat));
        }
    }
}