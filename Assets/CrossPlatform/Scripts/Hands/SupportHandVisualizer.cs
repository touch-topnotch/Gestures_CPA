using System;
using CrossPlatform.Scripts;
using UnityEngine;

namespace CrossPlatform.Gestures
{
    public class SupportHandVisualizer : MonoBehaviour
    {
        [SerializeField]
        protected BoneJoint[] joints;
        public virtual void Initialize()
        { 
            for(int i = 0; i < joints.Length; i++)
            {
                joints[i].Initialize();
            }
        }
        public virtual void ChangePosition(Vector3[] points)
        {
            if (points == null)
                return;
            for(int i = 0; i < points.Length; i++)
            {
                joints[i].SetPosition(points[i]);
            }
        }

        public void ChangePositionSmooth(Vector3[] points)
        {
            if (points == null)
            {
                return;
            }
            for (int i = 0; i < points.Length; i++)
            { 
                joints[i].SetPositionSmooth(points[i]);
            }
        }


        public virtual void Show()
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
        
    }
}