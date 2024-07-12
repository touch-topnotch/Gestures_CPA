using UnityEngine;

namespace CrossPlatform.Gestures
{
    public class HandSkeleton : MonoBehaviour
    {
        
        [SerializeField] private Transform[] _bones;
        private bool _isRecognized;
        public void HandEnabled()
        {
            _isRecognized = true;
        }

        public void HandDisabled()
        {
            _isRecognized = false;
        }

        public Vector3[] GetBones()
        {
            print("FOFJFOEJOFJEOFJEOFJFO" + this.transform.name);
            if (!_isRecognized)
            {
                Debug.LogError("Skeleton is disabled!");
                return null;
            } 
            // return Vectors of bones
            Vector3 [] bonesPosition = new Vector3[_bones.Length];
            for(int i = 0; i < _bones.Length; i++)
            {
                bonesPosition[i] = _bones[i].position;
            }
            print("ну точки вроде собрались правильно");
            return bonesPosition;
        }
    }
}