using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Components
{
    /// <summary>
    /// This class 
    /// </summary>
    public abstract class SmartComponent : MonoBehaviour
    {
        // the OnValidate function allows to decrease the amount of calls shouldAddMissingComponent property
        private bool _shouldAddMissingComponents;

        private void OnValidate()
        {
            _shouldAddMissingComponents = shouldAddMissingComponents;
        }

        protected abstract bool shouldAddMissingComponents { get; }

#if UNITY_EDITOR
        [ShowIf("_shouldAddMissingComponents")]
        [Button("Add Missing Components")]
        private void CallAddMissingComponents()
        {
            AddMissingComponents();
        }
#endif


        public virtual void AddMissingComponents()
        {
        }
    }
}