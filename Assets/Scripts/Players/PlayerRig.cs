using Scripts.Events;
using Scripts.Hands;
using Scripts.Movements;
using UnityEngine;
using Zenject;

namespace Scripts.PlayerLogic
{
    public class PlayerRig : MonoBehaviour
    {
        
        [SerializeField] protected Transform head;
        [SerializeField] protected Transform body;
        public HandMesh leftHand;
        public HandMesh rightHand;
        
        public Movement movement;

        public Transform GetHead() => head;
        public Transform GetBody() => body;
  


    }
}