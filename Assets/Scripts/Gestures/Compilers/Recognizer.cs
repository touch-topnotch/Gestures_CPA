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
        [Range(0, 1)] public float positionQuality = 0.01f;
        [Range(0, 0.2f)] public float rotationQuality = 0.1f;
        public int qualityDecreaser = 10;
        private UpdateEvent _onUpdate;
        [SerializeField]
        private PlayerRig _rig;
        
        private RecognitionEvent _onRecognized;
        private List<GestureFrame> _possibleFrames;
        private List<DynamicGesture> _possibleGestures;
        
        private int _curGesture;
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
                RecognizeInOneGesture();
            }
        }

        private void DrawПриблизетльныйGesture()
        {
            var приблизительныйFrameId = RecognizeFrameПриблизительно();
            if (приблизительныйFrameId != -1)
            {
              //  _player.ownUser.bodyParts.Hands.OverrideHands(_possibleFrames[приблизительныйFrameId].Hands); // fix
                l.rl("рисую приблизительный " + _possibleFrames[приблизительныйFrameId].name);
            }
        }
        private void GoByOneGesture()
        {
            DrawПриблизетльныйGesture();
            var frameId = RecognizeFrame(rotationQuality, positionQuality);
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
        private int RecognizeFrame(in float rotQuality, in float posQuality)
        {
            for(int i = 0; i < _possibleFrames.Count; i++)
            {
                if (!_rig.hands.IsRecognized)
                {
                    return -1;
                }
                
                // FIX - equal rotations and rootPose
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
            if (bonesData == null)
            {
                return true;
            }

            if (OptimizedDistance(bonesData.rootPos, handSkeleton[0].localPosition) > posQuality)
            {
                Debug.Log("Gesture position too far of hand");
                return false;
            }
            
            for (int i = 0; i < bonesData.rotations.Length; i++)
            {

                float distance = OptimizedDistance( bonesData.rotations[i], handSkeleton[i].rotation);
                
                if (distance > rotQuality)
                {
                    return false;
                }
            }
            return true;
        }

        public void HideHands()
        {
           // _player.ownUser.bodyParts.LeftHand.  //fix
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

        private float OptimizedDistance(in Quaternion a, in Quaternion b) =>
            (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z) +
            (a.w - b.w) * (a.w - b.w);
    }
}