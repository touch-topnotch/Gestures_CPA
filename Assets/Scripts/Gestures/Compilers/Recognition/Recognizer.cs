using System;
using System.Collections.Generic;
using Scripts.Design;
using Scripts.Events;
using Scripts.HandsLogic;
using UnityEngine;

namespace Scripts.Gestures
{
    public class Recognizer
    {
        public readonly GestureRecognized onGestureRecognized;
        public readonly FrameRecognized onFrameRecognized;
        
        private List<GestureFrame> _possibleFrames;
        private List<DynamicGesture> _possibleGestures;
        
        private readonly PlayerHands _hands;
        private readonly RecognitionPropertiesConfig _config;
        private readonly UpdateEvent _onUpdate;

        private readonly GesturesLibrary _library;

        private int _curGesture = 0;
        private int _curFrameId = 0;
        private bool wasDrawnNearly = false;
        

        public Recognizer(PlayerHands hands, RecognitionPropertiesConfig config)
        {
            _hands = hands;
            _config = config;
            _onUpdate = UpdateEvent.Instance;

            onFrameRecognized = new FrameRecognized();
            
            onGestureRecognized = new GestureRecognized();

            onFrameRecognized.AddListener(FrameLog);
            onFrameRecognized.AddListener((name)=>
            {
                _possibleGestures[_curGesture].FrameRecognized(name);
            });
            
            onGestureRecognized.AddListener(GestureLog);
        }

        private void FrameLog(string name)
        {
            Debug.Log("Frame " + name + " recognized");
        }
        private void GestureLog(string name)
        {
            Debug.Log("Dynamic Gesture " + name + " recognized");
        }

        public bool RecognizeFrame(RecognitionProperties properties, GestureFrame frame)
        {

            if (!_hands.IsRecognized)
                return false;
            
            if (RecognizeHand(frame.Hands.LeftBones, _hands.leftHand.points, properties)
                && RecognizeHand(frame.Hands.RightBones, _hands.rightHand.points, properties))
            {
                return true;
            }
            
            return false;
        }
        
        public void RecognizeDynamicGesture(Dictionary<string, DynamicGesture> possibleGestures)
        {
        
            _possibleGestures = new List<DynamicGesture>(possibleGestures.Values);
            _possibleFrames = new List<GestureFrame>();
            
            _curGesture = 0;
            _curFrameId = 0;
            
            for (int i = 0; i < _possibleGestures.Count; i++)
            {
                _possibleFrames.Add(_possibleGestures[i].frames[0]);
                //possibleGestures[i].LogFrames();
            }
            
            LogPossibleFrames();
            
            _onUpdate.AddListener(FindStartOfDynamicGesture);
        }
        private void FindStartOfDynamicGesture()
        {
            
            DrawNearlyGesture();

            var frameId = RecognizeFrame(_config.PlayerProperties);
            if (frameId != -1)
            {     
                _onUpdate.RemoveListener(FindStartOfDynamicGesture);
                _curGesture = frameId;
                _curFrameId++;
                onFrameRecognized?.Invoke(_possibleGestures[_curGesture].frames[0].name);

                HideHands();
                RecognizeInOneGesture();
                
            }
        }

        private void DrawNearlyGesture()
        {
            if (wasDrawnNearly)
                return;

            var NearlyFrameId = RecognizeFrame(_config.SupportiveProperties);
            if (NearlyFrameId != -1)
            {
                _hands.handVisualiser.OverrideHands(_possibleFrames[NearlyFrameId].Hands);

                foreach (var hand in _hands.handVisualiser.activeHands)
                {
                    hand.ChangeColorPinPong(HandShaderProps.EdgeColor, new Color(1,1,1,0.1f), new Color(1,1,1,0.5f), 2);
                }

                wasDrawnNearly = true;
            }
        }
        private void GoByOneGesture()
        {
            
            DrawNearlyGesture();
            
            var frameId = RecognizeFrame(_config.PlayerProperties);
            
            if (frameId != -1)
            {
                HideHands();
                
                onFrameRecognized?.Invoke(_possibleGestures[_curGesture].frames[_curFrameId].name);
                _curFrameId++;
                
                if (_curFrameId < _possibleGestures[_curGesture].frames.Count)  // all possible gestures = one gesture (list of one element);
                {
                    _possibleFrames[0] = _possibleGestures[_curGesture].frames[_curFrameId];
                    return;
                }
                
                
                _onUpdate.RemoveListener(GoByOneGesture);
                
                onGestureRecognized.Invoke(_possibleGestures[_curGesture].Name);
            }
        }
        
        private void RecognizeInOneGesture()
        {
            _possibleFrames = new List<GestureFrame> { _possibleGestures[_curGesture].frames[_curFrameId] };
            _onUpdate.AddListener(GoByOneGesture);
        }
        
        private int RecognizeFrame(RecognitionProperties props)
        {
            for(int i = 0; i < _possibleFrames.Count; i++)
            {
                if (RecognizeFrame(props, _possibleFrames[i]))
                    return i;
            }
            return -1;
        }
        
        
        private static bool RecognizeHand(in BonesData bonesData, in Transform[] handSkeleton, in RecognitionProperties props)
        {
            if (bonesData == null || bonesData.rotations?.Length != handSkeleton.Length)
                return true;


            var dist = OptimizedDistance(bonesData.rootPos, handSkeleton[0].localPosition);
         
            if (1 - dist < props.positionQuality)
            {// l.rl("Canceled, because position: " + dist + " > " + props.positionQuality);
                return false;
            }
            
            for (int i = 0; i < bonesData.rotations.Length; i++)
            {
                float distance = OptimizedDistance( bonesData.rotations[i], handSkeleton[i].localRotation);
                var quality = i == 0 ? props.rootRotationQuality : props.rotationQuality;
                if (distance < quality) // 0 - bad, 1 - good, 0.9 - ok
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
             _hands.handVisualiser.HideHands();
            wasDrawnNearly = false;
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