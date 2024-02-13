using Scripts.Events;
using Scripts.Network;
using Scripts.Static;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Scripts.HandsLogic
{
    [RequireComponent(typeof(ClientTransform))]
    [RequireComponent(typeof(LineRenderer))]
    public class BoneJoint: MonoBehaviour
    {
        
        public float speed = 4f;
        [HideInInspector] public Transform parent;
        private LineRenderer _lineRenderer;
        private Vector3 targetPos;
        private Quaternion targetRot;
        private UpdateEvent _onUpdate;
       
        public void SetPosition(in Vector3 position, in Vector3 parentPosition)
        {
            transform.position = position + parentPosition;
            UpdateLine();
        }
       
        public void Initialize()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            if (parent == null)
            {
                _lineRenderer.enabled = false;
            }
           
        }
        public void UpdateLine()
        {
            if (parent == null)
                return;
            
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, parent.position);
        }

        public void SetPositionSmooth(Vector3 pos, Quaternion rot, ref UpdateEvent onUpdate)
        {
            targetPos = pos;
            targetRot = rot;
            _onUpdate = onUpdate;
            _onUpdate.AddListener(UpdatePosition);
        }


        public void UpdatePosition()
        {  
            if (transform.position == targetPos)
            {
                _onUpdate.RemoveListener(UpdatePosition);
                return;
            }  

            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot ,speed * Time.deltaTime);
            UpdateLine();
        }
    }
}