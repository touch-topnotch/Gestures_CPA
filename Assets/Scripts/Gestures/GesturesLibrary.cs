using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Characters;
using Scripts.Characters;
using Scripts.Databases;
using Scripts.Events;
using Scripts.Static;
using Scripts.Systems;
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
        public Dictionary<string, FrameData> allAvailableFrames = new();
        public Dictionary<string, DynamicGesture> characterGestures;
        public Dictionary<string, FrameData> systemGestures = new();
        public Dictionary<string, FrameData> supportiveGestures = new();

        public event Action onLibraryInitialized;
        private readonly RestrictiveDictionary<string, DynamicGesture> allCharacterGestures = new();


        private CharacterPool _characterPool;

        FrameData this[string name]
        {
            get
            {
                if (allAvailableFrames.ContainsKey(name))
                {
                    return allAvailableFrames[name];
                }
                else
                    return null;
            }
        }

        public GesturesLibrary(CharacterPool characterPool)
        {
            _characterPool = characterPool;
            // _characterPool.characterChangedEvent.AddListener(OnCharacterChanged);
            Debug.Log("Trying to invoke");
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
            allCharacterGestures.AddDictionary(
                await GestureMapper.ReadCharacterGestures(_characterPool.charactersDict));
            characterGestures = allCharacterGestures.openDict;
            systemGestures = await GestureMapper.ReadFrameDatas("system");
            supportiveGestures = await GestureMapper.ReadFrameDatas("supportive");
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
        }

        private static JsonCharacterProperties AddFrameToChar(FrameData frameData,
            JsonCharacterProperties jsonChar)
        {
            string dynamicName = GestureMapper.PrefixOfName(frameData.name);
            var gestures = jsonChar.Gestures;
            var hasDynamic = false;
            for (int i = 0; i < gestures.Count; i++)
            {
                if (gestures[i].key == dynamicName)
                {
                    gestures[i] = AddToExistedGesture(frameData.name, frameData, gestures[i]);
                    hasDynamic = true;
                    break;
                }
            }

            if (!hasDynamic)
            {
                gestures.Add(CreateNewFrame(frameData));
            }

            return new JsonCharacterProperties()
            {
                Description = jsonChar.Description,
                RootFolder = jsonChar.RootFolder,
                Gestures = gestures
            };
        }

        private static JsonFrameStruct AddToExistedGesture(string name, FrameData frameData,
            JsonFrameStruct jsonStruct)
        {
            int index = GestureMapper.IndexOfName(name);

            List<string[]> frames = jsonStruct.value.Frames;

            if (index < frames.Count)
            {
                frames[index] = GestureMapper.FrameDataToString(frameData);
            }
            else
            {
                for (int i = frames.Count; i < index; i++)
                {
                    frames.Add(null);
                }

                frames.Add(GestureMapper.FrameDataToString(frameData));
            }

            return new JsonFrameStruct()
            {
                key = jsonStruct.key,
                value = new JsonFrameProperty()
                {
                    Frames = frames,
                    Type = jsonStruct.value.Type
                }
            };
        }

        private static JsonFrameStruct CreateNewFrame(FrameData frameData)
        {
            int index = GestureMapper.IndexOfName(frameData.name);
            List<string[]> frames = new List<string[]>();
            if (index > 0)
            {
                for (int i = 0; i < index - 1; i++)
                {
                    frames.Add(null);
                }
            }

            frames.Add(GestureMapper.FrameDataToString(frameData));
            for (int i = 0; i < frames[0].Length; i++)
            {
                Debug.Log(frames[0][i]);
            }

            var t = new JsonFrameStruct()
            {
                key = GestureMapper.PrefixOfName(frameData.name),
                value = new JsonFrameProperty()
                {
                    Frames = frames,
                    Type = (int)GestureType.Weapon
                }
            };
            return t;
        }

        public async Task RecordFrame(FrameData frame, GestureCollections collection,
            string characterName = "")
        {
            if (collection == GestureCollections.system)
            {
                if (systemGestures.ContainsKey(frame.name))
                    systemGestures[frame.name] = frame;
                else
                    systemGestures.Add(frame.name, frame);
                GestureMapper.SendFrameData(collection.ToString(), frame);
                return;
            }

            if (collection == GestureCollections.supportive)
            {
                if (supportiveGestures.ContainsKey(frame.name))
                    supportiveGestures[frame.name] = frame;
                else
                    supportiveGestures.Add(frame.name, frame);
                GestureMapper.SendFrameData(collection.ToString(), frame);
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
                        Gestures = new List<JsonFrameStruct>()
                        {
                            CreateNewFrame(frame)
                        }
                    }
                    : AddFrameToChar(frame, dictionary[characterName]);
                try
                {
                    CharacterMapper.SendCharacterStruct(new JsonCharacterStruct()
                        { key = characterName, value = jsonChar });
                }
                catch (Exception e)
                {
                    HintWindow.Log(e.Message);
                }
            }
        }

        public bool TryGetDynamicGesture(string gesture, out DynamicGesture frame)
        {
            return characterGestures.TryGetValue(gesture, out frame) ||
                   characterGestures.TryGetValue(GestureMapper.PrefixOfName(gesture), out frame);
        }

        public bool TryGetFrameData(string name, out FrameData frame)
        {
            if (characterGestures.ContainsKey(GestureMapper.PrefixOfName(name)))
            {
                frame = characterGestures[GestureMapper.PrefixOfName(name)].frames[GestureMapper.IndexOfName(name)];
                return true;
            }

            frame = null;
            return false;
        }

        public void SetFrameData(FrameData frame, string key)
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
            }
        }
    }
}