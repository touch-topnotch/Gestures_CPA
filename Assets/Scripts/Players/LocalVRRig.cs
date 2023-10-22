using System.Collections.Generic;
using Scripts.Hands;
using UnityEngine.XR;

namespace Scripts.PlayerLogic
{
    public class LocalVRRig: VRRig
    {
        protected BonesData _left = new BonesData(HandType.left);
        protected BonesData _right = new BonesData(HandType.right);

        private void Start()
        {
            onUpdate.AddListener(UpdateTransforms);
        }
        private void GetBoneRotations(Hand hand, BonesData data)
        {
            int i = 0;
            foreach (HandFinger finger in System.Enum.GetValues(typeof(HandFinger)))
            {
                
                List<Bone> fingerBones = new List<Bone>();
                if (hand.TryGetFingerBones(finger, fingerBones))
                {
                    foreach (Bone bone in fingerBones)
                    {
                        if (i == 0)
                        {
                            bone.TryGetPosition(out data.RootPos);
                        }
                        bone.TryGetRotation(out data.Rotations[i]);
                        // Perform actions with the bone rotation
                        i++;
                    }
                }

                i++;
            }
        }

        private void UpdateTransforms()
        {
            InputDevice leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            InputDevice rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

            if (leftHandDevice.TryGetFeatureValue(CommonUsages.handData, out var leftHand))
            {
                GetBoneRotations(leftHand, _left);
            }

            if (rightHandDevice.TryGetFeatureValue(CommonUsages.handData, out var rightHand))
            {
                GetBoneRotations(rightHand, _right);
            }
        }
    }
}