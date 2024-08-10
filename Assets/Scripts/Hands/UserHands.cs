using System;
using UnityEngine;

namespace Scripts.Hands
{
    public abstract class UserHands: MonoBehaviour
    {
        public HandSkeleton LeftSkeleton;
        public HandSkeleton RightSkeleton;

        public virtual void Initialize()
        {
            
        }
    }
}