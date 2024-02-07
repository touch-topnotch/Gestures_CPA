
using Scripts.Hands;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    public enum AvatarType
    {
        Local,
        Enemy,
        None
    } 
    public class Avatar: MonoBehaviour
    {
        public AvatarType type;
        [Space]
        public Transform head;
        public Transform body;
        public PlayerHands hands;
       
    }
} 