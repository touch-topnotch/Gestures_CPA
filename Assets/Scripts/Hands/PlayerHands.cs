using System;
using Scripts.Gestures;
using UnityEngine;

namespace Scripts.Hands
{

    public struct FingerPair
    {
        public Transform leftFinger;
        public Transform rightFinger;
    }

public class PlayerHands : MonoBehaviour
    {
        public HandMesh leftHand;
        public HandMesh rightHand;
        public HandsStruct handsStruct { get; private set;}
      
        public bool IsRecognized { get; private set; }
        public void HandEnabled() => IsRecognized = true;

        public void HandDisabled() => IsRecognized = false;

        private void OnValidate()
        {
            if (leftHand && rightHand)
            {
                handsStruct = new HandsStruct(leftHand.points, rightHand.points);
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
    }
}