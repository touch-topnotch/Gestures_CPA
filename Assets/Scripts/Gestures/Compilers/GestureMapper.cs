using System;
using System.Collections.Generic;
using Characters;
using Gesture_Editor_SDK.Realtime;
using Newtonsoft.Json;
using Scripts.Characters;
using Scripts.Databases;
using Scripts.HandsLogic;
using Scripts.Network;
using Scripts.PlayerLogic;
using UnityEngine;
using Scripts.Static;
using FrameAtlas = System.Collections.Generic.Dictionary<string,Scripts.Databases.DBFrameStruct>;
using GestureAtlas =  System.Collections.Generic.Dictionary<string,Scripts.Databases.JsonGestureStruct>;

namespace Scripts.Gestures
{
    public static class GestureMapper
    {
        private static readonly bool isDebug = true;
        private static string _emptyRecognizablePath = "Weapons/Empty/EmptyPrefab";

        public static readonly string _jsonPath = Application.dataPath + "/Resources/Database/CharacterLibrary.json";

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

        public static bool TryGetDynamicGesture(JsonGestureStruct jsonStruct, in List<IRecognizable> recognizables,
            out DynamicGesture gesture)
        {

            // firstly, find IRecognizable, if exist.
            IRecognizable recognizableObject = null;

            foreach (var rec in recognizables)
            {
                if (rec.gestureName == jsonStruct.Name)
                {
                    recognizableObject = rec;
                    break;
                }
            }

            if (recognizableObject == null)
            {
                Debug.Log("Recognizables for gesture " + jsonStruct.Name+ " not found");
            }

            List<GestureFrame> frames = new();

            foreach (var frame in jsonStruct.Frames)
            {
                frames.Add(
                    new GestureFrame(
                        jsonStruct.Name + "_" + frames.Count,
                        StringToHandsStruct(frame)
                    )
                );
            }


            gesture = new DynamicGesture(jsonStruct.Name,
                (GestureType)jsonStruct.Type,
                frames,
                recognizableObject);

            return true;
        }

        public static Dictionary<string, DynamicGesture> ReadDynamicGestures(
            Dictionary<string, Character> characters)
        {

            var charStruct = CharacterMapper.GetCharacterStruct();
            if (charStruct == null)
                return null;
            Dictionary<string, DynamicGesture> gestures = new();

            foreach (var jsonChar in charStruct)
            {
                if (characters.ContainsKey(jsonChar.Name))
                {
                    foreach (var gesture in jsonChar.Gestures)
                    {
                        if (TryGetDynamicGesture(gesture, characters[jsonChar.Name].recognizables,
                                out var dynamicGesture))
                        {
                            gestures.Add(dynamicGesture.Name, dynamicGesture);
                        }
                    }
                }
            }

            return gestures;
        }

      

     public static void UpdateDynamicGesture(string characterName, JsonGestureStruct jsonGesture)
        {
            var _jsonCharacters = CharacterMapper.GetCharacterStruct();
            
            for(int i = 0; i < _jsonCharacters.Count; i ++)
            {
                if (_jsonCharacters[i].Name == characterName)
                {
                    for(int j = 0; j < _jsonCharacters[i].Gestures.Count; j ++)
                    {
                        if (_jsonCharacters[i].Gestures[j].Name == jsonGesture.Name)
                        {
                         
                            _jsonCharacters[i].Gestures[j] = jsonGesture;
                            
                            CharacterMapper.SendCharacterStruct(_jsonCharacters);
                            
                            Debug.Log($"{jsonGesture.Name} overrided in Json");
                            return;
                        }
                    }
                    
                    _jsonCharacters[i].Gestures.Add(jsonGesture);
                    CharacterMapper.SendCharacterStruct(_jsonCharacters);
                    Debug.Log($"{jsonGesture.Name} created in Character " + characterName);
                    return;
                }
            }

            _jsonCharacters.Add(new JsonCharacterStruct()
            {
                Name = characterName,
                Description = characterName + " is cool!",
                Gestures = new List<JsonGestureStruct>()
                {
                    jsonGesture,
                },
                RootFolder = "Resources/Characters/" + characterName
            });
            CharacterMapper.SendCharacterStruct(_jsonCharacters);
            Debug.Log($"Gesture {jsonGesture.Name} and character " + characterName + " created");
        }
        public static void UpdateDynamicGesture(string characterName, DynamicGesture gesture)
        {

            var jsonGesture = new JsonGestureStruct
            {
                Name = gesture.Name,
                Type = (int)gesture.gestureType,
                Frames = gesture.frames.ConvertAll(frame => HandsStructToString(frame.Hands)),
            };
            UpdateDynamicGesture(characterName, jsonGesture);
         
        }

    
        public static List<string> HandsStructToString(HandsStruct hands)
        {
            return new List<string>
            {
                hands.LeftBones == null ? "": VectorConverter.QuaternionArrayToCode(hands.LeftBones.rotations),
                hands.RightBones == null ? "":VectorConverter.QuaternionArrayToCode(hands.RightBones.rotations),
                hands.LeftBones == null ? "":VectorConverter.VecToCodePos(hands.LeftBones.rootPos),
                hands.RightBones == null ? "":VectorConverter.VecToCodePos(hands.RightBones.rootPos)
            };
        }

        public static HandsStruct StringToHandsStruct(List<string> hands)
        {
            return new HandsStruct(
                hands[0] == "" ? null : new BonesData(
                    type: HandType.left,
                    rotations: VectorConverter.CodeToQuaternionArray(hands[0]),
                    rootPos: VectorConverter.CodeToVec3Pos(hands[2])
                ),
                hands[2] == "" ? null : new BonesData(
                    type: HandType.right,
                    rotations: VectorConverter.CodeToQuaternionArray(hands[1]),
                    rootPos: VectorConverter.CodeToVec3Pos(hands[3])
                )
            );
            
        }

        public static string PrefixOfName(string name) => name.Split('_').Length == 1 ? name : name.Substring(0,
            name.Length - name.Split('_')[^1].Length - 1);

        public static int IndexOfName(string name) => int.Parse(name.Split('_')[^1]);
    }


}