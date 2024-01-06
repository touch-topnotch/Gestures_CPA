using System.Collections.Generic;
using Newtonsoft.Json;
using Scripts.Databases;
using Scripts.Hands;
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

        private readonly string _jsonPath = Application.dataPath + "/Resources/Database/GFramesLibrary.json";
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
                GestureFrame frame = new GestureFrame(
                    jsonFrame.Key,
                    new HandsStruct(
                        new BonesData(
                            type: HandType.left,
                            rotations: VectorConverter.CodeToQuaternionArray(jsonFrame.Value.left_rots),
                            rootPos: VectorConverter.CodeToVec3Pos(jsonFrame.Value.left_pos)
                        ),
                        new BonesData(
                            type: HandType.right,
                            rotations: VectorConverter.CodeToQuaternionArray(jsonFrame.Value.right_rots),
                            rootPos: VectorConverter.CodeToVec3Pos(jsonFrame.Value.right_pos)
                        )
                    ));
                _library.SetGestureFrame(frame);
            }

        }


        public void Record(HandsStruct hands, string name)
        {

            DBFrameStruct frameStruct = new DBFrameStruct();


            if (hands.LeftBones != null)
            {
                frameStruct.left_rots = VectorConverter.QuaternionArrayToCode(hands.LeftBones.rotations);
                frameStruct.left_pos = VectorConverter.VecToCodePos(hands.LeftBones.rootPos);
            }


            if (hands.RightBones != null)
            {
                frameStruct.right_rots = VectorConverter.QuaternionArrayToCode(hands.RightBones.rotations);
                frameStruct.right_pos = VectorConverter.VecToCodePos(hands.RightBones.rootPos);
            }

            Read();

            if (_framesDict.ContainsKey(name))
            {
                _framesDict[name] = frameStruct;
                Debug.Log($"{name} overrided");

            }

            else
            {
                _framesDict.Add(name, frameStruct);
                Debug.Log($"{name} added");
            }

            var jsonString = JsonConvert.SerializeObject(_framesDict, Formatting.Indented);

            DataChanel.Send(_jsonPath, jsonString);
        }
    }

}