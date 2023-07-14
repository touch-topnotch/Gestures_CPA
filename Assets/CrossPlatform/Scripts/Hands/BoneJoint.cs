using UnityEngine;

namespace CrossPlatform.Gestures
{
    [RequireComponent(typeof(LineRenderer))]
    public class BoneJoint: MonoBehaviour
    {
        public Transform Parent;
        public float speed = 4f;
        private LineRenderer _lineRenderer;
        private bool isMoving;
        private Vector3 target;
       
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
            
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
        private void UpdateLine()
        {
            if (Parent == null)
                return;
            
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, Parent.position);
        }

        public void SetPositionSmooth(Vector3 position)
        {
            target = position;
            isMoving = true;
        }

        
        public void Update(){
            
            if (!isMoving)
                return;
            
            if(transform.position != target)
            {
                transform.position = Vector3.Lerp(transform.position, target, speed*Time.deltaTime);
                UpdateLine();
                return;
            }
            
            isMoving = false;
        }
    }
}