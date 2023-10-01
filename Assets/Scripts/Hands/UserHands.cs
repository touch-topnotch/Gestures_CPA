using System;
using Scripts.Gestures;
using UnityEngine;

namespace Scripts.Hands
{
    public abstract class UserHands: MonoBehaviour
    {
        public HandMesh leftHand;
        public HandMesh rightHand;
        public bool IsRecognized { get; private set; }


        public virtual void Initialize()
        {
            
        }

        public void HandEnabled() => IsRecognized = true;

        public void HandDisabled() => IsRecognized = false;

        public void SetSameColor(string name, Color color)
        {
            leftHand.material.SetColor(name, color);
            rightHand.material.SetColor(name, color);
        }
    }
}