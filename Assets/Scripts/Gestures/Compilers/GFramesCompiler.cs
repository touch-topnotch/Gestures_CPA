using System.Collections.Generic;
using Newtonsoft.Json;
using Scripts.Databases;
using UnityEngine;
using Scripts.Static;
using Scripts.Network;
using Scripts.PlayerLogic;
using Zenject;

using FrameAtlas = System.Collections.Generic.Dictionary<string,Scripts.Databases.DBFrameStruct>;

namespace Scripts.Gestures
{
   
    public class GFramesCompiler
    {
        [Inject]
        private GesturesLibrary _library;

        private readonly string
            //_jsonPath = "/Users/dmitry057/Projects/UnityProjects/Gestures_CPA/Assets/Resources/Database/GFramesLibrary.json";
            _jsonPath = "C:/Unity Projects/Gestures_CPA/Assets/Resources/Database/GFramesLibrary.json";
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

            foreach (KeyValuePair<string, DBFrameStruct> jsonFrame in reddenFrames)
            {
                GestureFrame frame = new GestureFrame
                {
                    name = jsonFrame.Key
                };
                frame.Hands.LeftBones.rotations = Vector3Converter.convertToQuaternion(jsonFrame.Value.left_rots);
                frame.Hands.RightBones.rotations = Vector3Converter.convertToQuaternion(jsonFrame.Value.right_rots);
                frame.Hands.LeftBones.rootPos = Vector3Converter.convertToVector3(jsonFrame.Value.left_pos);
                frame.Hands.RightBones.rootPos = Vector3Converter.convertToVector3(jsonFrame.Value.right_pos);
                
                _library.SetGestureFrame(frame);
            }

        }


        public void Record(HandsStruct hands, string name)
        {

            DBFrameStruct frameStruct = new DBFrameStruct();


            if (hands.LeftBones != null)
            {
                frameStruct.left_rots = Vector3Converter.convertToString(hands.LeftBones.rotations);
                frameStruct.right_pos = hands.LeftBones.rootPos.ToString();
            }


            if (hands.RightBones != null)
            {
                frameStruct.right_rots = Vector3Converter.convertToString(hands.RightBones.rotations);
                frameStruct.right_pos = hands.RightBones.rootPos.ToString();
            }
               
            Read();

            var jsonString = JsonConvert.SerializeObject(_framesDict, Formatting.Indented);
            Debug.Log(jsonString + " written");
            DataChanel.Send(_jsonPath, jsonString);
        }
    }

}