using System;
using UnityEngine;

namespace Scripts.Hands
{
    [Serializable]
    public class XRUserHands: UserHands
    {
        [SerializeField] private Transform[] leftPoints;
        [SerializeField] private Transform[] rightPoints;
        public override void Initialize()
        {
            LeftSkeleton = new HandSkeleton(leftPoints);
            RightSkeleton = new HandSkeleton(rightPoints);
        }
    }
}