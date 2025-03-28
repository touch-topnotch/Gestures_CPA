using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Characters;
using Newtonsoft.Json;
using Scripts.Characters;
using Scripts.Databases;
using Scripts.Events;
using Scripts.PlayerLogic;
using Scripts.Static;
using Scripts.Tests;
using UnityEngine;

namespace Scripts.Gestures
{
    
    public class GesturesLibrary
    {
        public readonly Restrictive<string, DynamicGesture> gestures = new();
        
        private readonly PlayerData _playerData;

        private Dictionary<string, DynamicGesture> allGestures => gestures.GetOpenDict();
        public Dictionary<string, DynamicGesture> DynamicGestures => gestures.GetOpenDict();
        private Dictionary<string, Character> _characters;
        
        public GesturesLibrary(PlayerData data, Dictionary<string, Character> characters)
        {
            _playerData = data;
            _characters = characters;
           EventInitializer.Instance.onServicesInitilalised += ()=>
           {
               AddDictionary(data);
           };
        }

        private async void AddDictionary(PlayerData data)
        {
            var d = await GestureMapper.ReadDynamicGestures(_characters);
            gestures.AddDictionary(d);
            var log = "Library has initialized for player: " + data.id  +". Mapped gestures: ";
            foreach (var VARIABLE in allGestures)
            {
                log += VARIABLE.Key + ", ";
            }
            Debug.Log(log);
        }

        private static JsonCharacterProperties AddGestureToChar(string name,  HandsStruct hands,  JsonCharacterProperties jsonChar)
        {
            string dynamicName = GestureMapper.PrefixOfName(name);
            var gestures = jsonChar.Gestures;
            var hasDynamic = false;
            for (int i = 0; i < gestures.Count; i++)
            {
                if (gestures[i].Name == dynamicName)
                {
                    gestures[i] = AddToExistedGesutre(name, hands, gestures[i]);
                    hasDynamic = true;
                    break;
                }
            }

            if (!hasDynamic)
            {
                gestures.Add(CreateNewGesture(hands, name));
            }

            return new JsonCharacterProperties()
            {
                Description = jsonChar.Description,
                RootFolder = jsonChar.RootFolder,
                Gestures = gestures
                
            };
        }
        private static JsonGestureStruct AddToExistedGesutre(string name, HandsStruct hands, JsonGestureStruct jsonStruct)
        {
            int index = GestureMapper.IndexOfName(name);
            
            List<string[]> frames = jsonStruct.Frames;
            
            if (index < frames.Count)
            {
                frames[index] = GestureMapper.HandsStructToString(hands);
            }
            else
            {
                for (int i = frames.Count; i < index; i++)
                {
                    frames.Add(null);
                }
                
                frames.Add(GestureMapper.HandsStructToString(hands));
            }
            
            return new JsonGestureStruct()
            {
                Name = jsonStruct.Name,
                Frames = frames,
                Type = jsonStruct.Type
            };
        }
        private static JsonGestureStruct CreateNewGesture(HandsStruct hands, string name)
        {
            int index = GestureMapper.IndexOfName(name);
            List<string[]> frames = new List<string[]>();
            if (index > 0)
            {
                for (int i = 0; i < index - 1; i++)
                {
                    frames.Add(null);
                }
            }
            frames.Add(GestureMapper.HandsStructToString(hands));
            for (int i = 0; i < frames[0].Length; i++)
            {
                Debug.Log(frames[0][i]);
            }
            var t = new JsonGestureStruct()
            {
                Name = GestureMapper.PrefixOfName(name),
                Frames = frames,
                Type = (int)GestureType.Weapon
            };
            return t;
        }
        public async Task RecordFrame(HandsStruct hands, string name, string characterName)
        {
            var dictionary = await CharacterMapper.GetAvailableCharactersStruct(new HashSet<string>(){characterName});
            var jsonChar = (dictionary == null || dictionary.Keys.Count == 0)
                ? new JsonCharacterProperties()
                {
                    Description = characterName + " is cool!",
                    RootFolder = "Resources/Characters/" + characterName,
                    Gestures = new List<JsonGestureStruct>()
                    {
                        CreateNewGesture(hands, name)
                    }
                }
                : AddGestureToChar(name, hands, dictionary[characterName]);
            CharacterMapper.SendCharacterStruct(new JsonCharacterStruct(){key = characterName,value = jsonChar});
        }
        
        public bool TryGetDynamicGesture(string gesture, out DynamicGesture frame)
        {
            return DynamicGestures.TryGetValue(gesture, out frame) ||
                   DynamicGestures.TryGetValue(GestureMapper.PrefixOfName(gesture), out frame);
        }
        public bool TryGetGestureFrame(string name, out GestureFrame frame)
        {
            if (DynamicGestures.ContainsKey(GestureMapper.PrefixOfName(name)))
            {
                frame = DynamicGestures[GestureMapper.PrefixOfName(name)].frames[GestureMapper.IndexOfName(name)];
                return true;
            }
            frame = null;
            return false;
        }

        private void SetGestureFrame(GestureFrame frame)
        {
            if (DynamicGestures.ContainsKey(frame.baseName))
            {
                if (DynamicGestures[frame.baseName].frames.Count <= GestureMapper.IndexOfName(frame.name))
                {
                    for(int i = DynamicGestures[frame.baseName].frames.Count; i <= GestureMapper.IndexOfName(frame.name); i++)
                    {
                        DynamicGestures[frame.baseName].frames.Add(null);
                    }
                }
                DynamicGestures[frame.baseName].frames[GestureMapper.IndexOfName(frame.name)] = frame;
            }
            else
            { //gestures
                throw new Exception("There is no gestures with name " + frame);
            }
        }

        public bool ContainsFrame(string frame) => (DynamicGestures.ContainsKey(GestureMapper.PrefixOfName(frame))) &&
                                                   DynamicGestures[GestureMapper.PrefixOfName(frame)].HasFrame(frame);

    }
}