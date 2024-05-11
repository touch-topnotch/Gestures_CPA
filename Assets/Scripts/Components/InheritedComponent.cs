using Scripts.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Components
{
    public abstract class InheritedComponent<T>: SmartComponent
    where T: MonoBehaviour
    {
        [HideIf("inheritedExists")] [SerializeField]
        public T inherited { get; private set; }
        
        protected virtual T GetInherited()
        {
            return gameObject.GetComponentInParent<T>();
        }

        private void Awake()
        {
            inherited ??= GetInherited();
        }

        private void OnValidate()
        {
            inherited ??= GetInherited();
        }

        private bool inheritedExists => inherited ??= GetInherited();
    }

    public class WeaponComponent : NetworkInheritedComponent<Weapon>
    {
        protected override bool shouldAddMissingComponents { get; }
    }
}