using Scripts.Events;
using UnityEngine;

namespace Scripts.Hands
{
    public class SupportHandVisualizer : MonoBehaviour
    {
        [SerializeField]
        protected BoneJoint[] joints;

        private UpdateEvent _onUpdate;

        public virtual void Initialize(ref UpdateEvent onUpdate)
        { 
            for(int i = 0; i < joints.Length; i++)
            {
                joints[i].Initialize();
            }

            _onUpdate = onUpdate;
        }
        public void ChangePosition(Vector3[] points, Transform parent = null)
        {
            if (points == null)
                return;
            for(int i = 0; i < points.Length; i++)
            {
                joints[i].SetPosition(points[i] , parent ? parent.position : Vector3.zero);
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
                joints[i].SetPositionSmooth(points[i], ref _onUpdate);
            }
        }

        public void RefreshLines()
        {
            for (int i = 0; i < joints.Length; i++)
            {
                joints[i].UpdateLine();
            }
        }

        public Transform[] GetBonesTransforms()
        {
            Transform[] transforms = new Transform[26];
            for (int i = 0; i < joints.Length; i++)
            {
                transforms[i] = joints[i].transform;
            }

            return transforms;
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