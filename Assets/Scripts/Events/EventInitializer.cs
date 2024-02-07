
using UnityEngine;

namespace Scripts.Events
{
    public class EventInitializer: MonoBehaviour
    {
        private void Update()
        {
            UpdateEvent.Instance?.Invoke();
        }
    }
}