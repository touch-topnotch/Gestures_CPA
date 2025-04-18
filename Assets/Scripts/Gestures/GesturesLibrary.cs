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
using Scripts.Systems;
using Scripts.Tests;
using Unity.Services.CloudSave;
using UnityEngine;

namespace Scripts.Gestures
{
    public enum GestureCollections
    {
        system,
        supportive,
        characters
    }

    public class GesturesLibrary
    {
        public Dictionary<string, GestureFrame> allAvailableFrames = new();
        public Dictionary<string, DynamicGesture> characterGestures;
        public Dictionary<string, GestureFrame> systemGestures = new();
        public Dictionary<string, GestureFrame> supportiveGestures = new();

        public event Action onLibraryInitialized;
        private readonly RestrictiveDictionary<string, DynamicGesture> allCharacterGestures = new();


        private CharacterPool _characterPool;

        public GesturesLibrary(CharacterPool characterPool)
        {
            _characterPool = characterPool;
            // _characterPool.characterChangedEvent.AddListener(OnCharacterChanged);
            if (!EventInitializer.Instance.isInitialized)
            {
                EventInitializer.Instance.onServicesInitilalised += () => { AddDictionary(); };
            }
            else
            {
                AddDictionary();
            }
        }

        private async void AddDictionary()
        {
#if UNITY_EDITOR
            allCharacterGestures.AddDictionary(
                await GestureMapper.ReadCharacterGestures(_characterPool.charactersDict));

            characterGestures = allCharacterGestures.openDict;
            systemGestures = await GestureMapper.ReadGestureFrames("system");
            supportiveGestures = await GestureMapper.ReadGestureFrames("supportive");
            foreach (var dgesture in characterGestures)
            {
                foreach (var frame in dgesture.Value.frames)
                {
                    allAvailableFrames.Add(frame.name, frame);
                }
            }

            foreach (var frame in systemGestures)
            {
                allAvailableFrames.Add(frame.Key, frame.Value);
            }

            foreach (var frame in supportiveGestures)
            {
                allAvailableFrames.Add(frame.Key, frame.Value);
            }

            var log = $"Library has initialized! .\n"
                      + $"   All Parsed Gestures: {Debugger.dictionaryToString(allCharacterGestures.openDict, false, true)}"
                      + $"\n   Character Gestures (Now without limitations): {Debugger.dictionaryToString(characterGestures, false, true)}"
                      + $"\n   System Gestures: {Debugger.dictionaryToString(systemGestures, false, true)}"
                      + $"\n   Supportive Gestures: {Debugger.dictionaryToString(supportiveGestures, false, true)}";

            Debug.Log(log);
            onLibraryInitialized?.Invoke();
#endif
        }

        private static JsonCharacterProperties AddGestureToChar(string name, HandsStruct hands,
            JsonCharacterProperties jsonChar)
        {
            string dynamicName = GestureMapper.PrefixOfName(name);
            var gestures = jsonChar.Gestures;
            var hasDynamic = false;
            for (int i = 0; i < gestures.Count; i++)
            {
                if (gestures[i].key == dynamicName)
                {
                    gestures[i] = AddToExistedGesture(name, hands, gestures[i]);
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

        private static JsonGestureStruct AddToExistedGesture(string name, HandsStruct hands,
            JsonGestureStruct jsonStruct)
        {
            int index = GestureMapper.IndexOfName(name);

            List<string[]> frames = jsonStruct.value.Frames;

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
                key = jsonStruct.key,
                value = new JsonGestureProperty()
                {
                    Frames = frames,
                    Type = jsonStruct.value.Type
                }
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
                key = GestureMapper.PrefixOfName(name),
                value = new JsonGestureProperty()
                {
                    Frames = frames,
                    Type = (int)GestureType.Weapon
                }
            };
            return t;
        }

        public async Task RecordFrame(HandsStruct hands, string name, GestureCollections collection,
            string characterName = "")
        {
            var frame = new GestureFrame(name, hands);
            if (collection == GestureCollections.system)
            {
                if (systemGestures.ContainsKey(name))
                    systemGestures[name] = frame;
                else
                    systemGestures.Add(name, frame);
                GestureMapper.SendGestureFrame(collection.ToString(), frame);
                return;
            }

            if (collection == GestureCollections.supportive)
            {
                if (supportiveGestures.ContainsKey(name))
                    supportiveGestures[name] = frame;
                else
                    supportiveGestures.Add(name, frame);
                GestureMapper.SendGestureFrame(collection.ToString(), frame);
                return;
            }

            if (collection == GestureCollections.characters)
            {
                var dictionary =
                    await CharacterMapper.GetAvailableCharactersStruct(new HashSet<string>() { characterName });
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
                CharacterMapper.SendCharacterStruct(new JsonCharacterStruct()
                    { key = characterName, value = jsonChar });
            }
        }

        public bool TryGetDynamicGesture(string gesture, out DynamicGesture frame)
        {
            return characterGestures.TryGetValue(gesture, out frame) ||
                   characterGestures.TryGetValue(GestureMapper.PrefixOfName(gesture), out frame);
        }

        public bool TryGetGestureFrame(string name, out GestureFrame frame)
        {
            if (characterGestures.ContainsKey(GestureMapper.PrefixOfName(name)))
            {
                frame = characterGestures[GestureMapper.PrefixOfName(name)].frames[GestureMapper.IndexOfName(name)];
                return true;
            }

            frame = null;
            return false;
        }

        public void SetGestureFrame(GestureFrame frame, string key)
        {
            switch (key)
            {
                case "system":
                    if (systemGestures.ContainsKey(frame.name))
                        systemGestures[frame.name] = frame;
                    else
                        systemGestures.Add(frame.name, frame);
                    break;
                case "supportive":
                    if (supportiveGestures.ContainsKey(frame.name))
                        supportiveGestures[frame.name] = frame;
                    else
                        supportiveGestures.Add(frame.name, frame);
                    break;
                case "characters":
                    throw new NotImplementedException();
                    break;
            }
        }
    }
}