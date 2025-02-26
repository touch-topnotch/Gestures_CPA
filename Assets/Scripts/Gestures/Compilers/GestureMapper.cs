using System.Collections.Generic;
using Gesture_Editor_SDK.Realtime;
using Newtonsoft.Json;
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
        private static readonly string _jsonPath = Application.dataPath + "/Resources/Database/GesturesLibrary.json";

        public static Dictionary<string, JsonGestureStruct> GetJsonGesturesStruct(ulong id)=>
            JsonConvert.DeserializeObject<GestureAtlas>(DataChanel.Get(_jsonPath, id));

        public static void SendJsonGesturesStruct(Dictionary<string, JsonGestureStruct> structs) =>
         DataChanel.Send(
             _jsonPath,
             JsonConvert.SerializeObject(structs, Formatting.Indented)
             );
        
        public static void ReplaceCharacters(ulong id)
        {
            var jsonStruct = GetJsonGesturesStruct(id);
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
        public static Dictionary<string, DynamicGesture> ReadDynamicGestures(PlayerData data)
        {
            var gestureDict = GetJsonGesturesStruct(data.id);

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

                var asset = jsonGesture.Value.Asset;

                var res = Resources.Load(asset.Path) as GameObject;
           
                if (!res) // нет в папке Resources
                {
                    Debug.Log("Resource not found by path: " + asset.Path);
                    if (!isDebug)
                        continue;
                    else
                        res = Resources.Load(_emptyRecognizablePath) as GameObject;
                }
                
                var recognizable = res.GetComponent(typeof(IRecognizable)) as IRecognizable;
                
                if (recognizable == null) // на текущем обьекте нет IRecognizable
                {
                    Debug.Log("Resource has no IRecognizable component");
                    if(!isDebug)
                        continue;
                    else
                    {
                        Debug.Log("Empty Recognizable added"); // берем пустой
                        res = Resources.Load(_emptyRecognizablePath) as GameObject;
                    }
                }

                if (res == null)
                    return null;
                
                if(asset.Type == 0)
                    res = Spawner.SpawnPooledPrefab(res, data.playerTransform, true);

                recognizable = res.GetComponent(typeof(IRecognizable)) as IRecognizable;
                recognizable.playerData = data;
                dynamicGestures.Add(jsonGesture.Key, new DynamicGesture(
                    jsonGesture.Key,
                    frames,
                    recognizable
                ));
            }

            return dynamicGestures;
        }


        public static void UpdateDynamicGesture(DynamicGesture gesture, ulong playerId)
        {
            var name = gesture.Name;

            var jsonGesture = new JsonGestureStruct
            {
                Frames = gesture.frames.ConvertAll(frame => HandsStructToString(frame.Hands)),
                Asset = new JsonAsset
                {
                    Type = 0,
                    Path = "Prefabs/Effects/" + name + "/" + name + "Prefab"
                }
            };

            var _gestureDict = GetJsonGesturesStruct(playerId);

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
            name.Length - name.Split('_')[^1].Length - 1);

        public static int IndexOfName(string name) => int.Parse(name.Split('_')[^1]);
    }


}