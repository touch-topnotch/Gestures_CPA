using System;
using Gesture_Editor_SDK.ReadOnly;
using Scripts.Events;
using Scripts.Gestures;
using UnityEngine;

namespace Scripts.Hands
{
    [Serializable]
    public struct HandsInformation
    {
        [Serializable]
        public struct HandInformation
        {
            [ReadOnlyInInspector]
            public Vector3 rootPosition;
            [ReadOnlyInInspector]
            public Quaternion rootRotation;
        }

        public HandInformation left;
        public HandInformation right;
        public FrameRecognized OnFrameRecognized;
    }
    public class Hands: MonoBehaviour
    {
        public HandMesh leftHand;
        public HandMesh rightHand;
        
     
        public bool IsRecognized { get; private set; }
        public void OnEnabled() => IsRecognized = true;
        public void OnDisabled() => IsRecognized = false;
        
        public void SetSameColor(string param, Color color)
        {
            leftHand.material.SetColor(param, color);
            rightHand.material.SetColor(param, color);
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

        public void MoveHands(in string frameName, in float speed, in Action onPlaced)
        {
            if(GesturesLibrary.Instance.TryGetGestureFrame(frameName, out var frame))
                MoveHands(frame,speed, onPlaced);
        }

        public void MoveHands(in GestureFrame frame, in float speed, in Action onPlaced)
        {
            switch (frame.Hands.HandUsed)
            {
                case HandUsedType.LEFT:
                    leftHand.ChangePositionSmooth(frame.Hands.LeftBones, speed, onPlaced);
                    break;
                case HandUsedType.RIGHT:
                    rightHand.ChangePositionSmooth(frame.Hands.RightBones, speed, onPlaced);
                    break;
                case HandUsedType.LEFTNRIGHT:
                    leftHand.ChangePositionSmooth(frame.Hands.LeftBones, speed, onPlaced);
                    rightHand.ChangePositionSmooth(frame.Hands.RightBones, speed);
                    break;
                case HandUsedType.NULL:
                    Debug.LogError("Gesture: " + frame.name + " doesn't contains bones!");
                    break;
            }
        }
    }
}