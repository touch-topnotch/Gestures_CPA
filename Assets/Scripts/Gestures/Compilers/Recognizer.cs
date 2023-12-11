using System;
using System.Collections.Generic;
using ModestTree;
using Scripts.Events;
using Scripts.Hands;
using Scripts.PlayerLogic;
using Scripts.Static;
using UnityEngine;
using Zenject;

namespace Scripts.Gestures
{
    public class Recognizer: MonoBehaviour
    {
        [Range(0, 1)] public float positionQuality = 0.01f;
        [Range(0, 1f)] public float rotationQuality = 0.1f;
        public int qualityDecreaser = 10;
       
        [SerializeField]
        private PlayerRig _rig;
        
        private RecognitionEvent _onRecognized;
        private UpdateEvent _onUpdate;
        
        private List<GestureFrame> _possibleFrames;
        private List<DynamicGesture> _possibleGestures;
        
        private int _curGesture;
        private bool wasDrawnПриблизительно = false;
        [Inject]
        private void Construct(UpdateEvent onUpdate)
        {
            _onUpdate = onUpdate;
        }
        
        public void RecognizeDynamicGesture(List<DynamicGesture> possibleGestures, ref RecognitionEvent onRecognized)
        {
            _onRecognized = onRecognized;
            _possibleGestures = possibleGestures;
            _possibleFrames = new List<GestureFrame>();
            for (int i = 0; i < possibleGestures.Count; i++)
            {
                _possibleFrames.Add(possibleGestures[i].GetGestureFrame());
                possibleGestures[i].LogFrames();
            }
            _onUpdate.AddListener(FindStartOfDynamicGesture);
        }
        private void FindStartOfDynamicGesture()
        {
            LogPossibleFrames();
            DrawПриблизетльныйGesture();
            
            var frameId = RecognizeFrame(rotationQuality, positionQuality);
            if (frameId != -1)
            {     
                _curGesture = frameId;
     
                _possibleGestures[_curGesture].FrameRecognized();
                
                _onUpdate.RemoveListener(FindStartOfDynamicGesture);
                HideHands();
                RecognizeInOneGesture();
                
            }
        }

        private void DrawПриблизетльныйGesture()
        {
            if (wasDrawnПриблизительно)
                return;
            
            var приблизительныйFrameId = RecognizeFrameПриблизительно();
            if (приблизительныйFrameId != -1)
            {
                _rig.hands.handCreator.OverrideHands(_possibleFrames[приблизительныйFrameId].Hands);

                foreach (var hand in _rig.hands.handCreator.activeHands)
                {
                    (hand as HandMesh)?.ChangeColorPinPong(HandShaderProps.EdgeColor, new Color(1,1,1,0.1f), new Color(1,1,1,0.5f), 2);
                }

                wasDrawnПриблизительно = true;
                l.rl("рисую приблизительный " + _possibleFrames[приблизительныйFrameId].name);
            }
        }
        private void GoByOneGesture()
        {
            DrawПриблизетльныйGesture();
            var frameId = RecognizeFrame(rotationQuality, positionQuality);
            if (frameId != -1)
            {
                HideHands();
                _possibleGestures[_curGesture].FrameRecognized();
                
                if (_possibleGestures[_curGesture].GetGestureFrame() != null)
                {
                    _possibleFrames = new List<GestureFrame> { _possibleGestures[_curGesture].GetGestureFrame() };
                    return;
                }
                _onUpdate.RemoveListener(GoByOneGesture);
                
                _onRecognized.Invoke(_possibleGestures[_curGesture]);
            }
        }
        private void RecognizeInOneGesture()
        {
            _possibleFrames = new List<GestureFrame>();
            _possibleFrames.Add(_possibleGestures[_curGesture].GetGestureFrame());
            _onUpdate.AddListener(GoByOneGesture);
        }
        private int RecognizeFrame(in float rotQuality, in float posQuality)
        {
            for(int i = 0; i < _possibleFrames.Count; i++)
            {
                if (!_rig.hands.IsRecognized)
                    return -1;
                
                if (RecognizeHand(_possibleFrames[i].Hands.LeftBones, _rig.hands.leftHand.points,  rotQuality, posQuality)
                    && RecognizeHand(_possibleFrames[i].Hands.RightBones, _rig.hands.rightHand.points, rotQuality, posQuality))
                 {
                     return i;
                 }
            }
            return -1;
        }

        private int RecognizeFrameПриблизительно() => RecognizeFrame(rotationQuality * qualityDecreaser, positionQuality * qualityDecreaser);
        private bool RecognizeHand(in BonesData bonesData, in Transform[] handSkeleton, in float rotQuality, in float posQuality)
        {
            if (bonesData.rotations?.Length != handSkeleton.Length)
                return true;


            if (OptimizedDistance(bonesData.rootPos, handSkeleton[0].localPosition) > posQuality)
                return false;
            
            for (int i = 0; i < bonesData.rotations.Length; i++)
            {

                float distance = OptimizedDistance( bonesData.rotations[i], handSkeleton[i].localRotation);
              
                if (distance > rotQuality)
                {
                    //Debug.Log($"{handSkeleton[i].localRotation.eulerAngles} - hand, {bonesData.rotations[i].eulerAngles} - bd, {i} - id");
                    return false;
                }
            }
            return true;
        }
        
        public void HideHands()
        {
            Debug.Log("Hide Hands");
            if(_rig.hands.haveCreator)
                _rig.hands.handCreator.HideHands();
            wasDrawnПриблизительно = false;
        }

        private void LogPossibleFrames()
        {
            string log = "Try to detect: ";
            foreach (var frame in _possibleFrames)
            {
                log += frame.name + ", ";
            }
            Debug.Log(log);
        }
        public static float OptimizedDistance(in Vector3 a, in Vector3 b) =>
            (a.x - b.x) * (a.x - b.x) + (a.y - b.y)* (a.y - b.y) + (a.z - b.z) * (a.z - b.z);
        public static float OptimizedDistance(in Vector4 a, in Vector4 b) =>
            (a.x - b.x) * (a.x - b.x) + (a.y - b.y)* (a.y - b.y) + (a.z - b.z) * (a.z - b.z) + (a.w - b.w) * (a.w - b.w);
        public static float OptimizedDistance(in Quaternion a, in Quaternion b) =>
            OptimizedDistance(a.eulerAngles, b.eulerAngles);

        public static float OptimizedDistance(in Color a, in Color b) =>
            OptimizedDistance(new Vector4(a.r, a.g, a.b, a.a), new Vector4(b.r, b.g, b.b, b.a));
    }
}