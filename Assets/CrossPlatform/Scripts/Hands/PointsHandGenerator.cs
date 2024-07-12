using UnityEngine;

namespace CrossPlatform.Gestures
{
    public class PointsHandGenerator : MonoBehaviour
    {
        [SerializeField]
        private BoneJoint[] leftJoints;
        
        [SerializeField]
        private BoneJoint[] rightJoints;
        
        [SerializeField]
        private Material leftPointsMaterial;
        
        [SerializeField]
        private Material rightPointsMaterial;

        public virtual void Initialize()
        { 
            for(int i = 0; i < leftJoints.Length; i++)
            {
                leftJoints[i].Initialize();
                rightJoints[i].Initialize();
            }
        }
        public virtual void ReplaceLeft(Vector3[] leftPoints, Color color)
        {
            if (leftPoints == null)
                return;
            for(int i = 0; i < leftPoints.Length; i++)
            {
                leftJoints[i].SetPosition(leftPoints[i]);
            }
            leftPointsMaterial.color = color;
        }
        public virtual void ReplaceRight(Vector3[] rightPoints, Color color)
        {
            if (rightPoints == null)
                return;
            for(int i = 0; i < rightPoints.Length; i++)
            {
                rightJoints[i].SetPosition(rightPoints[i]);
            }
            rightPointsMaterial.color = color;
        }
    }
}