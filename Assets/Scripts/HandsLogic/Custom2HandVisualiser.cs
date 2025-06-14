using System;
using Scripts.Adapters;
using Scripts.Components;
using Scripts.PlayerLogic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Scripts.HandsLogic
{
    [RequireComponent(typeof(HandMesh), typeof(HandAdapter))]
    public class Custom2HandVisualiser: RigComponent
    {

        [SerializeField] private HandMesh m_HandMesh;
        [SerializeField] private HandAdapter m_HandAdapter;
        
        [SerializeField] UnityEvent m_OnEnabled;
        [SerializeField] UnityEvent m_OnDisabled;
        
        protected override bool shouldAddMissingComponents => !(m_HandMesh && m_HandAdapter);
        public bool isEnabled { get; private set; }

        public OVRSkeleton testSkeleton;
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
            UpdateRenderingVisibility(m_HandAdapter.isTracked);
           
        }


        private float t = 1;
        protected void FixedUpdate()
        {
            
            UpdateRenderingVisibility(m_HandAdapter.isTracked);
            if (!m_HandAdapter.isTracked)
                return;
          
            for (int i = 0; i < m_HandAdapter.rotations.Length; i ++)
            {
                m_HandMesh.points[i].localRotation = m_HandAdapter.rotations[i];
            }

            m_HandMesh.points[0].localPosition = m_HandAdapter.rootPos;
        }
    }
}