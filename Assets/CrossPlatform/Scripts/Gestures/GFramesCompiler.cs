using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using CrossPlatform.Static;
using CrossPlatform.Network;
using Unity.VisualScripting.FullSerializer;
using HandAtlas =  System.Collections.Generic.Dictionary<string, string[]>;

using PlatformAtlas = System.Collections.Generic.Dictionary<string,
    System.Collections.Generic.Dictionary<string, string[]>>;

using FrameAtlas = System.Collections.Generic.Dictionary<string,
    System.Collections.Generic.Dictionary<string, 
        System.Collections.Generic.Dictionary<string, string[]>>>;

namespace CrossPlatform.Gestures
{
    public enum RuntimeXRInteractor
    {
        OpenXR,
        HendaiSolaris
    }
    public class GFramesCompiler
    {
        private RuntimeXRInteractor _xrInteractor;
        private readonly string _jsonPath = "Assets/CrossPlatform/Scripts/Gestures/GFramesLibrary.json";
        private GesturesLibrary _library;
        private FrameAtlas _framesDict = new();

        public void Initialize(RuntimeXRInteractor interactor, ref GesturesLibrary library)
        {
            _xrInteractor = interactor;
            _library = library;
            Read();
          
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
                    Name = jsonFrame.Key
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

                                frame.LeftPoints = convertedPoints;

                            }
                            else if (handP.Key == "right")
                            {
                                frame.RightPoints = convertedPoints;
                            }

                            
                        }
                    }
                }

                _library.SetGestureFrame(frame);
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