
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
        public BodyAnchors Anchors;
    }
} 