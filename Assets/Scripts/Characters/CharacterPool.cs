using System;
using System.Collections.Generic;
using Gesture_Editor_SDK.EditorAttributes.InspectorButtonAttribute;
using Gesture_Editor_SDK.ReadOnly;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Characters
{
    
    public class CharacterPool: MonoBehaviour
    {
        [SerializeField] [ReadOnlyInInspector] 
        private List<Character> characters = new();

        [Header("Current Character")] [SerializeField]
        private string _currentName = "";
        
        [Header("Properties")]
        [SerializeField] private string assetPath;
        private Dictionary<string, Character> charactersDict = new ();
        [Header("Events")]
        public UnityEvent<string> OnCharacterChanged;
        

        public string CurrentName
        {
            get => _currentName;
            
            set
            {
                if (value == _currentName)
                    return;
                
              
                if(charactersDict.ContainsKey(value))
                {
                    _currentName = value;
                }
                else
                {
                    _currentName = characters[0].name;
                    Debug.LogWarning("There is no character with name: " + CurrentName);
                }
                
                OnCharacterChanged?.Invoke(_currentName);
            }
        }

        public void Log(string onChE)
        {
            Debug.Log(onChE + "changed");
        }

        public Character GetCharacter()
        {
            if (characters.Count == 0 || charactersDict.Keys.Count == 0)
                RefreshDictionary();
            return charactersDict[CurrentName];
        }


#if UNITY_EDITOR
        [InspectorButton("Update Characters", space:4)]
        public void UpdateCharacters()
        {
            RefreshDictionary();
            // for all folders in the asset path
            // find object with name, which contains "_character", spawn it and add to the pool
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(assetPath);
            System.IO.DirectoryInfo[] subDirs = dir.GetDirectories();
            foreach (var subDir in subDirs)
            {
                string[] files = System.IO.Directory.GetFiles(subDir.FullName);
                foreach (var file in files)
                {

                    if (!file.Contains("_Character") || file.Contains(".meta"))
                    {
                        //Debug.LogWarning("File " + file + " doesn't contain _Character");
                        continue;
                    }
                    // if file path contains .../Resources/other_path, change it to other_path
                    var res_path = file;
                    if (file.Contains("Resources"))
                    {
                        res_path = file.Substring(file.IndexOf("Resources")+10);
                        res_path = res_path.Replace(".prefab", "");
                    }
                    var resourceObject = Resources.Load(res_path) as GameObject;

                    if (!resourceObject)
                    {
                        Debug.LogWarning("Can't load resource Object by path " + res_path);
                        continue;
                    }

                    if (!resourceObject.TryGetComponent<Character>(out var resourceCharacter))
                    {
                        Debug.LogWarning("Object doesn't contain Character component");
                        continue;
                    }

                    var charName = resourceCharacter.name;
                    if (charactersDict.ContainsKey(charName))
                    {
                        continue;
                    }
                    
                    foreach (var VARIABLE in charactersDict.Keys)
                    {
                        Debug.Log(VARIABLE);
                    }
                    Debug.Log(charactersDict.Keys + charName);
                
                    
                    var spawnedObject = PrefabUtility.InstantiatePrefab(resourceObject) as GameObject;
            
                    if (!spawnedObject)
                    {
                        Debug.LogWarning("Can't spawn resource Object");
                        continue;
                    }

                    spawnedObject.transform.SetParent(transform);
                    
                    Debug.Log("Trying to add character " + charName + "... ");
                    var character = spawnedObject.GetComponent<Character>();
                    charactersDict.Add(charName, character);
                    characters.Add(character);
                    Debug.Log("Character " + charName + " added to Object Pool");
                }
            }
            
        }
        #endif
        private void RefreshDictionary()
        {
            characters = new List<Character>();
            charactersDict = new Dictionary<string, Character>();
            var res = "";
            for (int i = 0; i < transform.childCount; i++)
            {
                
                if (transform.GetChild(i).TryGetComponent<Character>(out var character))
                {
                    
                    if (!charactersDict.ContainsKey(character.name))
                    {
                        characters.Add(character);
                        charactersDict.Add(character.name, character);
                        res += res == "" ? character.name : ", " + character.name;
                    }
                }
            }
            Debug.Log(res + " have been added to Character Pool");
            if (characters.Count > 0)
            {
                _currentName = characters[0].name;
                ReactivateCharacters();
            }
            else
            {
                Debug.Log("Cannot find any characters in pool");
            }
        }

        private void ReactivateCharacters()
        {
            foreach (var VARIABLE in charactersDict.Keys)
            {
                charactersDict[VARIABLE].gameObject.SetActive(VARIABLE == CurrentName);
            }
        }

        private void Start()
        {
            RefreshDictionary();
        }
    }
}