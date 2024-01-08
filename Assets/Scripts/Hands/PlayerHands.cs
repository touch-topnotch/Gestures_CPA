using System;
using Scripts.Gestures;
using UnityEngine;

namespace Scripts.Hands
{

    
    public class PlayerHands : MonoBehaviour
    {
        public HandMesh leftHand;
        public HandMesh rightHand;
        
        public SupportHandVisualiser handVisualiser;
        public bool IsRecognized { get; private set; }
        public void OnEnabled() => IsRecognized = true;
        public void OnDisabled() => IsRecognized = false;
        
        private void OnValidate()
        {
            if (handVisualiser == null &&GetComponent<SupportHandVisualiser>())
            {
                handVisualiser = GetComponent<SupportHandVisualiser>();
            }
        }
        
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