using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using File = System.IO.File;
using Scripts.Static;
using Telegram.Bot.Types.Enums;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Scripts.Network
{
    public static class DataChanel
    {
        public static void WriteAndSendFile(string filePath, string value)
        {
            Debug.Log("Trying to write");

            Task.Run(async () => { await WriteAndSendFileAsync(filePath, value); });
        }


        private static async Task WriteAndSendFileAsync(string filePath, string value)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                await writer.WriteAsync(value);
            }

            //
            var name = Path.GetFileName(filePath);
        }

        public static string Get(string jsonPath)
        {
            jsonPath = Calculations.ConvertToResourceFormat(jsonPath);
            var jsonFile = UnityEngine.Resources.Load<TextAsset>(jsonPath);
            if (jsonFile != null)
                return jsonFile.text;
            Debug.LogError("Failed to load JSON file from resources: " + jsonPath);
            return "";
        }

        public static string Get(string jsonPath, ulong id)
        {
            return "";
        }

        public static IEnumerator Get(string url, RequestHeader[] headers,
            KeyValuePair<string, object> keyValuePair, Action<string> callback)
        {
            using (UnityWebRequest www = new UnityWebRequest(url, "GET"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(keyValuePair));
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                for (int i = 0; i < headers.Length; i++)
                {
                    www.SetRequestHeader(headers[i].name, headers[i].value);
                }

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    callback(www.downloadHandler.text);
                    Debug.Log("Data sent successfully: " + www.downloadHandler.text);
                }
            }
        }

        public static IEnumerator Post(string url, RequestHeader[] headers,
            KeyValuePair<string, object> keyValuePair, Action<string> callback)
        {
            using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(keyValuePair));
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                for (int i = 0; i < headers.Length; i++)
                {
                    www.SetRequestHeader(headers[i].name, headers[i].value);
                }

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    callback(www.downloadHandler.text);
                    Debug.Log("Data sent successfully: " + www.downloadHandler.text);
                }
            }
        }
    }

    public struct RequestHeader
    {
        public string name;
        public string value;

        public RequestHeader(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
    }
}