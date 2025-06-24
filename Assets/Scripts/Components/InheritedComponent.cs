using Scripts.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Components
{
    public abstract class InheritedComponent<T> : SmartComponent
        where T : MonoBehaviour
    {
        [DisableIf("inheritedExists")] [SerializeField]
        private T _inherited;

        public T inherited => _inherited ??= GetInherited();

        protected virtual T GetInherited()
        {
            return gameObject.GetComponentInParent<T>();
        }

        protected virtual bool inheritedExists => inherited;
    }

    public class WeaponComponent : NetworkInheritedComponent<Weapon>
    {
        protected override bool shouldAddMissingComponents { get; }
    }
}