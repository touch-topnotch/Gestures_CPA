using System;
using System.Collections;
using Scripts.Gestures;
using Scripts.Hands;
using TMPro;
using UnityEngine;
using Zenject;

namespace Scripts.PlayerLogic
{
    public class LocalPCRig : PlayerRig
    {
        [SerializeField] [Range(0.01f, 10f)] private float _delayBetweenFrames;
        [SerializeField] private TMP_InputField _inputField;
        protected GesturesLibrary _library;
        private Transform _handsParent;

        // [Inject]
        // private void Construct(GesturesLibrary library)
        // {
        private void Start()
        {
            _library = new GesturesLibrary();//library;
            _handsParent = hands.leftHand.transform.parent;
            _inputField.image.color = Color.white;
            movement.StartMove();
        }
        public void TryGetGestureFrame(string frameName)
        {
            foreach (var DyGr in _library.DynamicGestures)
            {

                if (frameName == DyGr.Name)
                {
                    _inputField.image.color = Color.green;
                    // play Dynamic Gesture
                    StartCoroutine(SimulateDynamicGesture(DyGr));
                    return;
                }
              
            }

            foreach (var GestureFrame in _library.GestureFrames)
            {
                if (frameName == GestureFrame.name)
                {
                    _inputField.image.color = Color.yellow;
                    // play Gesture Frame
                    SimulateGestures(GestureFrame);
                    return;
                }
            }

            _inputField.image.color = Color.red;
        }

        public IEnumerator SimulateDynamicGesture(DynamicGesture gestures)
        {
            var wait = new WaitForSeconds(_delayBetweenFrames);
            foreach (var frame in gestures.Frames)
            {
                //if HandMesh is PCHandMesh
                (hands.leftHand as PCHandMesh)?.SetBonesSmooth(frame.Hands.LeftBones);
                (hands.rightHand as PCHandMesh)?.SetBonesSmooth(frame.Hands.RightBones);
                
                yield return wait;
            }

            _inputField.image.color = Color.white;
        }
        public void SimulateGestures(GestureFrame frame)
        {
            (hands.leftHand as PCHandMesh)?.SetBonesSmooth(frame.Hands.LeftBones);
            (hands.rightHand as PCHandMesh)?.SetBonesSmooth(frame.Hands.RightBones);
        }

        public void ToggleParentingHands(bool toggle)
        { 
            
            hands.leftHand.transform.SetParent(toggle ? _handsParent : null);
            hands.rightHand.transform.SetParent(toggle ? _handsParent : null);
        }
        public void ChangePosOfHand()
        {
            hands.leftHand.points[0].rotation = new Quaternion(0, hands.leftHand.points[0].rotation.y +0.1f, 0, 0);
        }
    }
}