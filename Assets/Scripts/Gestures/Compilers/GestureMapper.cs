using System;
using System.Collections.Generic;
using Design.GUI_Gesture;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Scripts.Databases;
using Scripts.Gestures.Classes;
using Scripts.Hands;
using UnityEngine;
using Scripts.Static;
using Scripts.Network;
using Scripts.PlayerLogic;
using Unity.VisualScripting;
using Zenject;

using FrameAtlas = System.Collections.Generic.Dictionary<string,Scripts.Databases.DBFrameStruct>;
using GestureAtlas =  System.Collections.Generic.Dictionary<string,Scripts.Databases.JsonGestureStruct>;
namespace Scripts.Gestures
{

    public static class GestureMapper
    {
        private static readonly string _jsonPath = Application.dataPath + "/Resources/Database/GesturesLibrary.json";

        public static Dictionary<string, JsonGestureStruct> GetJsonGesturesStruct =>
            JsonConvert.DeserializeObject<GestureAtlas>(DataChanel.Get(_jsonPath));

        public static void SendJsonGesturesStruct(Dictionary<string, JsonGestureStruct> structs) =>
         DataChanel.Send(
             _jsonPath,
             JsonConvert.SerializeObject(structs, Formatting.Indented)
             );
        

        public static void ReplaceCharacters()
        {
            var jsonStruct = GetJsonGesturesStruct;
            foreach (var name in jsonStruct.Keys)  
            {

                for (int i = 0; i < jsonStruct[name].Frames.Count; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        var s = jsonStruct[name].Frames[i][j];
                        if (s.Length < 3)
                            continue;
                        
                        var newS = "";
                        for (int k = 0; k < s.Length; k+=3)
                        {
                            if (s[k] == '$' && s[k + 1] == '$' && s[k + 2] == '$')
                            {
                                newS += "!";
                                continue;
                            }
                            newS += "" + s[k] + s[k + 1] + s[k + 2];
                        }

                        jsonStruct[name].Frames[i][j] = newS;
                    }
                   
                }
            }
            SendJsonGesturesStruct(jsonStruct);
        }
        public static Dictionary<string, DynamicGesture> ReadDynamicGestures()
        {
            var gestureDict = GetJsonGesturesStruct;

            if (gestureDict == null)
                return null;

            var dynamicGestures = new Dictionary<string, DynamicGesture>();
            foreach (var jsonGesture in gestureDict)
            {
                List<GestureFrame> frames = new();

                foreach (var frame in jsonGesture.Value.Frames)
                {
                    frames.Add(
                        new GestureFrame(
                            jsonGesture.Key + "_" + frames.Count,
                            StringToHandsStruct(frame)
                        )
                    );
                }

                List<Asset> assets = new();
                foreach (var asset in jsonGesture.Value.GUI.Assets)
                {
                    if (!Enum.TryParse<AssetType>(asset.Type, out var assetType)) assetType = AssetType.RESOURCE;

                    assets.Add(
                        new Asset(
                            assetType,
                            asset.Path,
                            asset.SpawnPoint
                        )
                    );
                }

                dynamicGestures.Add(jsonGesture.Key, new DynamicGesture(
                    jsonGesture.Key,
                    frames,
                    GestureType.HIT,
                    new ParsedGUI(
                        assets,
                        jsonGesture.Value.GUI.FrameLogic
                    ),
                    new Melee()
                ));
            }

            return dynamicGestures;
        }

        public static void UpdateDynamicGesture(DynamicGesture gesture)
        {
            var name = gesture.Name;

            var jsonGesture = new JsonGestureStruct
            {
                Frames = gesture.frames.ConvertAll(frame => HandsStructToString(frame.Hands)),
                GUI = new JsonGUI
                {
                    Assets = new List<JsonAsset>(),
                    FrameLogic = new List<string>()
                },
                Type = gesture.gestureType
            };

            var _gestureDict = GetJsonGesturesStruct;

            if (!_gestureDict.ContainsKey(name))
            {
                Debug.Log($"{name} added to Json");
                _gestureDict.Add(name, jsonGesture);
            }
            else
            {
                Debug.Log($"{name} overrided in Json");
                _gestureDict[name] = jsonGesture;
            }

            SendJsonGesturesStruct(_gestureDict);
        }

        public static List<string> HandsStructToString(HandsStruct hands)
        {
            return new List<string>
            {
                VectorConverter.QuaternionArrayToCode(hands.LeftBones.rotations),
                VectorConverter.QuaternionArrayToCode(hands.RightBones.rotations),
                VectorConverter.VecToCodePos(hands.LeftBones.rootPos),
                VectorConverter.VecToCodePos(hands.RightBones.rootPos)
            };
        }

        public static HandsStruct StringToHandsStruct(List<string> hands)
        {
            return new HandsStruct(
                new BonesData(
                    type: HandType.left,
                    rotations: VectorConverter.CodeToQuaternionArray(hands[0]),
                    rootPos: VectorConverter.CodeToVec3Pos(hands[2])
                ),
                new BonesData(
                    type: HandType.right,
                    rotations: VectorConverter.CodeToQuaternionArray(hands[1]),
                    rootPos: VectorConverter.CodeToVec3Pos(hands[3])
                )
            );
        }

        public static string PrefixOfName(string name) => name.Substring(0,
            name.Length - name.Split('_')[^1].Length);

        public static int IndexOfName(string name) => int.Parse(name.Split('_')[^1]);
    }


}