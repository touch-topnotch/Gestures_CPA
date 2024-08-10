using System.Collections.Generic;
using Scripts.Events;
using Scripts.Hands;
using Scripts.PlayerLogic;
using UnityEngine;
using Zenject;

namespace Scripts.Gestures
{
    public class Recognizer: MonoBehaviour
    {
        [Range(0, 1)] public float handPoseOffset = 0.01f;
        [Range(0, 1)] public float quality = 0.1f;
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
            }
            _onUpdate.AddListener(FindStartOfDynamicGesture);
        }
        private void FindStartOfDynamicGesture()
        {
            var frameId = RecognizeFrame();
            if (frameId != -1)
            {     
                _curGesture = frameId;
                _possibleGestures[_curGesture].FrameRecognized();
                _onUpdate.RemoveListener(FindStartOfDynamicGesture);
                RecognizeInOneGesture();
            }
        }
        private void GoByOneGesture()
        {
            
            var frameId = RecognizeFrame();
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
        private int RecognizeFrame()
        {
            print("Try to detect " + _possibleFrames[0].name);
            for(int i = 0; i < _possibleFrames.Count; i++)
            {
                if (RecognizeHand(_possibleFrames[i].Hands.LeftPoints, _player.playerHands.LeftSkeleton.GetTransforms(), _player.transform)
                    && RecognizeHand(_possibleFrames[i].Hands.RightPoints, _player.playerHands.RightSkeleton.GetTransforms(),_player.transform))
                {
                    return i;
                }
            }
            return -1;
        }
        private bool RecognizeHand(in Vector3[] gesturePoints, in Transform[] handSkeleton, in Transform playerTransform)
        {
            if (gesturePoints == null)
            {
                return true;
            }
            if (OptimizedDistance(gesturePoints[0], handSkeleton[0].localPosition) > handPoseOffset)
            {
                return false;
            }
            
            for (int i = 0; i < gesturePoints.Length; i++)
            {

                Vector3 curPosition = playerTransform.InverseTransformPoint(handSkeleton[i].position);
                float distance = OptimizedDistance( gesturePoints[i], curPosition);
                
                if (distance > quality)
                {
                    return false;
                }
            }
            return true;
        }
        private float OptimizedDistance(in Vector3 a, in Vector3 b) =>
            (a.x - b.x) * (a.x - b.x) + (a.y - b.y)* (a.y - b.y) + (a.z - b.z) * (a.z - b.z);
    }
}