using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Scripts.Static;
using Scripts.Network;
using Scripts.PlayerLogic;
using Zenject;

using HandAtlas =  System.Collections.Generic.Dictionary<string, string[]>;

using PlatformAtlas = System.Collections.Generic.Dictionary<string,
    System.Collections.Generic.Dictionary<string, string[]>>;

using FrameAtlas = System.Collections.Generic.Dictionary<string,
    System.Collections.Generic.Dictionary<string, 
        System.Collections.Generic.Dictionary<string, string[]>>>;

namespace Scripts.Gestures
{
   
    public class GFramesCompiler
    {
        [Inject]
        private RuntimeXRInteractor _xrInteractor;
        private GesturesLibrary _library;
        
        private readonly string _jsonPath = "/Users/dmitry057/Projects/UnityProjects/Gestures_CPA/Assets/Resources/Database/GFramesLibrary.json";
        
        private FrameAtlas _framesDict = new();
        public GFramesCompiler(GesturesLibrary library)
        {
            _library = library;
        }
        public void Read()
        {
            FrameAtlas reddenFrames =
                JsonConvert.DeserializeObject<FrameAtlas>(DataChanel.Get(_jsonPath));
            if (reddenFrames == null)
                return;
            
            _framesDict = reddenFrames;

            foreach (KeyValuePair<string, PlatformAtlas> jsonFrame in reddenFrames)
            {
                GestureFrame frame = new GestureFrame
                {
                    name = jsonFrame.Key
                };
                foreach (KeyValuePair<string, HandAtlas> jsonPlatform in jsonFrame.Value)
                {
                    if (jsonPlatform.Key == _xrInteractor.ToString())
                    {
                        foreach (KeyValuePair<string, string[]> handP in jsonPlatform.Value)
                        {
                            
                            var convertedPoints =  Vector3Converter.convertToVector3(handP.Value);
                            if (handP.Key == "left")
                            {

                                frame.Hands.LeftPoints = convertedPoints;

                            }
                            else if (handP.Key == "right")
                            {
                                frame.Hands.RightPoints = convertedPoints;
                            }

                            
                        }
                    }
                }
                _library.SetGestureFrame(frame);
            }

        }


        public void Record(HandsStruct hands, string name)
        {

            HandAtlas pointsOnPlatform = new HandAtlas();

            if (hands.LeftPoints != null)
                pointsOnPlatform.Add("left", Vector3Converter.convertToString(hands.LeftPoints));

            if (hands.RightPoints != null)
                pointsOnPlatform.Add("right", Vector3Converter.convertToString(hands.RightPoints));
            
            Read();

            if (!_framesDict.ContainsKey(name))
            {
                PlatformAtlas platform = new();
                platform.Add(_xrInteractor.ToString(), pointsOnPlatform);
                _framesDict.Add(name, platform);
            }
            else
            {
                bool f = false;
                foreach (var platformInFrame in _framesDict[name])
                {

                    if (platformInFrame.Key == _xrInteractor.ToString())
                    {
                        
                        f = true;
                        _framesDict[name][_xrInteractor.ToString()] = pointsOnPlatform;
                        break;
                    }
                }

                if (!f)
                {
                    _framesDict[name].Add(_xrInteractor.ToString(), pointsOnPlatform);
                }

            }

            var jsonString = JsonConvert.SerializeObject(_framesDict, Formatting.Indented);
            Debug.Log(jsonString + " written");
            DataChanel.Send(_jsonPath, jsonString);
        }
    }

}