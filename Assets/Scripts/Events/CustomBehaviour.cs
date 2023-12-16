using UnityEngine;
using Zenject;

namespace Scripts.Events
{
    public abstract class CustomBehaviour: MonoBehaviour
    {
        protected UpdateEvent onUpdate;
        [Inject]
        protected virtual void Construct(UpdateEvent onUpdate)
        {
            this.onUpdate = onUpdate;
        }
    }
}