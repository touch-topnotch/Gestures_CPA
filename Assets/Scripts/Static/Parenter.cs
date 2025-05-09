using Scripts.Events;
using Unity.Netcode;
using UnityEngine;

namespace Scripts.Static
{
    public class Parenter : NetworkBehaviour
    {
        private Transform _parent;
        private UpdateEvent _onUpdate;
        private bool _isParented;

        public void SetParent(Transform parent, ref UpdateEvent onUpdate)
        {
            if (_isParented)
                RemoveParent();

            _parent = parent;
            _onUpdate = onUpdate;
            _isParented = true;

            _onUpdate.AddListener(OnUpdate);
        }

        public void RemoveParent()
        {
            if (!_isParented)
                return;

            _onUpdate.RemoveListener(OnUpdate);
        }

        private void OnUpdate()
        {
            transform.position = _parent.transform.position;
            transform.rotation = _parent.transform.rotation;
        }
    }
}