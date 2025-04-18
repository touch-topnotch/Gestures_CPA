using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Characters;
using Gesture_Editor_SDK.Realtime;
using ModestTree;
using Newtonsoft.Json;
using Scripts.Characters;
using Scripts.Databases;
using Scripts.HandsLogic;
using Scripts.Network;
using UnityEngine;
using Scripts.Static;
using Scripts.Systems;
using Scripts.Weapons;
using Unity.Services.CloudSave;
using FrameAtlas = System.Collections.Generic.Dictionary<string, Scripts.Databases.DBFrameStruct>;
using GestureAtlas = System.Collections.Generic.Dictionary<string, Scripts.Databases.JsonGestureStruct>;

namespace Scripts.Gestures
{
    public static class GestureMapper
    {
        public static readonly string _jsonPath = Application.dataPath + "/Resources/Database/CharacterLibrary.json";

        public static GestureFrame
            JsonGestureToGestureFrame(string key, JsonGestureProperty jsonStruct, int index = 0) => new GestureFrame(
            key, StringToHandsStruct(jsonStruct.Frames[index]));

        public static void SendGestureFrame(string collectionKey, GestureFrame frame)
        {
#if UNITY_EDITOR
            var jsonStruct = new JsonGestureStruct
            {
                key = frame.name,
                value = new JsonGestureProperty()
                {
                    Type = 0,
                    Frames = new List<string[]> { HandsStructToString(frame.Hands) }
                }
            };
            CloudSaveProcessor.SetItemToCloud(JsonConvert.SerializeObject(jsonStruct), collectionKey,
                (e) => { Debug.Log(collectionKey + ": " + jsonStruct.key + " was sent to cloud"); });
#endif
        }

        public static string ReplaceCharacters(string input)
        {
            
            var s = input;
            if (s.Length < 3)
                return input;

            var newS = "";
            for (int k = 0; k < s.Length; k += 3)
            {
                if (s[k] == '$' && s[k + 1] == '$' && s[k + 2] == '$')
                {
                    newS += "!";
                    continue;
                }

                newS += "" + s[k] + s[k + 1] + s[k + 2];
            }

            return newS;
        }

        public static bool TryGetDynamicGesture(JsonGestureStruct jsonStruct,
            in Dictionary<string, Weapon> recognizables,
            out DynamicGesture gesture)
        {
            // firstly, find IRecognizable, if exist.
            IRecognizable recognizableObject = null;

            foreach (var gestureName in recognizables.Keys)
            {
                if (gestureName == jsonStruct.key)
                {
                    recognizableObject = recognizables[gestureName];
                    break;
                }
            }

            if (recognizableObject == null)
            {
                //                Debug.Log("Recognizables for gesture " + jsonStruct.Name+ " not found");
            }

            List<GestureFrame> frames = new();

            foreach (var frame in jsonStruct.value.Frames)
            {
                frames.Add(
                    new GestureFrame(
                        jsonStruct.key + "_" + frames.Count,
                        StringToHandsStruct(frame)
                    )
                );
            }


            gesture = new DynamicGesture(jsonStruct.key,
                (GestureType)jsonStruct.value.Type,
                frames,
                recognizableObject);

            return true;
        }


        public static async Task<Dictionary<string, GestureFrame>> ReadGestureFrames(string collectionKey)
        {
            
            var jsonGestures = await CloudSaveService.Instance.Data.Custom.LoadAllAsync(collectionKey);
            if (jsonGestures == null)
            {
                throw new Exception("Wrong collection key used or there is no gestures in collection");
            }

            var dictionary = new Dictionary<string, GestureFrame>();
            foreach (var key in jsonGestures.Keys)
            {
                dictionary.Add(key,
                    JsonGestureToGestureFrame(key, jsonGestures[key].Value.GetAs<JsonGestureProperty>()));
            }

            return dictionary;
        }

        public static async Task<Dictionary<string, DynamicGesture>> ReadCharacterGestures(
            Dictionary<string, Character> characters)
        {
            var jsonCharacters = await CharacterMapper.GetCharacterStructs(); // json прочитали
            if (jsonCharacters == null)
                return null;
            Dictionary<string, DynamicGesture> gestures = new();

            foreach (var charKey in jsonCharacters.Keys)
            {
                if (characters.ContainsKey(charKey))
                {
                    foreach (var gesture in jsonCharacters[charKey].Gestures)
                    {
                        if (TryGetDynamicGesture(gesture, characters[charKey].weapons,
                                out var dynamicGesture))
                        {
                            gestures.Add(dynamicGesture.Name, dynamicGesture);
                        }
                    }
                }
            }

            return gestures;
        }


        public static async void UpdateDynamicGesture(string characterName, JsonGestureStruct jsonGesture)
        {
            var _jsonCharacters = await CharacterMapper.GetCharacterStructs();

            foreach (var key in _jsonCharacters.Keys)
            {
                if (key == characterName)
                {
                    for (int j = 0; j < _jsonCharacters[key].Gestures.Count; j++)
                    {
                        if (_jsonCharacters[key].Gestures[j].key == jsonGesture.key)
                        {
                            _jsonCharacters[key].Gestures[j] = jsonGesture;

                            CharacterMapper.SendCharacterStruct(new JsonCharacterStruct(key, _jsonCharacters[key]));

                            Debug.Log($"{jsonGesture.key} overrided in Json");
                            return;
                        }
                    }

                    _jsonCharacters[key].Gestures.Add(jsonGesture);
                    CharacterMapper.SendCharacterStruct(new JsonCharacterStruct(key, _jsonCharacters[key]));
                    Debug.Log($"{jsonGesture.key} created in Character " + characterName);
                    return;
                }
            }

            CharacterMapper.SendCharacterStruct(new JsonCharacterStruct(characterName,
                new JsonCharacterProperties(
                    characterName + " is cool!",
                    "Resources/Characters/" + characterName,
                    new List<JsonGestureStruct>()
                    {
                        jsonGesture,
                    })));

            Debug.Log($"Gesture {jsonGesture.key} and character " + characterName + " created");
        }

        public static void UpdateDynamicGesture(string characterName, DynamicGesture gesture)
        {
            var jsonGesture = new JsonGestureStruct
            {
                key = gesture.Name,
                value = new JsonGestureProperty()
                {
                    Type = (int)gesture.gestureType,
                    Frames = gesture.frames.ConvertAll(frame => HandsStructToString(frame.Hands)),
                }
            };

            UpdateDynamicGesture(characterName, jsonGesture);
        }


        public static string[] HandsStructToString(HandsStruct hands)
        {
            var s = new string [4];
            s[0] = hands.LeftBones == null ? "!" : VectorConverter.QuaternionArrayToCode(hands.LeftBones.rotations);
            s[1] = hands.RightBones == null ? "!" : VectorConverter.QuaternionArrayToCode(hands.RightBones.rotations);
            s[2] = hands.LeftBones == null ? "!" : VectorConverter.VecToCodePos(hands.LeftBones.rootPos);
            s[3] = hands.RightBones == null ? "!" : VectorConverter.VecToCodePos(hands.RightBones.rootPos);
            return s;
        }


        public static HandsStruct StringToHandsStruct(string[] hands)
        {
            return new HandsStruct(
                hands[2] == "" || hands[2] == "!"
                    ? null
                    : new BonesData(
                        type: HandType.left,
                        rotations: VectorConverter.CodeToQuaternionArray(hands[0]),
                        rootPos: VectorConverter.CodeToVec3Pos(hands[2])
                    ),
                hands[3] == "" || hands[3] == "!"
                    ? null
                    : new BonesData(
                        type: HandType.right,
                        rotations: VectorConverter.CodeToQuaternionArray(hands[1]),
                        rootPos: VectorConverter.CodeToVec3Pos(hands[3])
                    )
            );
        }

        public static string PrefixOfName(string name) => name.Split('_').Length == 1
            ? name
            : name.Substring(0,
                name.Length - name.Split('_')[^1].Length - 1);

        public static int IndexOfName(string name) => int.Parse(name.Split('_')[^1]);
    }
}