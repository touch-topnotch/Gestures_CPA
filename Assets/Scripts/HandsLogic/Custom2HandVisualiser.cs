using System;
using Scripts.Adapters;
using Scripts.Components;
using Scripts.PlayerLogic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Scripts.HandsLogic
{
    public class Custom2HandVisualiser: RigComponent
    {

        [SerializeField] private HandMesh m_HandMesh;
        [SerializeField] private HandAdapter m_HandAdapter;
        
        [SerializeField] UnityEvent m_OnEnabled;
        [SerializeField] UnityEvent m_OnDisabled;
        
        protected override bool shouldAddMissingComponents => !(m_HandMesh && m_HandAdapter);
        public bool isEnabled { get; private set; }

        public OVRSkeleton testSkeleton;
        private void Initialize()
        {

            var _skinnedMeshRenderer = m_HandMesh._meshRenderer;
            if ((testSkeleton != null && testSkeleton.Bones.Count > 0))
            {
                int numSkinnableBones = testSkeleton.GetCurrentNumSkinnableBones();
                var bindPoses = new Matrix4x4[numSkinnableBones];
                var bones = new Transform[numSkinnableBones];
                var localToWorldMatrix = transform.localToWorldMatrix;
                for (int i = 0; i < numSkinnableBones && i < testSkeleton.Bones.Count; ++i)
                {
                    bones[i] = testSkeleton.Bones[i].Transform;
                    bindPoses[i] = testSkeleton.BindPoses[i].Transform.worldToLocalMatrix * localToWorldMatrix;
                }

                _skinnedMeshRenderer.sharedMesh.bindposes = bindPoses;
                _skinnedMeshRenderer.bones = bones;
                _skinnedMeshRenderer.updateWhenOffscreen = true;
            }
        }
        void UpdateRenderingVisibility(bool isTracked)
        {
            if (isTracked == isEnabled)
                return;
            
            if(isTracked)
            {
                m_OnEnabled?.Invoke();
            }
            else
            {
                m_OnDisabled?.Invoke();
            }
            
            //m_HandMesh.ToggleMesh(isTracked);
            
            isEnabled = isTracked;
        }

        private void Start()
        {
           // UpdateRenderingVisibility(m_HandAdapter.isTracked);
            Initialize();
        }


        private float t = 1;
        protected void FixedUpdate()
        {
          //  UpdateRenderingVisibility(m_HandAdapter.isTracked);

            // for (int i = 0; i < m_HandAdapter.points.Length; i ++)
            // {
            //     if (m_HandAdapter.points[i] != null)
            //     {
            //         m_HandMesh.points[i].localPosition = m_HandAdapter.points[i].localPosition;
            //         m_HandMesh.points[i].localRotation = m_HandAdapter.points[i].localRotation;
            //     }
            //         
            //         
            // }
            if (t < 0)
            {
                Debug.Log(m_HandMesh.points[0].position);
                Debug.Log(m_HandMesh._meshRenderer.rootBone.position);
                t = 1;
            }

            t -= Time.deltaTime;


        }
    }
}