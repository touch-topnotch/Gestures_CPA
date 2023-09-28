using System;
using Scripts.Databases;
using Scripts.Hands;
using Scripts.Network;
using Scripts.Static;
using Unity.Netcode;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkUser : NetworkBehaviour
    {
        public UserData userData = new UserData() { id = 319, name = "debugger", bonesData = new float [26, 2] };
        public BodyParts bodyParts;
        public NetworkObject networkObject;
    }
    [Serializable]
    public struct BodyParts
    {
        public Parenter Head;
        public Parenter LeftHand;
        public Parenter RightHand;
        public Parenter Body;
    }
}