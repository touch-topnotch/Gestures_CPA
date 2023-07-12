using Unity.VisualScripting;
using UnityEngine;

namespace CrossPlatform.Gestures
{
    [RequireComponent(typeof(LineRenderer))]
    public class BoneJoint: MonoBehaviour
    {
        public Transform Parent;
        private LineRenderer _lineRenderer;
        
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
            UpdateLine();
        }
       
        public void Initialize()
        {
            if (Parent == null)
                return;
            _lineRenderer = GetComponent<LineRenderer>();
        }
        private void UpdateLine()
        {
            if (Parent == null)
                return;
            
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, Parent.position);
        }
    }
}