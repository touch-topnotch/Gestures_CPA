using Scripts.Events;
using Scripts.Gestures;
using Scripts.Hands;
using Scripts.Movements;
using UnityEngine;
using Zenject;

namespace Scripts.PlayerLogic
{
    public class PlayerRig : MonoBehaviour
    {

        public PlayerHands hands;

        public Movement movement;
        
        [SerializeField] protected Transform head;
        [SerializeField] protected Transform body;
        
        public Transform GetHead() => head;
        public Transform GetBody() => body;

    }
}