using System;
using Scripts.PlayerLogic;
using UnityEngine.Events;

namespace Scripts.Events
{
    [Serializable]
    public class PlayerStateChangedEvent : UnityEvent<PlayerState>
    {
        
    }
}