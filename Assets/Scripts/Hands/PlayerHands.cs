using System;
using Scripts.Gestures;
using UnityEngine;

namespace Scripts.Hands
{
    
    public class PlayerHands : MonoBehaviour
    {
        public HandMesh leftHand;
        public HandMesh rightHand;
        public bool IsRecognized { get; private set; }
        public void HandEnabled() => IsRecognized = true;

        public void HandDisabled() => IsRecognized = false;

        public void SetSameColor(string shader_name, Color color)
        {
            leftHand.material.SetColor(shader_name, color);
            rightHand.material.SetColor(shader_name, color);
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