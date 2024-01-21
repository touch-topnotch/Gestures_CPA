using Scripts.Events;
using Scripts.PlayerLogic;
using UnityEngine;
using Zenject;

namespace Scripts.Movements
{
    public interface IMovable
    {
        public bool isMoved();
        public void StartMove();
        public void StopMove();
    }
}