using System.Collections.Generic;
using Scripts.Hands;
using UnityEngine;
using UnityEngine.XR;

namespace Scripts.PlayerLogic
{
    public class LocalPCRig: PlayerRig
    {
        public void ChangePosOfHand()
        {
            leftHand.points[0].rotation = new Quaternion(0, leftHand.points[0].rotation.y +0.1f, 0, 0);
        }
    }
}