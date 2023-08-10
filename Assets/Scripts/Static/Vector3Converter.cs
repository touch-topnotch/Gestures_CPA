using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Static
{

    
    public class Vector3Converter : JsonConverter<Vector3>
    {
        public static int quality = 4;
        public static Vector3[] convertToVector3(string[] value)
        {
            if (value == null || value.Length == 0)
            {
                return null;
            }
            Vector3[] jArr = new Vector3[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                var words = value[i].Split(", ");
                jArr[i].x = float.Parse(words[0]);
                jArr[i].y = float.Parse(words[1]);
                jArr[i].z = float.Parse(words[2]);
            }

            return jArr;
        }
        public static string[] convertToString(Vector3[] value)
        {
            
            if (value == null || value.Length == 0)
            {
                return null;
            }
            string[] jArr = new string[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                jArr[i] = Round(value[i].x) + ", " + Round(value[i].y) + ", " + Round(value[i].z);
            }

            return jArr;
        }

        
        private static string Round(float value) => string.Format("{0:N"+$"{quality}"+"}", value);

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
            float x = obj.GetValue("x").ToObject<float>();
            float y = obj.GetValue("y").ToObject<float>();
            float z = obj.GetValue("z").ToObject<float>();

            // Create and return a new Vector3 object with the extracted values
            return new Vector3(x, y, z);
        }

        public static void LogVec3(Vector3 vec)
        {
            Debug.Log(string.Format("{0:N2}", vec.x) +", "+ string.Format("{0:N2}", vec.y)+", " + string.Format("{0:N2}", vec.z));
        }
    }
}