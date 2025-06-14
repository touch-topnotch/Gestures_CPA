using UnityEditor;
using UnityEngine;

namespace Scripts.Adapters
{ 
    [CreateAssetMenu(fileName = "_BoneAdaptationData", menuName = "XR/BoneAdaptationData")]
    public class BoneAdaptationData: ScriptableObject
    {
        public AdaptedBone[] adaptedBones;
    }
}