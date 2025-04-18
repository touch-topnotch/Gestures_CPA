using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Authentication;
using Scripts.Static;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Networking;


namespace Scripts.Network
{
    public class CloudSaveProcessor : MonoBehaviour
    {
        private const string projectId = CustomPaths.projectId;
        private const string environmentId = CustomPaths.environmentId;
        public static CloudSaveProcessor Instance { get; private set; }

        private async void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

        }
        public static void GetAllCustomItems(Action<string> onSuccess){
            string url =
                $"https://services.api.unity.com/cloud-save/v1/data/projects/{projectId}/environments/{environmentId}/custom"; //

            WebRequests.Get(url,
                (UnityWebRequest unityWebRequest) =>
                {
                    unityWebRequest.SetRequestHeader("Authorization", "Basic " + CustomPaths.keyBase64); // it is true
                },
                (string error) => { Debug.LogError("Error: " + error); },
                onSuccess
            );
        }
        public static void SetItemToCloud(string item, string itemKey, Action<string> onSuccess)
        {
            /*Debug.Log("ITEM: " +item);
            var url =
                $"https://services.api.unity.com/cloud-save/v1/data/projects/{projectId}/environments/{environmentId}/custom/{itemKey}/items";
            WebRequests.PostJson(
                url,
                (UnityWebRequest unityWebRequest) =>
                {
                    unityWebRequest.SetRequestHeader("Authorization",
                        "Basic " + CustomPaths.keyBase64); 
                },
                item,
                (string error) => { Debug.LogError("Error: " + error); },
                onSuccess
            );*/
        }
        public static void SetItemToCloud(ItemStruct item, string itemKey, Action<string> onSuccess)
        {
            SetItemToCloud(JsonUtility.ToJson(item), itemKey,onSuccess);
        }

        public struct ItemStruct
        {
            public string key;
            public object value;

            public ItemStruct(string key, object value)
            {
                this.key = key;
                this.value = value;
            }
        }
    }
}