using System.Collections.Generic;
using System.IO;
using Authentication;
using Characters;
using Newtonsoft.Json;
using Scripts.Databases;
using Scripts.Network;
using Scripts.Static;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

public class test : MonoBehaviour
{
    //  [Button("Test send to cloud save")]
    //   public void Start()
    // {
    //     CloudSaveProcessor.SetItemToCloud(File.ReadAllText(Application.dataPath + "/Resources/Database/test.json"),
    //         "characters", (e) =>
    //         {
    //             Debug.Log("bebra " + e);
    //         });
    // }

    [Button("Get All")]
    public async void Start()
    {
        await UnityServices.InitializeAsync();

        var chars = await CloudSaveService.Instance.Data.Custom.LoadAllAsync("characters");
        Debug.Log(JsonConvert.SerializeObject(chars));
    }

    void PrintRecursive(object obj)
    {
        if (obj == null)
        {
            return;
        }

        var type = obj.GetType();
        if (!type.IsClass || type == typeof(string))
        {
            Debug.Log(obj);
        }
        else
        {
            foreach (var propertyInfo in type.GetProperties())
            {
                var value = propertyInfo.GetValue(obj, null);
                if (value != null)
                {
                    Debug.Log($"Key: {propertyInfo.Name}, Value: {value}");
                }

                PrintRecursive(value);
            }
        }
    }

    [Button("Get by key")]
    [BoxGroup("By key")]
    public void GetByKey()
    {
        //  Debug.Log(CloudSaveService.Instance.Data.Custom.QueryAsy);
    }
}