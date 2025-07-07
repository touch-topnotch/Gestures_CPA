using System;
using System.Runtime.Serialization;
using Scripts.Players;
using Scripts.Static.Definitions;
using UnityEngine;

namespace Scripts.Weapons
{
    public abstract class WeaponObserver: MonoBehaviour
    {
        public abstract void Initialize(PlayerData playerData);
        public abstract void OnActivated();
        public abstract void OnDeactivated();
        public abstract void OnHitStarted();
        public abstract void OnHitStopped();
        public abstract void OnImpact(Affected affected);
        public abstract void OnReleased();
    }
}