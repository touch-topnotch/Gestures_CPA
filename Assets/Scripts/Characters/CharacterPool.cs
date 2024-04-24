using System;
using System.Collections.Generic;
using Characters;
using Gesture_Editor_SDK.EditorAttributes.InspectorButtonAttribute;
using Scripts.Design;
using Scripts.HandsLogic;
using Scripts.PlayerLogic;
using Scripts.Static;
using Scripts.Systems;
using Scripts.Tests;
using Scripts.Weapons;
using Sirenix.OdinInspector;
using Unity.Netcode;
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
        public List<CharacterData> characterConfigs;
        public Dictionary<string, Character> charactersDict { get; private set; }
        
        [BoxGroup("Object pool")]
        [SerializeField] private List<MaterialPair> _materials = new List<MaterialPair>();
        
        [SerializeField] private Hands _hands;

        [Header("Events")] public UnityEvent<string> characterChangedEvent = new();

        public Character currentCharacter => charactersDict.ContainsKey(_currentCharacterName)
            ? charactersDict[_currentCharacterName]
            : null;

            #region Unity Inspectors tools
#if UNITY_EDITOR
        [Button]
        public void UpdateCharacter()
        {
            SetCharacter(_currentCharacterName);
        }
        [InspectorButton("Update Characters", space: 4)]
        
        public void UpdateCharacters()
        {
            RefreshDictionary();
            // for all folders in the asset path
            // find object with name, which contains "_character", spawn it and add to the pool
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(CustomPaths.Characters);
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
                    if (charactersDict.ContainsKey(charName))
                    {
                        charactersDict[charName] = character;
                    }
                    else
                    {
                        charactersDict.Add(charName, character);
                    }
                    // charactersDict.SmartAdd(charName,character);
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

            //character.SetSource(source);

          //  character.GenerateAvatars();
            DestroyImmediate(characterPrefab);
        }
#endif
        #endregion
        
        public void SpawnCharacters()
        {
            charactersDict = new();
            foreach (var VARIABLE in characterConfigs)
            {
                charactersDict.Add(VARIABLE.characterName, InitialiseCharacter(VARIABLE));
            }
        }

        
        public List<KeyValuePair<string, List<ulong>>> SpawnWeapons()
        {
            var all_weapons = new List<KeyValuePair<string, List<ulong>>>();
            foreach (var VARIABLE in characterConfigs)
            {
                var spawns = charactersDict[VARIABLE.characterName].SpawnWeapons(VARIABLE.weapons);
                all_weapons.Add(new(VARIABLE.characterName, spawns));
            }

            return all_weapons;
        }

        public void SetWeapons(List<KeyValuePair<string, List<ulong>>> weapons)
        {
            foreach (var char_weapons in weapons)
            {
                charactersDict[char_weapons.Key].SetWeapons(char_weapons.Value);
                foreach (var VARIABLE in char_weapons.Value)
                {
                    NetworkManager.Singleton.SpawnManager.SpawnedObjects[VARIABLE].GetComponent<Weapon>().Initialize(transform.parent.GetComponent<PlayerData>());
                }
            }

        }
        

    
        private void Start()
        { 
            characterChangedEvent.AddListener(LogCharacter);
        }
        private Character InitialiseCharacter(CharacterData data)
        {
            var charInstance = new GameObject();
            charInstance.transform.SetParent(this.transform, false);
            charInstance.name = data.characterName;
            var character = charInstance.AddComponent<Character>();
            character.SpawnCharacters(data);
            return character;
        }
        
        public void SetMaterialPair(MaterialPair pair)
        {
            _hands.HandMaterialPair = pair;
            if(currentCharacter != null)
                currentCharacter.ChangeMaterials(_hands.HandMaterialPair);
        }

        public void SetAvatarType(AvatarType type)
        {
            _currentType = type;
            if(currentCharacter != null)
                currentCharacter.ChangeAvatarType(type, _hands);
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
            
            characterChangedEvent?.Invoke(_currentCharacterName);
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

        
        public void LogCharacter(string name)
        {
            Debug.Log("Character " + transform.parent.name + " changed on "+ name + ", "+ _currentCharacterName);
        }
        private void RefreshDictionary()
        {
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

            if (_hands && currentCharacter)
            {
                currentCharacter.RefreshAvatars();
                currentCharacter.ChangeMaterials(_hands.HandMaterialPair, _currentType);
            }
            
        }
    }
}
