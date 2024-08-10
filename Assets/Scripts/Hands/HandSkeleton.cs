using UnityEngine;

namespace Scripts.Hands
{
    public class HandSkeleton
    {
        private Transform[] _bones;

        public HandSkeleton(Transform[] bones)
        {
            _bones = bones;
        }

        public Vector3[] GetPositions()
        {
            Vector3 [] bonesPosition = new Vector3[_bones.Length];
            for(int i = 0; i < _bones.Length; i++)
            {
                bonesPosition[i] = _bones[i].position;
            }
            
            return bonesPosition;
        }

        public Transform[] GetTransforms() => _bones;


    }
}