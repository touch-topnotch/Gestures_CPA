using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace CrossPlatform.Static
{

    public class Vector3Converter : JsonConverter<Vector3>
    {
        public static Vector3[] convertToVector3(string[] value)
        {
            if (value == null || value.Length == 0)
            {
                return null;
            }
            Vector3[] jArr = new Vector3[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                jArr[i] = JsonUtility.FromJson<Vector3>(value[i]);
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
                jArr[i] = JsonUtility.ToJson(value[i]);
            }

            return jArr;
        }
    

        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            
            Debug.LogWarning("ITS MY SCRIPT");
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
    }
}