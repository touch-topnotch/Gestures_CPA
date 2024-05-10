using Sirenix.OdinInspector;

namespace Scripts.Components
{
    public abstract class SmartComponent: SerializedMonoBehaviour
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