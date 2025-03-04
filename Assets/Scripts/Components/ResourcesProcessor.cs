using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Components
{
    public enum ResourceType
    {
        Sounds,
        VFX,
        Models,
        Other
    }
    public abstract class ResourcesProcessor<T> : SerializedMonoBehaviour
    where T: Object
    {
        [BoxGroup("Resources")][SerializeField]
        protected Dictionary<string, T> itemsDict = new();
        [BoxGroup("Resources")][SerializeField]
        protected Dictionary<string, List<T>> listOfItemsDict = new();
        
        private T LoadResource(string resourceName)
        {
            if (itemsDict.ContainsKey(resourceName))
                return itemsDict[resourceName];
            Debug.LogWarning("Resource " + resourceName + " not found in " + GetType().Name);
            return default;
        }
        
        private T LoadRandomResource(string listName)
        {
            if (listOfItemsDict.ContainsKey(listName))
                return listOfItemsDict[listName][Random.Range(0, listOfItemsDict[listName].Count)];
            Debug.LogWarning("List " + listName + " not found in " + GetType().Name);
            return default;
        }

      
        
        private T LoadSequencedResource(string listName, int id)
        {
            if (!listOfItemsDict.ContainsKey(listName))
            {
                Debug.LogWarning("List " + listName + " not found in " + GetType().Name);
                return default;
            }

            if (listOfItemsDict[listName].Count <= id)
            {
                Debug.LogWarning("Index out of range");
                return default;
            }

            return listOfItemsDict[listName][id];
        }

        protected abstract void ManipulateResource(T resource);
        
        public virtual void ActivateResource(string itemName, Action<T> manipulation = null, bool mightBeNull = true)
        {
            if(manipulation == null)
                ManipulateResource(LoadResource(itemName));
            else
            {
                manipulation(LoadResource(itemName));
            }
        }

        public virtual void ActivateRandomResource(string listName,Action<T> manipulation = null, bool mightBeNull = true)
        {
            if(manipulation == null)
                ManipulateResource(LoadRandomResource(listName));
            else
            {
                manipulation(LoadRandomResource(listName));
            }
        }  
        public virtual void ActivateSequencedResource(string listName, int id, Action<T> manipulation = null, bool mightBeNull = true)
        {
            if(manipulation == null)
                ManipulateResource(LoadSequencedResource(listName, id));
            else
            {
                manipulation(LoadSequencedResource(listName, id));
            }
        }

      


        #region Authomatization
    
        #if UNITY_EDITOR
        [BoxGroup("Add missing resources")] [FolderPath] [SerializeField]
        protected string _folderPath;
        [BoxGroup("Add missing resources")]
        [Button("Add missing resources")]
        protected virtual void AddMissingResources()
        {
            // go by folder path and
            // add all audio clips in folder to tempClips, if it contains S_ prefix
            // if clip contains S_R_ prefix, add it to _audioClipLists with name S_name_of_clip_without_prefix
            // else add it to _audioClips with name S_name_of_clip_without_prefix
            if (Directory.Exists(_folderPath))
            {
                var files = Directory.GetFiles(_folderPath);
                foreach (var file in files)
                {
                    if (file.Contains(".meta")) continue;
                    var item = AssetDatabase.LoadAssetAtPath(file, typeof(T)) as T;
                    
                    if(!item)
                           continue;
                    
                    if (item.name.Contains("_R_"))
                    {
                        var name = item.name.Split("_")[2];
                        if (listOfItemsDict.ContainsKey(name))
                        {
                            listOfItemsDict[name].Add(item);
                        }
                        else
                        {
                            listOfItemsDict.Add(name, new List<T> { item });
                        }
                    }
                    else
                    {
                        if (item.name.Contains("Frame"))
                        {
                            var name = "Frame";
                            if (int.TryParse(item.name.Split("_")[4], out int index))
                            {
                                if (!listOfItemsDict.ContainsKey(name))
                                {
                                    listOfItemsDict.Add(name, new List<T>());
                                }
    
                                if (listOfItemsDict[name].Count > index)
                                {
                                    listOfItemsDict[name][index] = item;
                                }
                                else
                                {
                                    
                                    for (int i = listOfItemsDict[name].Count; i < index; i++)
                                    {
                                        listOfItemsDict[name].Add(item);
                                    }
    
                                    //_audioClipLists[name]
                                    listOfItemsDict[name].Add(item);
                                }
                            }
                            else
                            {
                                Debug.Log("Index of frame is not exist! File: " + item.name);
                            }
                        }
                        else
                        {
                            var name = item.name.Split("_")[2];
                            if (itemsDict.ContainsKey(name))
                                itemsDict[name] = item;
                            else
                                itemsDict.Add(name, item);
                        }
                    }
                    
                }
            }
        }
    
    
        [BoxGroup("Sorting")][SerializeField] [FolderPath]
        private string _telegramResourcesFolderPath;
    
        // [BoxGroup("Sorting")][SerializeField]
        // private ResourceType _type;
        
        [BoxGroup("Sorting")]
        [Button("Sort Telegram resources to folders")]
        
        private void SortResources()
        {
            if (!_telegramResourcesFolderPath.Contains("Telegram"))
            {
                Debug.LogError("Wrong folder selected. Please select folder, which name contains 'Telegram'!");
                return;
            }
                
            Debug.Log("Importing resources to folders ...");
            // search all files with S_ in folder , find name = fileName.Split("_")[1] and move them to folder Assets/Sounds/name
            if (Directory.Exists(_telegramResourcesFolderPath))
            {
                var files = Directory.GetFiles(_telegramResourcesFolderPath);
                foreach (var file in files)
                {
                    if (file.Contains(".meta"))
                        AssetDatabase.DeleteAsset(file);
    
                    var item = (T)AssetDatabase.LoadAssetAtPath(file, typeof(T));
                    if (!item || item.name.Split("_").Length < 3)
                        continue;
                    
                    var name = item.name.Split("_")[2];
    
                    var folder = "Assets/Resources/Weapons/" + name + "/" + GetTypeByPrefix(item.name.Split("_")[0]);
    
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
    
                    string fullPath = folder + "/" + item.name + Path.GetExtension(file);
                    AssetDatabase.MoveAsset(file, fullPath);
                    
                    Debug.Log("Imported " +fullPath);
                    
                }
            }
        }
    
        protected void PoolObject(string list, int id = -1)
        {
            
            GameObject item;
            if (id == -1)
            {
                if (!itemsDict.ContainsKey(list))
                { 
                    Debug.LogWarning("Can't pool object, because items dict not contains " + list);
                    return;
                }
                   
                if (itemsDict[list] is not GameObject)
                {
                    Debug.LogWarning("Can't pool object, because item is not a GameObject");
                    return;
                }
    
                if (GameObject.Find(itemsDict[list].name))
                {
                    Debug.LogWarning("This object already exists");
                    return;
                }
                itemsDict[list] = PrefabUtility.InstantiatePrefab(itemsDict[list] as GameObject, this.transform) as T;
            }
            else
            {
                if (!listOfItemsDict.ContainsKey(list))
                { 
                    Debug.LogWarning("Can't pool object, because list of items dict not contains " + list);
                    return;
                }
    
                if (listOfItemsDict[list].Count <= id)
                {
                    Debug.LogWarning("Can't pool object, because index is more than count of items");
                    return;
                }
                if (listOfItemsDict[list][id] is not GameObject)
                {
                    Debug.LogWarning("Can't pool object, because item is not a GameObject");
                    return;
                }
                if (GameObject.Find(listOfItemsDict[list][id].name))
                {
                    Debug.LogWarning("This object already exists");
                    return;
                }
                listOfItemsDict[list][id] = PrefabUtility.InstantiatePrefab(listOfItemsDict[list][id] as GameObject, this.transform) as T;
            }
        }
    
        protected void PoolAllObjects()
        {
            foreach (var VARIABLE in listOfItemsDict.Keys)
            {
                for (int i = 0; i < listOfItemsDict[VARIABLE].Count; i++)
                {
                    PoolObject(VARIABLE, i);
                }
            }
    
            foreach (var VARIABLE in itemsDict.Keys)
            {
                PoolObject(VARIABLE);
            }
        }
    #endif

        #endregion

        public void ManipulateOfAllObjects(Action<T> manipulation)
        {
            foreach (var VARIABLE in itemsDict.Values)
            {
                manipulation(VARIABLE);
            }

            foreach (var VARIABLE in listOfItemsDict.Values)
            {
                foreach (var item in VARIABLE)
                {
                    manipulation(item);
                }
            }
        }
        private static ResourceType GetTypeByPrefix(string prefix)
        {
            switch (prefix)
            {
                case "S":
                    return ResourceType.Sounds;
                case "V":
                    return ResourceType.VFX;
                case "M":
                    return ResourceType.Models;
                default:
                    return ResourceType.Other;
            }
        }
        private static string GetPrefixByType(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Sounds:
                    return "S_";
                case ResourceType.VFX:
                    return "V_";
                case ResourceType.Models:
                    return "M_";
                default:
                    return "O_";
            }
        }
    }

}