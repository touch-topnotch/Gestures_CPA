using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Scripts.Static;
using Scripts.Network;
using Scripts.PlayerLogic;
using Zenject;

using TransfAtlas = System.Collections.Generic.Dictionary<string, string[]>;
using HandAtlas = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string[]>>;

using PlatformAtlas = System.Collections.Generic.Dictionary<string,
    System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string[]>>>;

using FrameAtlas = System.Collections.Generic.Dictionary<string,
    System.Collections.Generic.Dictionary<string,
        System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string[]>>>>;

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
                        foreach (KeyValuePair<string, TransfAtlas> handP in jsonPlatform.Value)
                        {
                            if (handP.Key == "left")
                            {
                                foreach (KeyValuePair<string, string[]> transf in handP.Value)
                                {
                                    if(transf.Key == "pos")
                                        frame.Hands.LeftBones.Positions = Vector3Converter.convertToVector3(transf.Value);
                                    if(transf.Key == "rot")
                                        frame.Hands.LeftBones.Rotations = Vector3Converter.convertToQuaternion(transf.Value);
                                }
                            }
                            else  if (handP.Key == "right")
                            {
                                foreach (KeyValuePair<string, string[]> transf in handP.Value)
                                {
                                    if(transf.Key == "pos")
                                        frame.Hands.RightBones.Positions = Vector3Converter.convertToVector3(transf.Value);
                                    if(transf.Key == "rot")
                                        frame.Hands.RightBones.Rotations = Vector3Converter.convertToQuaternion(transf.Value);
                                }
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

            
            if (hands.LeftBones != null)
            {
                TransfAtlas transfAtlas = new TransfAtlas()
                {
                    {"pos", Vector3Converter.convertToString(hands.LeftBones.Positions)},
                    {"rot", Vector3Converter.convertToString(hands.LeftBones.Rotations)},
                };
                pointsOnPlatform.Add("left", 
                    transfAtlas
                );
            }
               

            if (hands.RightBones != null)
            {
                TransfAtlas transfAtlas = new TransfAtlas()
                {
                    {"pos", Vector3Converter.convertToString(hands.RightBones.Positions)},
                    {"rot", Vector3Converter.convertToString(hands.RightBones.Rotations)},
                };
                pointsOnPlatform.Add("right", 
                    transfAtlas
                );
            }
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