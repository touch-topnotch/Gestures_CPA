using System;
using UnityEngine;

namespace Scripts.Gestures
{
    [Serializable]
   // [CreateAssetMenu(fileName = "RecognizerProperties_", menuName = "Config/RecognizerProperties")]
    public struct RecognizerProperties//: ScriptableObject
    {
        [Range(0, 1f)] public float positionQuality; // 1 - tutelka v tutelky, 0 - authomaticaly recongize
        [Range(0, 1f)] public float rotationQuality; // 1 - tutelka v tutelky, 0 - authomaticaly recongize
    } 
}