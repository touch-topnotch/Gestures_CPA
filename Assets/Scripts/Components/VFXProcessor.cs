using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Components
{
    public class VFXProcessor: ResourcesProcessor<GameObject>
    {
        protected override void ManipulateResource(GameObject resource)
        {
            
        }

        protected override void AddMissingResources()
        {
            base.AddMissingResources();
            PoolAllObjects();
        }

        public void DisableAllObjects()
        {
            ManipulateOfAllObjects((o => { o.SetActive(false); }));
        }

   
    }
}