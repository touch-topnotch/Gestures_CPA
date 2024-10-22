using System.Collections.Generic;
using Scripts.Hands;
using UnityEngine;
using UnityEngine.XR;

namespace Scripts.PlayerLogic
{
    public class LocalPCRig: PlayerRig
    {
        protected BonesData _left = new BonesData(HandType.left);
        protected BonesData _right = new BonesData(HandType.right);

        private void Start()
        {
            // Set Random Rotations to _left.Rotations
            // Set Random Rotations to _right.Rotations
            
            _left.RootPos = new Vector3(0, 0, 0);
            _left.Rotations = new Quaternion[26];
            _right.RootPos = new Vector3(0, 0, 0);
            _right.Rotations = new Quaternion[26];
        }
        
    }
}