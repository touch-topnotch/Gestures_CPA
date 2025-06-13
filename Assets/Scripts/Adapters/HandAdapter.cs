using System;
using Scripts.Components;
using Scripts.HandsLogic;
using UnityEngine;

namespace Scripts.Adapters
{
    public abstract class HandAdapter : SmartComponent
    {
        [SerializeField] protected HandType handType;
        public abstract Transform[] points { get; protected set; }
        public abstract bool isTracked { get; }
    }
}