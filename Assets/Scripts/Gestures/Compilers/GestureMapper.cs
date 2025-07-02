using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Characters;
using Newtonsoft.Json;
using Scripts.Characters;
using Scripts.Databases;
using Scripts.Gesture_Editor_SDK.Realtime;
using Scripts.HandsLogic;
using Scripts.Network;
using UnityEngine;
using Scripts.Static;
using Scripts.Static.Definitions;
using Scripts.Weapons;
using Unity.Services.CloudSave;
using FrameAtlas = System.Collections.Generic.Dictionary<string, Scripts.Databases.DBFrameStruct>;
using GestureAtlas = System.Collections.Generic.Dictionary<string, Scripts.Databases.JsonFrameStruct>;

namespace Scripts.Gestures
{
    public static class GestureMapper
    {
        public static readonly string _jsonPath = Application.dataPath + "/Resources/Database/CharacterLibrary.json";

        public static FrameData
            JsonFrameToFrameData(string key, JsonFrameProperty jsonStruct, int index = 0) =>
            StringToFrameData(key, jsonStruct.Frames[index]);

        public static void SendFrameData(string collectionKey, FrameData frame)
        {
            var jsonStruct = new JsonFrameStruct
            {
                key = frame.name,
                value = new JsonFrameProperty()
                {
                    Type = 0,
                    Frames = new List<string[]> { FrameDataToString(frame) }
                }
            };
            CloudSaveProcessor.SetItemToCloud(JsonConvert.SerializeObject(jsonStruct), collectionKey,
                (e) => { Debug.Log(collectionKey + ": " + jsonStruct.key + " was sent to cloud"); });
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

        public static bool TryGetDynamicGesture(JsonFrameStruct jsonStruct,
            out DynamicGesture gesture)
        {
            // firstly, find IGestureAbility, if exist.
            // IGestureAbility recognizableObject = null;
            //
            // foreach (var gestureName in recognizables.Keys)
            // {
            //     if (gestureName == jsonStruct.key)
            //     {
            //         recognizableObject = recognizables[gestureName];
            //         break;
            //     }
            // }
            //
            // if (recognizableObject == null)
            // {
            //     Debug.Log("Recognizables for gesture " + jsonStruct.key + " not found");
            // }

            List<FrameData> frames = new();

            foreach (var frame in jsonStruct.value.Frames)
            {
                frames.Add(StringToFrameData(jsonStruct.key + "_" + frames.Count, frame));
            }


            gesture = new DynamicGesture(jsonStruct.key, (AbilityType)jsonStruct.value.Type,frames);

            return true;
        }


        public static async Task<Dictionary<string, FrameData>> ReadFrameDatas(string collectionKey)
        {
            var JsonFrames = await CloudSaveService.Instance.Data.Custom.LoadAllAsync(collectionKey);
            if (JsonFrames == null)
            {
                throw new Exception("Wrong collection key used or there is no gestures in collection");
            }

            var dictionary = new Dictionary<string, FrameData>();
            foreach (var key in JsonFrames.Keys)
            {
                dictionary.Add(key,
                    JsonFrameToFrameData(key, JsonFrames[key].Value.GetAs<JsonFrameProperty>()));
            }

            return dictionary;
        }

        public static async Task<Dictionary<string, DynamicGesture>> ReadCharacterGestures()
        {
            var jsonCharacters = await CharacterMapper.GetCharacterStructs(); // json прочитали
            if (jsonCharacters == null)
                return null;
            Dictionary<string, DynamicGesture> gestures = new();

            foreach (var charKey in jsonCharacters.Keys)
            {
               
                foreach (var gesture in jsonCharacters[charKey].Gestures)
                {
                    if (TryGetDynamicGesture(gesture, out var dynamicGesture))
                    {
                        gestures.Add(dynamicGesture.name, dynamicGesture);
                    }
                }
                
            }

            return gestures;
        }


        public static async void UpdateDynamicGesture(string characterName, JsonFrameStruct JsonFrame)
        {
            var _jsonCharacters = await CharacterMapper.GetCharacterStructs();

            foreach (var key in _jsonCharacters.Keys)
            {
                if (key == characterName)
                {
                    for (int j = 0; j < _jsonCharacters[key].Gestures.Count; j++)
                    {
                        if (_jsonCharacters[key].Gestures[j].key == JsonFrame.key)
                        {
                            _jsonCharacters[key].Gestures[j] = JsonFrame;

                            CharacterMapper.SendCharacterStruct(new JsonCharacterStruct(key, _jsonCharacters[key]));

                            Debug.Log($"{JsonFrame.key} overrided in Json");
                            return;
                        }
                    }

                    _jsonCharacters[key].Gestures.Add(JsonFrame);
                    CharacterMapper.SendCharacterStruct(new JsonCharacterStruct(key, _jsonCharacters[key]));
                    Debug.Log($"{JsonFrame.key} created in Character " + characterName);
                    return;
                }
            }

            CharacterMapper.SendCharacterStruct(new JsonCharacterStruct(characterName,
                new JsonCharacterProperties(
                    characterName + " is cool!",
                    "Resources/Characters/" + characterName,
                    new List<JsonFrameStruct>()
                    {
                        JsonFrame,
                    })));

            Debug.Log($"Gesture {JsonFrame.key} and character " + characterName + " created");
        }

        public static void UpdateDynamicGesture(string characterName, DynamicGesture gesture)
        {
            var JsonFrame = new JsonFrameStruct
            {
                key = gesture.name,
                value = new JsonFrameProperty()
                {
                    Type = (int)gesture.gestureType,
                    Frames = gesture.frames.ConvertAll(FrameDataToString),
                }
            };

            UpdateDynamicGesture(characterName, JsonFrame);
        }


        public static string[] FrameDataToString(FrameData frameData)
        {
            var s = new string [4];
            s[0] = frameData.LeftBones == null
                ? "!"
                : VectorConverter.QuaternionArrayToCode(frameData.LeftBones.rotations);
            s[1] = frameData.RightBones == null
                ? "!"
                : VectorConverter.QuaternionArrayToCode(frameData.RightBones.rotations);
            s[2] = frameData.LeftBones == null ? "!" : VectorConverter.VecToCodePos(frameData.LeftBones.rootPos);
            s[3] = frameData.RightBones == null ? "!" : VectorConverter.VecToCodePos(frameData.RightBones.rootPos);
            return s;
        }


        public static FrameData StringToFrameData(string name, string[] hands)
        {
            return new FrameData(
                name,
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