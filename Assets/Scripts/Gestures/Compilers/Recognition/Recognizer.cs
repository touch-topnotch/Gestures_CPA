using System;
using System.Collections.Generic;
using Scripts.Design;
using Scripts.Events;
using Scripts.HandsLogic;
using Scripts.PlayerLogic;
using Scripts.Static;
using Scripts.Systems;
using UnityEngine;

namespace Scripts.Gestures
{
    public class Recognizer
    {
        public readonly GestureRecognized onGestureRecognized;
        public readonly FrameRecognized onFrameRecognized;

        private Color _colorActive = new Color(1, 1, 1, 0.0f);
        private Color _colorPassive = new Color(1, 1, 1, 0.5f);
        private List<GestureFrame> _possibleFrames;
        private List<DynamicGesture> _possibleGestures;
        
        private readonly PlayerHands _hands;
        private readonly RecognitionPropertiesConfig _config;
        private readonly UpdateEvent _onUpdate;

        private readonly GesturesLibrary _library;
        private BodyAnchors bodyAnchors;
        private int _curGesture = 0;
        private int _curFrameId = 0;
        private bool wasDrawnNearly = false;
        public Recognizer(PlayerHands hands, RecognitionPropertiesConfig config)
        {
            Debug.Log(VectorConverter.VecToCodeRot(
                          new Vector3(8.84876633f, 359.548981f, 61.1509857f)) + "\n" +
                      VectorConverter.VecToCodeRot(new Vector3(22.8301735f, 313.677795f, 130.638489f)));
            _hands = hands;
            _config = config;
            _onUpdate = UpdateEvent.Instance;

            onFrameRecognized = new FrameRecognized();
            
            onGestureRecognized = new GestureRecognized();
            bodyAnchors = _hands.transform.parent.GetComponent<BodyAnchors>();
            onFrameRecognized.AddListener(FrameLog);
            onFrameRecognized.AddListener((name)=>
            {
                _possibleGestures[_curGesture].FrameRecognized(name);
            });
            
            onGestureRecognized.AddListener(GestureLog);
            onGestureRecognized.AddListener((s)=>
            {
                HideHands();
            });
        }

        private void FrameLog(string name)
        {
            Debug.Log("Frame " + name + " recognized");
        }
        private void GestureLog(string name)
        {
            Debug.Log("Dynamic Gesture " + name + " recognized");
        }

        public bool RecognizeFrame(RecognitionProperties properties, GestureFrame frame, bool invokeEvent, int gestureId)
        {
            //
            // if (!_hands.IsRecognized)
            //     return false;
            //
            if (RecognizeHand(frame.Hands.LeftBones, _hands.leftHand.points, properties)
                && RecognizeHand(frame.Hands.RightBones, _hands.rightHand.points, properties))
            {
                if (invokeEvent)
                {
                    _curGesture = gestureId;
                    onFrameRecognized?.Invoke(frame.name);
                }
                    
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


            var frameId = RecognizeFrame(_config.PlayerProperties, true);
            if (frameId != -1)
            {     
                _onUpdate.RemoveListener(FindStartOfDynamicGesture);
                _curGesture = frameId;
                _curFrameId++;
                HideHands();
                RecognizeInOneGesture();
                
            }
        }

        private void DrawNearlyGesture()
        {
            if (wasDrawnNearly)
                return;
      
            var NearlyFrameId = RecognizeFrame(_config.SupportiveProperties, false);
            
            if (NearlyFrameId != -1)
            {
                Debug.Log("Draw nearly" + _possibleFrames[NearlyFrameId].name);
                _hands.handVisualiser.Move(_possibleFrames[NearlyFrameId].Hands, 4, null);
                _hands.handVisualiser.ManipulateLasts((m)=>m.ChangeColorPinPong(_colorActive, _colorPassive, new ColorParams(
                    HandShaderProps.EdgeColor,
                        1, false)));

                wasDrawnNearly = true;
            }
        }
        private void GoByOneGesture()
        {
            
            DrawNearlyGesture();
            
            var frameId = RecognizeFrame(_config.PlayerProperties, true);
            
            if (frameId != -1)
            { 
                HideHands();
                
            //    onFrameRecognized?.Invoke(_possibleGestures[_curGesture].frames[_curFrameId].name);
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
        
        private int RecognizeFrame(RecognitionProperties props, bool invokeEvent)
        {
            for(int i = 0; i < _possibleFrames.Count; i++)
            {
                if (RecognizeFrame(props, _possibleFrames[i], invokeEvent, i))
                    return i;
            }
            return -1;
        }

      
        private bool RecognizeHand(in BonesData bonesData, in Transform[] handSkeleton, in RecognitionProperties props)
        {
            if (bonesData == null || bonesData.rotations?.Length != handSkeleton.Length)
                return true;
            
            bonesData.ListenAnchors(PlayerData.local.bodyAnchors);
            var dist = OptimizedDistance(bonesData.rootPos, handSkeleton[0].localPosition);
          
            if (1 - dist < props.positionQuality) 
            {
                //l.rl("Canceled, because position: " + dist + " < " + props.positionQuality);
                return false;
            }
            float distance = OptimizedDistance(bonesData.rotations[0],handSkeleton[0].localRotation);

            if (distance < props.rootRotationQuality)
            {
               // l.rl("Canceled, because root rotation: " + distance + " > " + props.rootRotationQuality);
                return false;
            }
                
   
            for (int i = 1; i < bonesData.rotations.Length; i++)
            {
                distance = OptimizedDistance( bonesData.rotations[i], handSkeleton[i].localRotation);
                var quality = props.rotationQuality;
                if (distance < quality) // 0 - bad, 1 - good, 0.9 - ok
                {
                //    l.rl("Canceled, because rotation: " + distance + " > " + props.rotationQuality);
                    return false;
                }
            }
            return true;
        }
        
        public void HideHands()
        {
            Debug.Log("Hide hands"); 
            _hands.handVisualiser.ManipulateAll((e)=>e.Hide());
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
            (float)Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z));
        public static float OptimizedDistance(in Vector4 a, in Vector4 b) =>
            (a.x - b.x) * (a.x - b.x) + (a.y - b.y)* (a.y - b.y) + (a.z - b.z) * (a.z - b.z) + (a.w - b.w) * (a.w - b.w);
        public static float OptimizedDistance(in Quaternion a, in Quaternion b) =>
            Math.Abs(Quaternion.Dot(a, b));

        public static float OptimizedDistance(in Color a, in Color b) =>
            OptimizedDistance(new Vector4(a.r, a.g, a.b, a.a), new Vector4(b.r, b.g, b.b, b.a));
    }
    
}