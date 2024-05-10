using Sirenix.OdinInspector;
using Unity.Netcode;

namespace Scripts.Components
{
    public abstract class NetworkSmartComponent: NetworkBehaviour
    {
        protected abstract bool shouldAddMissingComponents { get; }
      
#if UNITY_EDITOR
        [ShowIf("shouldAddMissingComponents")]
        [Button("Add Missing Components")]
        private void CallAddMissingComponents()
        {
            AddMissingComponents();
        }
#endif
        public virtual void AddMissingComponents() { }
    }
}