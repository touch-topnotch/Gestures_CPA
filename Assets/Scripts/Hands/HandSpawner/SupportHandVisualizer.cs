using Scripts.Events;
using UnityEngine;

namespace Scripts.Hands
{
    public class SupportHandVisualizer : MonoBehaviour, IHandVisualiser
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
        public void ChangePosition(BonesData data, Transform parent = null)
        {
            if (data == null)
                return;
            joints[0].SetPosition(data.RootPos, parent ? parent.position : Vector3.zero);
            for(int i = 0; i < data.Rotations.Length; i++)
            {
                joints[i].SetPosition(data.Positions[i] , parent ? parent.position : Vector3.zero);
            }
        }



        public void ChangePositionSmooth(BonesData data)
        {
            if (data == null)
            {
                return;
            }

            for (int i = 0; i < data.Positions.Length; i++)
            {
                joints[i].SetPositionSmooth(data.Positions[i], data.Rotations[i], ref _onUpdate);
            }
        }

        public void RefreshLines()
        {
            for (int i = 0; i < joints.Length; i++)
            {
                joints[i].UpdateLine();
            }
        }

        public Transform[] GetTransforms()
        {
            Transform[] transforms = new Transform[26];
            for (int i = 0; i < joints.Length; i++)
            {
                transforms[i] = joints[i].transform;
            }

            return transforms;
        }


        public void ChangePosition()
        {
            throw new System.NotImplementedException();
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