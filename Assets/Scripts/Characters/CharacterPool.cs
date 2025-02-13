using System;
using System.Collections.Generic;
using System.Linq;
using Gesture_Editor_SDK.EditorAttributes.InspectorButtonAttribute;
using Gesture_Editor_SDK.ReadOnly;
using Scripts.Design;
using Scripts.HandsLogic;
using Scripts.PlayerLogic;
using Scripts.Tests;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Characters
{

    public class CharacterPool : MonoBehaviour
    {
        [Header("Properties")]
        [SerializeField] private string _currentCharacterName = "";

        [EnumToggleButtons]
        [SerializeField] private AvatarType _currentType;
        
        [BoxGroup("Object pool")]
        [SerializeField] private CustomDictionary<string, Character> charactersDict = new();
        
        [BoxGroup("Object pool")]
        [SerializeField] private List<MaterialPair> _materials = new List<MaterialPair>();
        
        [FolderPath]
        [SerializeField] private string _assetPath;
        
        [SerializeField] private Hands _hands;

        [Header("Events")] public UnityEvent<string> OnCharacterChanged = new();

        public void LogCharacter(string name)
        {
            Debug.Log("Character " + transform.parent.name + " changed on "+ name + ", "+ _currentCharacterName);
        }
        private void OnValidate()
        {
            SetCharacter(_currentCharacterName);
            SetAvatarType(_currentType);
        }

        public void SetMaterialPair(MaterialPair pair)
        {
            _hands.HandMaterialPair = pair;
            CurrentCharacter?.ChangeMaterials(_hands.HandMaterialPair);
        }

        public void SetAvatarType(AvatarType type)
        {
            _currentType = type;
            CurrentCharacter?.ChangeAvatarType(type, _hands);
        }

        public void SetCharacter(string name)
        {
            if (name == _currentCharacterName)
            {
                ReactivateCharacters();
                return;
            }
            
            if (charactersDict.ContainsKey(name))
            {
                _currentCharacterName = name;
            }
            else
            {
                Debug.LogWarning("There is no character with name: " + _currentCharacterName);
                return;
            }
            ReactivateCharacters();

            OnCharacterChanged?.Invoke(_currentCharacterName);
        }

        public void SetCharAndId(int id, string name)
        {
            _hands.HandMaterialPair = _materials[id% _materials.Count];
            SetCharacter(name);
        }

        public void SetMaterialId(int id)
        {
            _hands.HandMaterialPair = _materials[id% _materials.Count];
        }

        public Character CurrentCharacter => charactersDict.ContainsKey(_currentCharacterName) ? charactersDict[_currentCharacterName] : null;
        
        
#if UNITY_EDITOR
        [InspectorButton("Update Characters", space: 4)]
        public void UpdateCharacters()
        {
            RefreshDictionary();
            // for all folders in the asset path
            // find object with name, which contains "_character", spawn it and add to the pool
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(_assetPath);
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
                        res_path = file.Substring(file.IndexOf("Resources") + 10);
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
                
                    charactersDict.SmartAdd(charName,character);
                    Debug.Log("Character " + charName + " added to Object Pool");
                }
            }
        }

        [InspectorButton("Generate new Character (by current name)")]
        public void GenerateCharacter()
        {
            GameObject characterPrefab = new();
            var character = characterPrefab.AddComponent<Character>();
            var modelPath = $"Assets/Models/Characters/{_currentCharacterName}_Model.prefab";
            var characterPath = $"Assets/Resources/Characters/{_currentCharacterName}/{_currentCharacterName}.prefab";
            var source = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            if (source == null)
            {
                Debug.LogWarning(
                    $"There is no {_currentCharacterName} by path: Assets/Models/Characters/{_currentCharacterName}_Model.prefab");
                return;
            }

            character.SetSource(source);

            character.GenerateAvatars();
            DestroyImmediate(characterPrefab);
        }
#endif
        private void RefreshDictionary()
        {
            foreach (var VARIABLE in transform.GetComponentsInChildren<Character>())
            {
                if (!string.IsNullOrEmpty(VARIABLE.name) && !charactersDict.ContainsKey(VARIABLE.name))
                    charactersDict.Add(VARIABLE.name, VARIABLE);
            }
        
            if (charactersDict.Count > 0)
            {
                ReactivateCharacters();
            }
            else
            {
                Debug.Log("Cannot find any characters in pool");
            }
        }

        private void ReactivateCharacters()
        {
            foreach (var VARIABLE in charactersDict)
            {
                if(VARIABLE.Value)
                    VARIABLE.Value.gameObject.SetActive(VARIABLE.Key == _currentCharacterName);
            }

            if (_hands && CurrentCharacter)
                CurrentCharacter.ChangeMaterials(_hands.HandMaterialPair, _currentType);
        }

        private void Start()
        {
            OnCharacterChanged.AddListener(LogCharacter);
            //ReactivateCharacters();
        }
   
    }
}