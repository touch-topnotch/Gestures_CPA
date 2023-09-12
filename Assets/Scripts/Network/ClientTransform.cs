using Scripts.Static;
using Unity.Netcode.Components;
using UnityEngine;

namespace Scripts.Network
{
    [DisallowMultipleComponent]
    public class ClientTransform : NetworkTransform
    {
        /// <summary>
        /// Used to determine who can write to this transform. Owner client only.
        /// This imposes state to the server. This is putting trust on your clients. Make sure no security-sensitive features use this transform.
        /// </summary>
       
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
        
        public void SetPosition(in Vector3 pos)
        {
            transform.position = pos;
        }

        public void SetRotation(in Quaternion rot)
        {
            transform.rotation = rot;
        }
    }
}