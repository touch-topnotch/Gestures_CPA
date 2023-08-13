using Scripts.Events;
using Scripts.Static;
using UnityEngine;

namespace Scripts.Hands
{
    [RequireComponent(typeof(LineRenderer))]
    public class BoneJoint: MonoBehaviour
    {
        public Transform Parent;
        public float speed = 4f;
        private LineRenderer _lineRenderer;
        private Vector3 target;
        private UpdateEvent _onUpdate;
       
        public void SetPosition(in Vector3 position, in Vector3 parentPosition)
        {
            transform.position = position + parentPosition;
            UpdateLine();
        }
       
        public void Initialize()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            if (Parent == null)
            {
                _lineRenderer.enabled = false;
            }
           
        }
        public void UpdateLine()
        {
            if (Parent == null)
                return;
            
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, Parent.position);
        }

        public void SetPositionSmooth(Vector3 position, ref UpdateEvent onUpdate)
        {
            target = position;
            _onUpdate = onUpdate;
            _onUpdate.AddListener(UpdatePosition);
        }


        public void UpdatePosition()
        {
            if (transform.position == target)
            {
                _onUpdate.RemoveListener(UpdatePosition);
                return;
            }

            transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
            UpdateLine();
        }
    }
}