using Scripts.Adapters;
using UnityEngine;
using Util = Scripts.HandsLogic.HandBonesUtility;

namespace Scripts.HandsLogic
{
    [RequireComponent(typeof(OVRSkeleton), typeof(OVRHand))]
    public class OculusHandAdapter : HandAdapter
    {
        
        [SerializeField] private OVRSkeleton skeleton;
        protected override bool shouldAddMissingComponents => false;

        private Quaternion[] _rotations = new Quaternion[26];
        private Vector3 _rootPos;
        public override Quaternion[] rotations => _rotations;

        public override Vector3 rootPos => _rootPos;
        public override bool isTracked => skeleton.Bones?.Count > 0;

        private void Awake()
        {
            for(int i = 0; i < _rotations.Length; i ++)
            {
                _rotations[i] = Quaternion.identity;
            }
        }

        public void FixedUpdate()
        {
            if (!isTracked || skeleton.Bones.Count < 18 )
                return;

            for (int i = 0; i < data.adaptedBones.Length; i++)
            {
                var bone = data.adaptedBones[i];
                _rotations[bone.originIndex] = Quaternion.Euler(skeleton.Bones[bone.pluginIndex].Transform.localEulerAngles + bone.rotationOffset);
            }

            _rootPos = skeleton.Bones[0].Transform.localPosition;
        }
    }
}