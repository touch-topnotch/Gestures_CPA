using System;
using Scripts.PlayerLogic;
using Scripts.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Components
{
    public abstract class InheritedComponent<T>: SmartComponent
    where T: MonoBehaviour
    {
        [HideIf("inheritedExists")] [SerializeField]
        protected T inherited { get; private set; }
        
        protected virtual T GetInherited()
        {
            return gameObject.GetComponentInParent<T>();
        }

        private bool inheritedExists => inherited ??= GetInherited();
    }

    public class WeaponComponent : NetworkInheritedComponent<Weapon>
    {
        protected override bool shouldAddMissingComponents { get; }
    }
}