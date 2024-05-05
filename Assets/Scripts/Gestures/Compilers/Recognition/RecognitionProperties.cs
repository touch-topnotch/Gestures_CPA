using System;
using UnityEngine;

namespace Scripts.Gestures
{
    [Serializable]
    public struct RecognitionProperties
    {
        [Range(0.0001f, 0.9999f)]
        public float positionQuality; // 1 - tutelka v tutelky, 0 - authomaticaly recongize
        [Range(0.0001f, 0.9999f)]
        public float rotationQuality; // 1 - tutelka v tutelky, 0 - authomaticaly recongize
        [Range(0.0001f, 0.9999f)] 
        public float rootRotationQuality; // 1 - tutelka v tutelky, 0 - authomaticaly recongize
    } 
}