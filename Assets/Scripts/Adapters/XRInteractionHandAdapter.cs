using Scripts.HandsLogic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Hands;

namespace Scripts.Adapters
{
//     public class XRInteractionHandAdapter: HandAdapter
//     {
//         public XRHandSubsystem subsystem;
//         private XRHand hand => handType == HandType.left ? subsystem.leftHand : subsystem.rightHand;
//         protected override bool shouldAddMissingComponents => subsystem == null;
//         
//         public void Update()
//         {
//             if (!isTracked)
//                 return;
//
//             rootPos = hand.rootPose.position;
//             for (int i = 0; i < 26; i++)
//             {
//                 if (hand.GetJoint(XRHandJointIDUtility.FromIndex(i)).TryGetPose(out var pose))
//                     rotations[i] = pose.rotation;
//             }
//         }
// #if UNITY_EDITOR
//        // [Button("Test System")]
//         public void TestSystem()
//         {
//              Assert.AreEqual(HandBonesUtility.ToIndex("Wrist"), 0, "ты даун");
//              Assert.AreEqual(HandBonesUtility.ToIndex("Palm"), 16, "ты даун");
//              Assert.AreEqual(HandBonesUtility.ToIndex("RingTip"), 21, "ты даун");
//              Assert.AreEqual(HandBonesUtility.ToIndex("LittleMetacarpal"), 6, "ты даун");
//              Assert.AreEqual(HandBonesUtility.ToName(6), "LittleMetacarpal", "ты даун");
//              Assert.AreEqual(HandBonesUtility.ToName(0), "Wrist", "ты даун");
//              Assert.AreEqual(HandBonesUtility.ToName(21), "RingTip", "ты даун");
//         }
//         #endif
//         public override bool isTracked => hand.isTracked;
//     }
}