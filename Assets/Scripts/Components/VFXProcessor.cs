using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Components
{
    public class VFXProcessor : ResourcesProcessor<GameObject>
    {
        protected override void ManipulateResource(GameObject resource)
        {
        }
#if UNITY_EDITOR
        protected override void AddMissingResources()
        {
            base.AddMissingResources();
            PoolAllObjects();
        }
#endif

        public void DisableAllObjects()
        {
            ManipulateOfAllObjects((o => { o.SetActive(false); }));
        }

        protected override bool shouldAddMissingComponents { get; }
    }
}