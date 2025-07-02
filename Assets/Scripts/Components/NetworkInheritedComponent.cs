using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Components
{
    public abstract class NetworkInheritedComponent<T> : NetworkSmartComponent
        where T : MonoBehaviour
    {
        [HideIf("inheritedExists")]
        [SerializeField]
        protected T inherited { get; private set; }

        protected virtual T GetInherited()
        {
            return gameObject.GetComponentInParent<T>();
        }

        private bool inheritedExists => inherited ??= GetInherited();
    }
}