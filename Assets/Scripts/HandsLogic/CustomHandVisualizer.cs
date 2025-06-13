using System;
using System.Collections.Generic;
using Scripts.Adapters;
using Scripts.PlayerLogic;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Hands;

namespace Scripts.HandsLogic
{
//     public class CustomHandVisualizer : RigComponent
//     {
//         public enum VelocityType
//         {
//             Linear,
//             Angular,
//             None,
//         }
//
//         [SerializeField]
//         [Tooltip(
//             "If this is enabled, this component will enable the Input System internal feature flag 'USE_OPTIMIZED_CONTROLS'. You must have at least version 1.5.0 of the Input System and have its backend enabled for this to take effect.")]
//         bool m_UseOptimizedControls;
//
//         [SerializeField] XROrigin m_Origin;
//
//         [SerializeField] private PlayerHands m_PlayerHands;
//
//         public bool drawMeshes
//         {
//             get => m_DrawMeshes;
//             set => m_DrawMeshes = value;
//         }
//
//         [SerializeField] bool m_DrawMeshes;
//         bool m_PreviousDrawMeshes;
//
//         [SerializeField] UnityEvent m_OnEnabled;
//         [SerializeField] UnityEvent m_OnDisabled;
//
//         [SerializeField] HandAdapter m_HandAdapter;
//         HandGameObjects m_LeftHandGameObjects;
//         HandGameObjects m_RightHandGameObjects;
//
//         protected void Awake()
//         {
// #if ENABLE_INPUT_SYSTEM
//             if (m_UseOptimizedControls)
//                 InputSystem.settings.SetInternalFeatureFlag("USE_OPTIMIZED_CONTROLS", true);
// #endif // ENABLE_INPUT_SYSTEM
//         }
//         protected void OnEnable()
//         {
//             Debug.Log("HANDS ENABLED");
//             UpdateRenderingVisibility(m_LeftHandGameObjects, m_HandAdapter.LeftTracked());
//             UpdateRenderingVisibility(m_RightHandGameObjects, m_HandAdapter.RightTracked());
//         }
//
//         protected void OnDisable()
//         {
//             Debug.Log("HANDS DISABLED");
//             UpdateRenderingVisibility(m_LeftHandGameObjects, false);
//             UpdateRenderingVisibility(m_RightHandGameObjects, false);
//         }
//
//         protected void OnDestroy()
//         {
//             if (m_LeftHandGameObjects != null)
//             {
//                 m_LeftHandGameObjects.OnDestroy();
//                 m_LeftHandGameObjects = null;
//             }
//
//             if (m_RightHandGameObjects != null)
//             {
//                 m_RightHandGameObjects.OnDestroy();
//                 m_RightHandGameObjects = null;
//             }
//         }
//
//         protected void FixedUpdate()
//         {
//             if (m_LeftHandGameObjects == null)
//             {
//                 m_LeftHandGameObjects = new HandGameObjects(
//                     Handedness.Left,
//                     m_PlayerHands.leftHand);
//             }
//         
//             if (m_RightHandGameObjects == null)
//             {
//                 m_RightHandGameObjects = new HandGameObjects(
//                     Handedness.Right,
//                     m_PlayerHands.rightHand);
//             }
//             
//             UpdateRenderingVisibility(m_LeftHandGameObjects, m_HandAdapter.LeftTracked());
//             UpdateRenderingVisibility(m_RightHandGameObjects, m_HandAdapter.RightTracked());
//          
//             Debug.Log("  UpdateRenderingVisibility(m_LeftHandGameObjects, m_Subsystem.leftHand.isTracked);" + "\n"+
//                 "UpdateRenderingVisibility(m_RightHandGameObjects, m_Subsystem.rightHand.isTracked);");
//         
//             m_PreviousDrawMeshes = m_DrawMeshes;
//         
//         }
//
//         void UpdateRenderingVisibility(HandGameObjects handGameObjects, bool isTracked)
//         {
//             if (handGameObjects == null)
//                 return;
//             if (isTracked)
//             {
//                 m_OnEnabled?.Invoke();
//             }
//             else
//             {
//                 m_OnDisabled?.Invoke();
//             }
//
//             handGameObjects.ToggleDrawMesh(m_DrawMeshes && isTracked);
//         }
//
//         void OnTrackingAcquired(XRHand hand)
//         {
//             switch (hand.handedness)
//             {
//                 case Handedness.Left:
//                     UpdateRenderingVisibility(m_LeftHandGameObjects, true);
//                     break;
//
//                 case Handedness.Right:
//                     UpdateRenderingVisibility(m_RightHandGameObjects, true);
//                     break;
//             }
//         }
//
//         void OnTrackingLost(XRHand hand)
//         {
//             switch (hand.handedness)
//             {
//                 case Handedness.Left:
//                     UpdateRenderingVisibility(m_LeftHandGameObjects, false);
//                     break;
//
//                 case Handedness.Right:
//                     UpdateRenderingVisibility(m_RightHandGameObjects, false);
//                     break;
//             }
//         }
//
//         void OnUpdatedHands(XRHandSubsystem subsystem, XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
//             XRHandSubsystem.UpdateType updateType)
//         {
//             // We have no game logic depending on the Transforms, so early out here
//             // (add game logic before this return here, directly querying from
//             // subsystem.leftHand and subsystem.rightHand using GetJoint on each hand)
//             if (updateType == XRHandSubsystem.UpdateType.Dynamic)
//                 return;
//
//             bool leftHandTracked = subsystem.leftHand.isTracked;
//             bool rightHandTracked = subsystem.rightHand.isTracked;
//
//             if (m_PreviousDrawMeshes != m_DrawMeshes)
//             {
//                 m_LeftHandGameObjects.ToggleDrawMesh(m_DrawMeshes && leftHandTracked);
//                 m_RightHandGameObjects.ToggleDrawMesh(m_DrawMeshes && rightHandTracked);
//                 m_PreviousDrawMeshes = m_DrawMeshes;
//             }
//
//             m_LeftHandGameObjects.UpdateJoints(
//                 m_Origin,
//                 subsystem.leftHand,
//                 (updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.LeftHandJoints) != 0,
//                 m_DrawMeshes);
//
//             if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose) != 0)
//                 m_LeftHandGameObjects.UpdateRootPose(subsystem.leftHand);
//
//             m_RightHandGameObjects.UpdateJoints(
//                 m_Origin,
//                 subsystem.rightHand,
//                 (updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.RightHandJoints) != 0,
//                 m_DrawMeshes);
//
//             if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose) != 0)
//                 m_RightHandGameObjects.UpdateRootPose(subsystem.rightHand);
//         }
//
//         class HandGameObjects
//         {
//             public HandMesh m_HandMesh;
//             GameObject m_DrawJointsParent;
//
//             Transform[] m_JointXforms = new Transform[XRHandJointID.EndMarker.ToIndex()];
//             GameObject[] m_DrawJoints = new GameObject[XRHandJointID.EndMarker.ToIndex()];
//             GameObject[] m_VelocityParents = new GameObject[XRHandJointID.EndMarker.ToIndex()];
//             LineRenderer[] m_Lines = new LineRenderer[XRHandJointID.EndMarker.ToIndex()];
//             bool m_IsTracked;
//
//             static Vector3[] s_LinePointsReuse = new Vector3[2];
//             const float k_LineWidth = 0.005f;
//
//             public HandGameObjects(
//                 Handedness handedness,
//                 HandMesh handMesh)
//             {
//                 void AssignJoint(
//                     XRHandJointID jointId,
//                     Transform jointXform,
//                     Transform drawJointsParent)
//                 {
//                     int jointIndex = jointId.ToIndex();
//                     m_JointXforms[jointIndex] = jointXform;
//                 }
//
//                 m_HandMesh = handMesh;
//                 var hand_transf = m_HandMesh.transform;
//                 //    hand_transf.parent.transform.localPosition = handOffset;
//                 hand_transf.localRotation = Quaternion.identity;
//
//                 Transform wristRootXform = null;
//                 for (int childIndex = 0; childIndex < hand_transf.childCount; ++childIndex)
//                 {
//                     var child = hand_transf.GetChild(childIndex);
//                     if (child.gameObject.name.EndsWith(XRHandJointID.Wrist.ToString()))
//                         wristRootXform = child;
//                 }
//
//                 if (wristRootXform == null)
//                 {
//                     Debug.LogWarning("Hand transform hierarchy not set correctly - couldn't find Wrist joint!");
//                 }
//                 else
//                 {
//                     AssignJoint(XRHandJointID.Wrist, wristRootXform, m_DrawJointsParent.transform);
//                     for (int childIndex = 0; childIndex < wristRootXform.childCount; ++childIndex)
//                     {
//                         var child = wristRootXform.GetChild(childIndex);
//
//                         if (child.name.EndsWith(XRHandJointID.Palm.ToString()))
//                         {
//                             AssignJoint(XRHandJointID.Palm, child, m_DrawJointsParent.transform);
//                             continue;
//                         }
//
//                         for (int fingerIndex = (int)XRHandFingerID.Thumb;
//                              fingerIndex <= (int)XRHandFingerID.Little;
//                              ++fingerIndex)
//                         {
//                             var fingerId = (XRHandFingerID)fingerIndex;
//
//                             var jointIdFront = fingerId.GetFrontJointID();
//                             if (!child.name.EndsWith(jointIdFront.ToString()))
//                                 continue;
//
//                             AssignJoint(jointIdFront, child, m_DrawJointsParent.transform);
//                             var lastChild = child;
//
//                             int jointIndexBack = fingerId.GetBackJointID().ToIndex();
//                             for (int jointIndex = jointIdFront.ToIndex() + 1;
//                                  jointIndex <= jointIndexBack;
//                                  ++jointIndex)
//                             {
//                                 for (int nextChildIndex = 0; nextChildIndex < lastChild.childCount; ++nextChildIndex)
//                                 {
//                                     var nextChild = lastChild.GetChild(nextChildIndex);
//                                     if (nextChild.name.EndsWith(XRHandJointIDUtility.FromIndex(jointIndex).ToString()))
//                                     {
//                                         lastChild = nextChild;
//                                         break;
//                                     }
//                                 }
//
//                                 if (!lastChild.name.EndsWith(XRHandJointIDUtility.FromIndex(jointIndex).ToString()))
//                                     throw new InvalidOperationException(
//                                         "Hand transform hierarchy not set correctly - couldn't find " +
//                                         XRHandJointIDUtility.FromIndex(jointIndex) + " joint!");
//
//                                 var jointId = XRHandJointIDUtility.FromIndex(jointIndex);
//                                 AssignJoint(jointId, lastChild, m_DrawJointsParent.transform);
//                             }
//                         }
//                     }
//                 }
//
//                 for (int fingerIndex = (int)XRHandFingerID.Thumb;
//                      fingerIndex <= (int)XRHandFingerID.Little;
//                      ++fingerIndex)
//                 {
//                     var fingerId = (XRHandFingerID)fingerIndex;
//
//                     var jointId = fingerId.GetFrontJointID();
//                     if (m_JointXforms[jointId.ToIndex()] == null)
//                         Debug.LogWarning("Hand transform hierarchy not set correctly - couldn't find " + jointId +
//                                          " joint!");
//                 }
//             }
//
//             public void OnDestroy()
//             {
//                 for (int jointIndex = 0; jointIndex < m_DrawJoints.Length; ++jointIndex)
//                 {
//                     Destroy(m_DrawJoints[jointIndex]);
//                     m_DrawJoints[jointIndex] = null;
//                 }
//
//                 for (int jointIndex = 0; jointIndex < m_VelocityParents.Length; ++jointIndex)
//                 {
//                     Destroy(m_VelocityParents[jointIndex]);
//                     m_VelocityParents[jointIndex] = null;
//                 }
//
//                 Destroy(m_DrawJointsParent);
//                 m_DrawJointsParent = null;
//             }
//
//             public void ToggleDrawMesh(bool drawMesh)
//             {
//                 for (int childIndex = 0; childIndex < m_HandMesh.transform.childCount; ++childIndex)
//                 {
//                     var xform = m_HandMesh.transform.GetChild(childIndex);
//                     if (xform.TryGetComponent<SkinnedMeshRenderer>(out var renderer))
//                         renderer.enabled = drawMesh;
//                 }
//             }
//             public void UpdateRootPose(XRHand hand)
//             {
//                 var xform = m_JointXforms[XRHandJointID.Wrist.ToIndex()];
//                 xform.localPosition = hand.rootPose.position;
//                 xform.localRotation = hand.rootPose.rotation;
//             }
//
//             public void UpdateJoints(
//                 XROrigin xrOrigin,
//                 XRHand hand,
//                 bool areJointsTracked,
//                 bool drawMeshes)
//             {
//                 if (m_IsTracked != areJointsTracked)
//                 {
//                     ToggleDrawMesh(areJointsTracked && drawMeshes);
//                     m_IsTracked = areJointsTracked;
//                 }
//
//                 if (!m_IsTracked)
//                     return;
//
//                 var originTransform = xrOrigin.Origin.transform;
//                 var originPose = new Pose(originTransform.position, originTransform.rotation);
//
//                 var wristPose = Pose.identity;
//                 UpdateJoint(originPose, hand.GetJoint(XRHandJointID.Wrist),
//                     ref wristPose);
//                 UpdateJoint(originPose, hand.GetJoint(XRHandJointID.Palm), ref wristPose,
//                     false);
//
//                 for (int fingerIndex = (int)XRHandFingerID.Thumb;
//                      fingerIndex <= (int)XRHandFingerID.Little;
//                      ++fingerIndex)
//                 {
//                     var parentPose = wristPose;
//                     var fingerId = (XRHandFingerID)fingerIndex;
//
//                     int jointIndexBack = fingerId.GetBackJointID().ToIndex();
//                     for (int jointIndex = fingerId.GetFrontJointID().ToIndex();
//                          jointIndex <= jointIndexBack;
//                          ++jointIndex)
//                     {
//                         if (m_JointXforms[jointIndex] != null)
//                             UpdateJoint(originPose, hand.GetJoint(XRHandJointIDUtility.FromIndex(jointIndex)), ref parentPose);
//                     }
//                 }
//             }
//
//             void UpdateJoint(
//                 Pose originPose,
//                 XRHandJoint joint,
//                 ref Pose parentPose,
//                 bool cacheParentPose = true)
//             {
//                 int jointIndex = joint.id.ToIndex();
//                 var xform = m_JointXforms[jointIndex];
//                 if (xform == null || !joint.TryGetPose(out var pose))
//                     return;
//
//                 m_DrawJoints[jointIndex].transform.localPosition = pose.position;
//                 m_DrawJoints[jointIndex].transform.localRotation = pose.rotation;
//
//             
//
//                 var inverseParentRotation = Quaternion.Inverse(parentPose.rotation);
//                 xform.localPosition = inverseParentRotation * (pose.position - parentPose.position);
//                 xform.localRotation = inverseParentRotation * pose.rotation;
//                 if (cacheParentPose)
//                     parentPose = pose;
//             }
//
//             static void ToggleRenderers<TRenderer>(bool toggle, Transform xform)
//                 where TRenderer : Renderer
//             {
//                 if (xform.TryGetComponent<TRenderer>(out var renderer))
//                     renderer.enabled = toggle;
//
//                 for (int childIndex = 0; childIndex < xform.childCount; ++childIndex)
//                     ToggleRenderers<TRenderer>(toggle, xform.GetChild(childIndex));
//             }
//         }
//
//         protected override bool shouldAddMissingComponents =>
//             !(m_Origin && m_PlayerHands);
//         public override void AddMissingComponents()
//         {
//             m_Origin ??= this.transform.GetComponentInChildren<XROrigin>();
//             m_PlayerHands ??= transform.GetComponentInChildren<PlayerHands>();
//             m_OnEnabled.AddListener(m_PlayerHands.OnEnabled);
//             m_OnDisabled.AddListener(m_PlayerHands.OnDisabled);
//         }
//     }
    
}