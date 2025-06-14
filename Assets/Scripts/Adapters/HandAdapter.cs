using System;
using JetBrains.Annotations;
using Scripts.Components;
using Scripts.HandsLogic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Adapters
{
    [Serializable]
    public class AdaptedBone
    {
        public string originName;
        public int originIndex;
        public string pluginName;
        public int pluginIndex;
        public bool existInPlugin;
        public Vector3 rotationOffset;

        public Quaternion ToXRSystem([CanBeNull] Quaternion trackedLocalRotation) => existInPlugin
            ? trackedLocalRotation * Quaternion.Euler(rotationOffset)
            : Quaternion.Euler(rotationOffset);
        
    }

    public abstract class HandAdapter : SmartComponent
    {
        [SerializeField] protected HandType handType;
        [SerializeField] protected BoneAdaptationData data; 

        protected override bool shouldAddMissingComponents => !data;
        public abstract Quaternion[] rotations { get; }
        public abstract Vector3 rootPos { get; }
        public abstract bool isTracked { get; }
        
        
        
    }
}