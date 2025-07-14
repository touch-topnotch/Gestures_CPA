using System;
using System.Collections.Generic;
using Scripts.PlayerLogic;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Hands;

namespace Scripts.HandsLogic
{
   
    public class CustomHandVisualizer : RigComponent
    {
        public enum VelocityType
        {
            Linear,
            Angular,
            None,
        }
        
        [SerializeField] [Range(0.1f, 20)] public float positionSpeed = 10f;
        public Vector3 leftPositionOffset;
        public Vector3 rightPositionOffset;
        [SerializeField]
        [Tooltip(
            "If this is enabled, this component will enable the Input System internal feature flag 'USE_OPTIMIZED_CONTROLS'. You must have at least version 1.5.0 of the Input System and have its backend enabled for this to take effect.")]
        bool m_UseOptimizedControls;


        [SerializeField] private PlayerHands m_PlayerHands;

        public bool drawMeshes
        {
            get => m_DrawMeshes;
            set => m_DrawMeshes = value;
        }

        [SerializeField] bool m_DrawMeshes;
        bool m_PreviousDrawMeshes;

        [SerializeField] GameObject m_DebugDrawPrefab;

        public bool debugDrawJoints
        {
            get => m_DebugDrawJoints;
            set => m_DebugDrawJoints = value;
        }

        [SerializeField] bool m_DebugDrawJoints;
        bool m_PreviousDebugDrawJoints;

        [SerializeField] GameObject m_VelocityPrefab;

        public VelocityType velocityType
        {
            get => m_VelocityType;
            set => m_VelocityType = value;
        }

        [SerializeField] VelocityType m_VelocityType;
        VelocityType m_PreviousVelocityType;

        [SerializeField] UnityEvent m_OnEnabled;
        [SerializeField] UnityEvent m_OnDisabled;

        XRHandSubsystem m_Subsystem;
        HandGameObjects m_LeftHandGameObjects;
        HandGameObjects m_RightHandGameObjects;

        static readonly List<XRHandSubsystem> s_SubsystemsReuse = new List<XRHandSubsystem>();

        protected void Awake()
        {
#if ENABLE_INPUT_SYSTEM
            if (m_UseOptimizedControls)
                InputSystem.settings.SetInternalFeatureFlag("USE_OPTIMIZED_CONTROLS", true);
#endif // ENABLE_INPUT_SYSTEM
        }


        protected void OnEnable()
        {
            if (m_Subsystem == null)
                return;

            UpdateRenderingVisibility(m_LeftHandGameObjects, m_Subsystem.leftHand.isTracked);
            UpdateRenderingVisibility(m_RightHandGameObjects, m_Subsystem.rightHand.isTracked);
        }

        protected void OnDisable()
        {
            if (m_Subsystem != null)
            {
                m_Subsystem.trackingAcquired -= OnTrackingAcquired;
                m_Subsystem.trackingLost -= OnTrackingLost;
                m_Subsystem.updatedHands -= OnUpdatedHands;
                m_Subsystem = null;
            }

            UpdateRenderingVisibility(m_LeftHandGameObjects, false);
            UpdateRenderingVisibility(m_RightHandGameObjects, false);
        }

        protected void OnDestroy()
        {
            m_LeftHandGameObjects = null; 
            m_RightHandGameObjects = null;
        }

        protected void FixedUpdate()
        {
            if (m_Subsystem != null)
                return;

            SubsystemManager.GetSubsystems(s_SubsystemsReuse);
            if (s_SubsystemsReuse.Count == 0)
                return;

            m_Subsystem = s_SubsystemsReuse[0];

            if (m_LeftHandGameObjects == null)
            {
                m_LeftHandGameObjects = new HandGameObjects(
                    m_PlayerHands.leftHand,
                    positionSpeed);
            }

            if (m_RightHandGameObjects == null)
            {
                m_RightHandGameObjects = new HandGameObjects(
                    m_PlayerHands.rightHand,
                    positionSpeed);
            }


            UpdateRenderingVisibility(m_LeftHandGameObjects, m_Subsystem.leftHand.isTracked);
            UpdateRenderingVisibility(m_RightHandGameObjects, m_Subsystem.rightHand.isTracked);

            m_PreviousDrawMeshes = m_DrawMeshes;
            m_PreviousDebugDrawJoints = m_DebugDrawJoints;
            m_PreviousVelocityType = m_VelocityType;

            m_Subsystem.trackingAcquired += OnTrackingAcquired;
            m_Subsystem.trackingLost += OnTrackingLost;
            m_Subsystem.updatedHands += OnUpdatedHands;
        }

        void UpdateRenderingVisibility(HandGameObjects handGameObjects, bool isTracked)
        {
            if (handGameObjects == null)
                return;
            if (isTracked)
            {
                m_OnEnabled?.Invoke();
            }
            else
            {
                m_OnDisabled?.Invoke();
            }

            handGameObjects.ToggleDrawMesh(m_DrawMeshes && isTracked);
        }

        void OnTrackingAcquired(XRHand hand)
        {
            switch (hand.handedness)
            {
                case Handedness.Left:
                    UpdateRenderingVisibility(m_LeftHandGameObjects, true);
                    break;

                case Handedness.Right:
                    UpdateRenderingVisibility(m_RightHandGameObjects, true);
                    break;
            }
        }

        void OnTrackingLost(XRHand hand)
        {
            switch (hand.handedness)
            {
                case Handedness.Left:
                    UpdateRenderingVisibility(m_LeftHandGameObjects, false);
                    break;

                case Handedness.Right:
                    UpdateRenderingVisibility(m_RightHandGameObjects, false);
                    break;
            }
        }

        void OnUpdatedHands(XRHandSubsystem subsystem, XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
            XRHandSubsystem.UpdateType updateType)
        {
            // We have no game logic depending on the Transforms, so early out here
            // (add game logic before this return here, directly querying from
            // subsystem.leftHand and subsystem.rightHand using GetJoint on each hand)
            if (updateType == XRHandSubsystem.UpdateType.Dynamic)
                return;

            bool leftHandTracked = subsystem.leftHand.isTracked;
            bool rightHandTracked = subsystem.rightHand.isTracked;

            if (m_PreviousDrawMeshes != m_DrawMeshes)
            {
                m_LeftHandGameObjects.ToggleDrawMesh(m_DrawMeshes && leftHandTracked);
                m_RightHandGameObjects.ToggleDrawMesh(m_DrawMeshes && rightHandTracked);
                m_PreviousDrawMeshes = m_DrawMeshes;
            }
            m_LeftHandGameObjects.UpdateJoints(
                subsystem.leftHand,
                (updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.LeftHandJoints) != 0,
                m_DrawMeshes);

            if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose) != 0)
                m_LeftHandGameObjects.UpdateRootPose(subsystem.leftHand);

            m_RightHandGameObjects.UpdateJoints(
                subsystem.rightHand,
                (updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.RightHandJoints) != 0,
                m_DrawMeshes);
                
            if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose) != 0)
                m_RightHandGameObjects.UpdateRootPose(subsystem.rightHand);
        }

        class HandGameObjects
        { 
            HandMesh m_HandMesh;
            Transform[] m_JointXforms = new Transform[XRHandJointID.EndMarker.ToIndex()];
            bool m_IsTracked;
            float m_positionSpeed;
            private Vector3 m_positionOffset;
            static Vector3[] s_LinePointsReuse = new Vector3[2];
            const float k_LineWidth = 0.005f;

            public HandGameObjects(
                HandMesh handMesh,
                float positionSpeed)
            {
                void AssignJoint(
                    XRHandJointID jointId,
                    Transform jointXform)
                {
                    int jointIndex = jointId.ToIndex();
                    m_JointXforms[jointIndex] = jointXform;
                }
                this.m_positionSpeed = positionSpeed; 
                m_HandMesh = handMesh;
                
                     var hand_transf = m_HandMesh.transform;
                //    hand_transf.parent.transform.localPosition = handOffset;
                hand_transf.localRotation = Quaternion.identity;

                Transform wristRootXform = null;
                for (int childIndex = 0; childIndex < hand_transf.childCount; ++childIndex)
                {
                    var child = hand_transf.GetChild(childIndex);
                    if (child.gameObject.name.EndsWith(XRHandJointID.Wrist.ToString()))
                        wristRootXform = child;
                }
                if (wristRootXform == null)
                {
                    Debug.LogWarning("Hand transform hierarchy not set correctly - couldn't find Wrist joint!");
                }
                else
                {
                    AssignJoint(XRHandJointID.Wrist, wristRootXform);
                    for (int childIndex = 0; childIndex < wristRootXform.childCount; ++childIndex)
                    {
                        var child = wristRootXform.GetChild(childIndex);

                        if (child.name.EndsWith(XRHandJointID.Palm.ToString()))
                        {
                            AssignJoint(XRHandJointID.Palm, child);
                            continue;
                        }

                        for (int fingerIndex = (int)XRHandFingerID.Thumb;
                             fingerIndex <= (int)XRHandFingerID.Little;
                             ++fingerIndex)
                        {
                            var fingerId = (XRHandFingerID)fingerIndex;

                            var jointIdFront = fingerId.GetFrontJointID();
                            if (!child.name.EndsWith(jointIdFront.ToString()))
                                continue;

                            AssignJoint(jointIdFront, child);
                            var lastChild = child;

                            int jointIndexBack = fingerId.GetBackJointID().ToIndex();
                            for (int jointIndex = jointIdFront.ToIndex() + 1;
                                 jointIndex <= jointIndexBack;
                                 ++jointIndex)
                            {
                                for (int nextChildIndex = 0; nextChildIndex < lastChild.childCount; ++nextChildIndex)
                                {
                                    var nextChild = lastChild.GetChild(nextChildIndex);
                                    if (nextChild.name.EndsWith(XRHandJointIDUtility.FromIndex(jointIndex).ToString()))
                                    {
                                        lastChild = nextChild;
                                        break;
                                    }
                                }

                                if (!lastChild.name.EndsWith(XRHandJointIDUtility.FromIndex(jointIndex).ToString()))
                                    throw new InvalidOperationException(
                                        "Hand transform hierarchy not set correctly - couldn't find " +
                                        XRHandJointIDUtility.FromIndex(jointIndex) + " joint!");

                                var jointId = XRHandJointIDUtility.FromIndex(jointIndex);
                                AssignJoint(jointId, lastChild);
                            }
                        }
                    }
                }

                for (int fingerIndex = (int)XRHandFingerID.Thumb;
                     fingerIndex <= (int)XRHandFingerID.Little;
                     ++fingerIndex)
                {
                    var fingerId = (XRHandFingerID)fingerIndex;

                    var jointId = fingerId.GetFrontJointID();
                    if (m_JointXforms[jointId.ToIndex()] == null)
                        Debug.LogWarning("Hand transform hierarchy not set correctly - couldn't find " + jointId +
                                         " joint!");
                }
            }
            
            public void ToggleDrawMesh(bool drawMesh)
            {
                for (int childIndex = 0; childIndex < m_HandMesh.transform.childCount; ++childIndex)
                {
                    var xform = m_HandMesh.transform.GetChild(childIndex);
                    if (xform.TryGetComponent<SkinnedMeshRenderer>(out var renderer))
                        renderer.enabled = drawMesh;
                }
            }

            public void UpdateRootPose(XRHand hand)
            {
                m_HandMesh.UpdateJoint(XRHandJointID.Wrist, hand.rootPose.position);
                m_HandMesh.UpdateJoint(XRHandJointID.Wrist, hand.rootPose.rotation);
            }

            public void UpdateJoints(
                XRHand hand,
                bool areJointsTracked,
                bool drawMeshes)
            {
                if (m_IsTracked != areJointsTracked)
                {
                    ToggleDrawMesh(areJointsTracked && drawMeshes);
                    m_IsTracked = areJointsTracked;
                }

                if (!m_IsTracked)
                    return;

                var wristPose = Pose.identity;
                UpdateJoint(hand.GetJoint(XRHandJointID.Wrist),
                    ref wristPose);
                UpdateJoint(hand.GetJoint(XRHandJointID.Palm), ref wristPose,
                    false);

                for (int fingerIndex = (int)XRHandFingerID.Thumb;
                     fingerIndex <= (int)XRHandFingerID.Little;
                     ++fingerIndex)
                {
                    var parentPose = wristPose;
                    var fingerId = (XRHandFingerID)fingerIndex;

                    int jointIndexBack = fingerId.GetBackJointID().ToIndex();
                    for (int jointIndex = fingerId.GetFrontJointID().ToIndex();
                         jointIndex <= jointIndexBack;
                         ++jointIndex)
                    {
                        if (m_JointXforms[jointIndex] != null)
                            UpdateJoint(hand.GetJoint(XRHandJointIDUtility.FromIndex(jointIndex)), ref parentPose);
                    }
                }
            }

            void UpdateJoint(
                XRHandJoint joint,
                ref Pose parentPose,
                bool cacheParentPose = true)
            {
                int jointIndex = joint.id.ToIndex();
                var xform = m_JointXforms[jointIndex];
                if (xform == null || !joint.TryGetPose(out var pose))
                    return;
                var inverseParentRotation = Quaternion.Inverse(parentPose.rotation);
                m_HandMesh.UpdateJoint(joint.id, inverseParentRotation * (pose.position - parentPose.position));
                    m_HandMesh.UpdateJoint(joint.id, inverseParentRotation * pose.rotation);
                if (cacheParentPose)
                    parentPose = pose;
            }

            static void ToggleRenderers<TRenderer>(bool toggle, Transform xform)
                where TRenderer : Renderer
            {
                if (xform.TryGetComponent<TRenderer>(out var renderer))
                    renderer.enabled = toggle;

                for (int childIndex = 0; childIndex < xform.childCount; ++childIndex)
                    ToggleRenderers<TRenderer>(toggle, xform.GetChild(childIndex));
            }
        }

        protected override bool shouldAddMissingComponents =>
            !(m_PlayerHands);

        public override void AddMissingComponents()
        {
            m_PlayerHands ??= transform.GetComponentInChildren<PlayerHands>();
            m_OnEnabled.AddListener(m_PlayerHands.OnEnabled);
            m_OnDisabled.AddListener(m_PlayerHands.OnDisabled);
        }
    }
}