using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using CrossPlatform.Static;
using CrossPlatform.Network;

using HandAtlas =  System.Collections.Generic.Dictionary<string, string[]>;

using PlatformAtlas = System.Collections.Generic.Dictionary<string,
    System.Collections.Generic.Dictionary<string, string[]>>;

using FrameAtlas = System.Collections.Generic.Dictionary<string,
    System.Collections.Generic.Dictionary<string, 
        System.Collections.Generic.Dictionary<string, string[]>>>;

namespace CrossPlatform.Gestures
{
    public class GFramesCompiler
    {
        private readonly RuntimePlatform _debugPlatform = RuntimePlatform.Android;
        private readonly string _jsonPath = "Assets/CrossPlatform/Scripts/Gestures/GFramesLibrary.json";
        private List<GestureFrame> _framesLibrary = new();

        private FrameAtlas _framesDict = new();

        private string _platformName;

        public void Initialize()
        {
            _platformName = Application.platform.ToString();
            Read();

        }

        public void Read()
        {
            FrameAtlas reddenFrames =
                JsonConvert.DeserializeObject<FrameAtlas>(DataChanel.Get(_jsonPath));
            if (reddenFrames == null)
                return;
            Debug.Log($"Reading gesture dataBase ...");
            _framesDict = reddenFrames;

            foreach (KeyValuePair<string, PlatformAtlas> jsonFrame in reddenFrames)
            {
                GestureFrame frame = new GestureFrame
                {
                    Name = jsonFrame.Key
                };
                foreach (KeyValuePair<string, HandAtlas> jsonPlatform in jsonFrame.Value)
                {
                    if (jsonPlatform.Key == _platformName ||
                        jsonPlatform.Key == _debugPlatform.ToString())
                    {
                        foreach (KeyValuePair<string, string[]> handP in jsonPlatform.Value)
                        {
                            if (handP.Key == "left")
                            {
                                frame.LeftPoints = Vector3Converter.convertToVector3(handP.Value);
                            }
                            else if (handP.Key == "right")
                            {
                                frame.RightPoints = Vector3Converter.convertToVector3(handP.Value);
                            }
                        }
                    }
                }

                _framesLibrary.Add(frame);
                Debug.Log($"Added gesture {frame.Name} with type {frame.HandUsed.ToString()}");
            }

        }


        public void Record(Vector3[] pointsLeft, Vector3[] pointsRight, string name)
        {

            HandAtlas pointsOnPlatform = new HandAtlas();

            if (pointsLeft != null)
                pointsOnPlatform.Add("left", Vector3Converter.convertToString(pointsLeft));

            if (pointsRight != null)
                pointsOnPlatform.Add("right", Vector3Converter.convertToString(pointsRight));
            
            Read();

            if (!_framesDict.ContainsKey(name))
            {
                PlatformAtlas platform = new();
                platform.Add(_platformName, pointsOnPlatform);
                _framesDict.Add(name, platform);
            }
            else
            {
                bool f = false;
                foreach (var platformInFrame in _framesDict)
                {

                    if (platformInFrame.Key == _platformName)
                    {
                        f = true;
                        _framesDict[name][_platformName] = pointsOnPlatform;
                        break;
                    }
                }

                if (!f)
                {
                    _framesDict[name].Add(_platformName, pointsOnPlatform);
                }

            }

            var jsonString = JsonConvert.SerializeObject(_framesDict, Formatting.Indented);
            Debug.Log(jsonString + " written");
            DataChanel.Send(_jsonPath, jsonString);
        }
    }

}