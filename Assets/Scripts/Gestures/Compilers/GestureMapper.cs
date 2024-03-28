using System.Collections.Generic;
using System.Threading.Tasks;
using Characters;
using Gesture_Editor_SDK.Realtime;
using Scripts.Characters;
using Scripts.Databases;
using Scripts.HandsLogic;
using UnityEngine;
using Scripts.Static;
using Scripts.Weapons;
using FrameAtlas = System.Collections.Generic.Dictionary<string,Scripts.Databases.DBFrameStruct>;
using GestureAtlas =  System.Collections.Generic.Dictionary<string,Scripts.Databases.JsonGestureStruct>;

namespace Scripts.Gestures
{
    public static class GestureMapper
    {
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

        public static bool TryGetDynamicGesture(JsonGestureStruct jsonStruct, in Dictionary<string, Weapon> recognizables,
            out DynamicGesture gesture)
        {

            // firstly, find IRecognizable, if exist.
            IRecognizable recognizableObject = null;

            foreach (var gestureName in recognizables.Keys)
            {
                if (gestureName == jsonStruct.Name)
                {
                    recognizableObject = recognizables[gestureName];
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

        public static async Task<Dictionary<string, DynamicGesture>> ReadDynamicGestures(
            Dictionary<string, Character> characters)
        {

            var jsonCharacters  =await CharacterMapper.GetCharacterStructs(); // json прочитали
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
                    for(int j = 0; j < _jsonCharacters[key].Gestures.Count; j ++)
                    {
                        if (_jsonCharacters[key].Gestures[j].Name == jsonGesture.Name)
                        {
                         
                            _jsonCharacters[key].Gestures[j] = jsonGesture;
                            
                            CharacterMapper.SendCharacterStruct(new JsonCharacterStruct(key, _jsonCharacters[key]));
                            
                            Debug.Log($"{jsonGesture.Name} overrided in Json");
                            return;
                        }
                    }
                    
                    _jsonCharacters[key].Gestures.Add(jsonGesture);
                    CharacterMapper.SendCharacterStruct(new JsonCharacterStruct(key, _jsonCharacters[key]));
                    Debug.Log($"{jsonGesture.Name} created in Character " + characterName);
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


        public static string[] HandsStructToString(HandsStruct hands)
        {
            Debug.Log(hands.LeftBones);
            Debug.Log(hands.RightBones);
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