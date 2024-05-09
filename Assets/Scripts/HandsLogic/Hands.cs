using System;
using Gesture_Editor_SDK.ReadOnly;
using Scripts.Design;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using Scripts.Systems;
using UnityEngine;

namespace Scripts.HandsLogic
{
    public class Hands : MonoBehaviour
    {
        public HandMesh leftHand;
        public HandMesh rightHand;

        private MaterialPair _handMaterialPair;
        private bool _isSync;
        private Action _onPlaced;

        public MaterialPair HandMaterialPair
        {
            get
            {
                if (!_handMaterialPair.Left || !_handMaterialPair.Right)
                {
                    _handMaterialPair.Left = leftHand.HandMaterial;
                    _handMaterialPair.Right = rightHand.HandMaterial;
                }

                return _handMaterialPair;
            }
            set
            {
                Debug.Log("Setting left mat: " + value.Left.name + ", right mat: " + value.Right.name + " to " +
                          transform.parent.parent.name);
                leftHand.HandMaterial = value.Left;
                rightHand.HandMaterial = value.Right;
            }
        }

        public bool IsRecognized { get; private set; }
        public void OnEnabled() => IsRecognized = true;
        public void OnDisabled() => IsRecognized = false;

        public void SetSameColor(string param, Color color)
        {
            // leftHand.material.SetColor(param, color);
            // rightHand.material.SetColor(param, color);
        }

        public void RecoverLeftHand(in HandAnchor anchor)
        {
            leftHand.points[0].position = anchor.rootPos;

            leftHand.SetRotations(anchor.rotations);
        }

        public void RecoverRightHand(in HandAnchor anchor)
        {
            rightHand.points[0].position = anchor.rootPos;
            rightHand.SetRotations(anchor.rotations);
        }

        // public void MoveHands(in string frameName, in float speed, in Action onPlaced)
        // {
        //     if(GesturesLibrary.Instance.TryGetGestureFrame(frameName, out var frame))
        //         MoveHands(frame,speed, onPlaced);
        // }

        public void SyncHands()
        {
            _isSync = !_isSync;
            if (_isSync)
            {
                _onPlaced?.Invoke();
            }
        }

        public void MoveHands(in GestureFrame frame, in BodyAnchors anchors, float speed, Action onPlaced,
            bool changePosition)
        {
            frame.Hands.LeftBones?.ListenAnchors(anchors);
            frame.Hands.RightBones?.ListenAnchors(anchors);
            switch (frame.Hands.HandUsed)
            {
                case HandUsedType.LEFT:
                    leftHand.Move(frame.Hands.LeftBones, speed, onPlaced, changePosition);
                    break;
                case HandUsedType.RIGHT:
                    rightHand.Move(frame.Hands.RightBones, speed, onPlaced, changePosition);
                    break;
                case HandUsedType.LEFTNRIGHT:
                    _onPlaced = onPlaced;
                    _isSync = true; //  0 hands - true, 1 hand - false, 2 hands - true. Короче это так работает, забей
                    leftHand.Move(frame.Hands.LeftBones, speed, SyncHands, changePosition);
                    rightHand.Move(frame.Hands.RightBones, speed, SyncHands, changePosition);
                    break;
                case HandUsedType.NULL:
                    Debug.LogError("Gesture: " + frame.name + " doesn't contains bones!");
                    break;
            }
        }
    }
}