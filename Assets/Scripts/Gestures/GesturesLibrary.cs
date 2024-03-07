using System;
using System.Collections.Generic;
using Characters;
using Scripts.Characters;
using Scripts.Databases;
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
            
            gestures.AddDictionary(GestureMapper.ReadDynamicGestures(characters));
            
            var log = "Library has initialized for player: " + data.id +". Mapped gestures: ";
            foreach (var VARIABLE in allGestures)
            {
                log += VARIABLE.Key + ", ";
            }
            Debug.Log(log);
        }

        private static JsonCharacterStruct AddGestureToChar(string name,  HandsStruct hands,  JsonCharacterStruct jsonChar)
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

            return new JsonCharacterStruct()
            {
                Name = jsonChar.Name,
                Description = jsonChar.Description,
                RootFolder = jsonChar.RootFolder,
                Gestures = gestures,
            };
        }
        private static JsonGestureStruct AddToExistedGesutre(string name, HandsStruct hands, JsonGestureStruct jsonStruct)
        {
            int index = GestureMapper.IndexOfName(name);
            
            List<List<string>> frames = jsonStruct.Frames;
            
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
            List<List<string>> frames = new List<List<string>>();
            if (index > 0)
            {
                for (int i = 0; i < index - 1; i++)
                {
                    frames.Add(null);
                }
            }
            frames.Add(GestureMapper.HandsStructToString(hands));
            return new JsonGestureStruct()
            {
                Name = GestureMapper.PrefixOfName(name),
                Frames = frames,
                Type = (int)GestureType.Weapon
            };
        }
        public void RecordFrame(HandsStruct hands, string name, string characterName)
        {

            Debug.Log("Recording frame: " +name);
            var jsonCharacterStructs = CharacterMapper.GetCharacterStruct();
            if (jsonCharacterStructs == null)
            {
                jsonCharacterStructs = new List<JsonCharacterStruct>();
            }
            var charId = -1;
         
            for (int i = 0; i < jsonCharacterStructs.Count; i++)
            {
                if (jsonCharacterStructs[i].Name == characterName)
                {
                    charId = i;
                    break;
                }
            }

            if (charId == -1)
            {
                jsonCharacterStructs.Add(new JsonCharacterStruct()
                {
                    Name = characterName,
                    Description = characterName + " is cool!",
                    RootFolder = "Resources/Characters/"+ characterName,
                    Gestures = new List<JsonGestureStruct>(){CreateNewGesture(hands, name)}
                });
            }
            else // there is a character
            {
                jsonCharacterStructs[charId] = AddGestureToChar(name, hands, jsonCharacterStructs[charId]);
            }
            CharacterMapper.SendCharacterStruct(jsonCharacterStructs);
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