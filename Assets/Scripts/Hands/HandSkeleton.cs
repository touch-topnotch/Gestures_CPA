using UnityEngine;

namespace Scripts.Hands
{
    public class HandSkeleton
    {
        
        private Transform[] _bones;
        private bool _isRecognized;

        public HandSkeleton(Transform[] bones)
        {
            _bones = bones;
        }
        public void HandEnabled()
        {
            _isRecognized = true;
        }

        public void HandDisabled()
        {
            _isRecognized = false;
        }

        public Vector3[] GetPositions()
        {
            TryDisabled();
            Vector3 [] bonesPosition = new Vector3[_bones.Length];
            for(int i = 0; i < _bones.Length; i++)
            {
                bonesPosition[i] = _bones[i].position;
            }
            
            return bonesPosition;
        }

        public Transform[] GetTransforms()
        {
            TryDisabled();
            return _bones;
        }

        private void TryDisabled()
        {
            if (!_isRecognized)
            {
                Debug.LogError("Skeleton is disabled!");
            }
        }
    }
}