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
        private Rig _rig;
        private RecognitionPropertiesConfig rigConfig => _rig.RecognitionProperties;
        private RecognitionEvent _onRecognized;
        private UpdateEvent _onUpdate;
        
        private List<GestureFrame> _possibleFrames;
        private List<DynamicGesture> _possibleGestures;
        
        private int _curGesture;
        private bool wasDrawnПриблизительно = false;
        
        [Inject]
        public void Initialize(UpdateEvent onUpdate, Rig rig)
        {
            _rig = rig;
            _onUpdate = onUpdate;
        }
        
        public void RecognizeDynamicGesture(Dictionary<string, DynamicGesture> possibleGestures, ref RecognitionEvent onRecognized)
        {
            
            Debug.Log("Start to recognize dynamic gesture...");
           
            _onRecognized = onRecognized;
            _possibleGestures = new List<DynamicGesture>(possibleGestures.Values);
            _possibleFrames = new List<GestureFrame>();
            for (int i = 0; i < _possibleGestures.Count; i++)
            {
                _possibleFrames.Add(_possibleGestures[i].GetGestureFrame());
                //possibleGestures[i].LogFrames();
            }
            LogPossibleFrames();
            
            _onUpdate.AddListener(FindStartOfDynamicGesture);
        }
        private void FindStartOfDynamicGesture()
        {
            
            DrawПриблизетльныйGesture();

            var frameId = RecognizeFrame(rigConfig.PlayerProperties);
            if (frameId != -1)
            {     
                _onUpdate.RemoveListener(FindStartOfDynamicGesture);
                _curGesture = frameId; 
                _possibleGestures[_curGesture].FrameRecognized();
                
               
                HideHands();
                RecognizeInOneGesture();
                
            }
        }

        private void DrawПриблизетльныйGesture()
        {
            if (wasDrawnПриблизительно)
                return;

            var приблизительныйFrameId = RecognizeFrame(rigConfig.SupportiveProperties);
            if (приблизительныйFrameId != -1)
            {
                _rig.GetHands.handVisualiser.OverrideHands(_possibleFrames[приблизительныйFrameId].Hands);

                foreach (var hand in _rig.GetHands.handVisualiser.activeHands)
                {
                    hand.ChangeColorPinPong(HandShaderProps.EdgeColor, new Color(1,1,1,0.1f), new Color(1,1,1,0.5f), 2);
                }

                wasDrawnПриблизительно = true;
                l.rl("рисую приблизительный " + _possibleFrames[приблизительныйFrameId].name);
            }
        }
        private void GoByOneGesture()
        {
            
            DrawПриблизетльныйGesture();
            var frameId = RecognizeFrame(rigConfig.PlayerProperties);
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
        private int RecognizeFrame(RecognitionProperties props)
        {
            for(int i = 0; i < _possibleFrames.Count; i++)
            {
                if (!_rig.GetHands.IsRecognized)
                    return -1;
                
                if (RecognizeHand(_possibleFrames[i].Hands.LeftBones, _rig.GetHands.leftHand.points, props)
                    && RecognizeHand(_possibleFrames[i].Hands.RightBones, _rig.GetHands.rightHand.points, props))
                 {
                     return i;
                 }
            }
            return -1;
        }
        private bool RecognizeHand(in BonesData bonesData, in Transform[] handSkeleton, in RecognitionProperties props)
        {
            if (bonesData.rotations?.Length != handSkeleton.Length)
                return true;


            var dist = OptimizedDistance(bonesData.rootPos, handSkeleton[0].localPosition);
         
            if (1 - dist < props.positionQuality)
            {// l.rl("Canceled, because position: " + dist + " > " + props.positionQuality);
                return false;
            }
            
            for (int i = 0; i < bonesData.rotations.Length; i++)
            {
                float distance = OptimizedDistance( bonesData.rotations[i], handSkeleton[i].localRotation);
              
                if (distance < props.rotationQuality) // 0 - bad, 1 - good, 0.9 - ok
                {
                    //l.rl("Canceled, because rotation: " + distance + " < " + props.rotationQuality);
                    return false;
                }
            }
            return true;
        }
        
        public void HideHands()
        {
            Debug.Log("Hide Hands");
             _rig.GetHands.handVisualiser.HideHands();
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
            Math.Abs(Quaternion.Dot(a, b));

        public static float OptimizedDistance(in Color a, in Color b) =>
            OptimizedDistance(new Vector4(a.r, a.g, a.b, a.a), new Vector4(b.r, b.g, b.b, b.a));
    }
}