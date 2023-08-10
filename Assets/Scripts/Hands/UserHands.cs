using System;
using Scripts.Gestures;
using UnityEngine;

namespace Scripts.Hands
{
    public abstract class UserHands: MonoBehaviour
    {
        public HandSkeleton LeftSkeleton;
        public HandSkeleton RightSkeleton;
        public bool IsRecognized { get; private set; }


        public virtual void Initialize()
        {
            
        }

        public void HandEnabled() => IsRecognized = true;

        public void HandDisabled() => IsRecognized = false;
    }
}