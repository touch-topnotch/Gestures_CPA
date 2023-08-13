using System.Collections.Generic;
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
        [Range(0, 1)] public float handPoseOffset = 0.01f;
        [Range(0, 0.2f)] public float quality = 0.1f;
        public int qualityDecreaser = 10;
        private UpdateEvent _onUpdate;
        private Player _player;
        
        private RecognitionEvent _onRecognized;
        private List<GestureFrame> _possibleFrames;
        private List<DynamicGesture> _possibleGestures;
        
        private int _curGesture;

        [Inject]
        private void Construct(Player player, UpdateEvent onUpdate)
        {
            _player = player;
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
            var frameId = RecognizeFrame(quality, handPoseOffset);
            if (frameId != -1)
            {     
                _curGesture = frameId;
                _possibleGestures[_curGesture].FrameRecognized();
                _onUpdate.RemoveListener(FindStartOfDynamicGesture);
                RecognizeInOneGesture();
            }
        }

        private void DrawПриблизетльныйGesture()
        {
            var приблизительныйFrameId = RecognizeFrameПриблизительно();
            if (приблизительныйFrameId != -1)
            {
                _player.supHandCreator.OverrideHands(_possibleFrames[приблизительныйFrameId].Hands);
                l.rl("рисую приблизительный " + _possibleFrames[приблизительныйFrameId].name);
            }
        }
        private void GoByOneGesture()
        {
            DrawПриблизетльныйGesture();
            var frameId = RecognizeFrame(quality, handPoseOffset);
            if (frameId != -1)
            {
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
        private int RecognizeFrame(in float gQuality, in float gHandOffset)
        {
            for(int i = 0; i < _possibleFrames.Count; i++)
            {
                if (!_player.playerHands.IsRecognized)
                {
                    return -1;
                }
             
                if (RecognizeHand(_possibleFrames[i].Hands.LeftPoints, _player.playerHands.LeftSkeleton.GetTransforms(), _player.transform, gQuality, gHandOffset)
                    && RecognizeHand(_possibleFrames[i].Hands.RightPoints, _player.playerHands.RightSkeleton.GetTransforms(),_player.transform, quality, gHandOffset))
                {
                    return i;
                }
            }
            return -1;
        }

        private int RecognizeFrameПриблизительно() => RecognizeFrame(quality * qualityDecreaser, handPoseOffset * qualityDecreaser);
        private bool RecognizeHand(in Vector3[] gesturePoints, in Transform[] handSkeleton, in Transform playerTransform, in float gQuality, in float gHandOffset)
        {
            if (gesturePoints == null)
            {
                return true;
            }
            if (OptimizedDistance(gesturePoints[0], handSkeleton[0].localPosition) > gHandOffset)
            {
                return false;
            }
            
            for (int i = 0; i < gesturePoints.Length; i++)
            {

                Vector3 curPosition = playerTransform.InverseTransformPoint(handSkeleton[i].position);
                float distance = OptimizedDistance( gesturePoints[i], curPosition);
                
                if (distance > gQuality)
                {
                    return false;
                }
            }
            return true;
        }

        public void HideHands()
        {
            _player.supHandCreator.HideHands();
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
        private float OptimizedDistance(in Vector3 a, in Vector3 b) =>
            (a.x - b.x) * (a.x - b.x) + (a.y - b.y)* (a.y - b.y) + (a.z - b.z) * (a.z - b.z);
    }
}