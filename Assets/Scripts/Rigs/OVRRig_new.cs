#if USING_XR_MANAGEMENT && (USING_XR_SDK_OCULUS || USING_XR_SDK_OPENXR)
#define USING_XR_SDK
#endif

using System;
using Scripts.Characters;
using Scripts.Movements;
using Scripts.PlayerLogic;
using Scripts.Static.Extensions;
using Scripts.Systems;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using CharacterController = UnityEngine.CharacterController;
using Node = UnityEngine.XR.XRNode;

namespace Scripts.Rigs
{
    /// <summary>
    /// A head-tracked stereoscopic virtual reality camera rig.
    /// </summary>
    public class OvrRigNew : Rig
    {
        
        [BoxGroup("Body Settings")]
            [Range(0.01f, 3f)]
            [SerializeField] 
        private float bodyHeightOffset = 1.8f;
        
        [BoxGroup("Body Settings")]
            [SerializeField]
        private bool lerpBody = true;
        
        [BoxGroup("Body Settings")] 
            [Range(0.1f, 10f)]
            [SerializeField] 
            [ShowIf("lerpBody")]
        private float bodyLerpSpeed = 2f;
        
    
        
        [BoxGroup("Locomotion Settings")]
            [SerializeField]
        private Transform cameraAnchor;

        [BoxGroup("Locomotion Settings")]
            [SerializeField]
        private UnityEngine.CharacterController _characterController;
        [BoxGroup("Locomotion Settings")] [SerializeField][MinMaxSlider(0, 1)]
        private Vector2 moveBoards;
        
        
        [SerializeField] 
        [Range(0, 1)] 
        private float standingInaccuracy;
        
        [BoxGroup("Locomotion Settings")] 
        [SerializeField] [Range(0, 30)]
        private float maxVelocityXZ;

        [BoxGroup("Locomotion Settings")] 
        [SerializeField] [Range(0, 30)]
        private float maxVelocityY;
    
        

        [BoxGroup("Locomotion Settings")] [SerializeField]
        private float gravity = 9.81f;
        
        [BoxGroup("Locomotion Settings")] [SerializeField]
        private float moveForce = 9.81f;
        
        [BoxGroup("Locomotion Settings")] [SerializeField]
        private float jumpForce = 9.81f;
        [BoxGroup("Locomotion Settings")]
    

        private Vector3 _cameraDelta;
        private Vector3 _previousCameraPosition;
        
        private float _personHeight;
        private float _currentHeight;
        private void Start()
        {
            // only for debugging
            cameraAnchor.position = new Vector3(0,1.8f,0);
            if (cameraAnchor.GetComponent<Camera>())
                cameraAnchor.GetComponent<Camera>().enabled = false;
            
            _previousCameraPosition = cameraAnchor.position;
            RememberHeight();
        }

        protected virtual void RememberHeight()
        {
            _personHeight = cameraAnchor.position.y;
        }
        
        protected void Update()
        {
            var cameraAnchorPosition = cameraAnchor.position;
            _cameraDelta = cameraAnchorPosition - _previousCameraPosition;
            _currentHeight = cameraAnchorPosition.y;
            
            anchors.Head.rotation = cameraAnchor.rotation;
            
            if (IsInStandingPosition())
            {
                Debug.Log("Centering");
                Centrize();
            }
            else
            {
                Move();
            }
            
            SynchronizeBodyAnchors();
            
            _previousCameraPosition = cameraAnchorPosition;
        }
        private void SynchronizeBodyAnchors()
        {
            var c = anchors.Head.position;
            if (lerpBody)
            {
                anchors.Body.position = Vector3.Lerp(anchors.Body.position,
                    new Vector3(c.x, c.y - bodyHeightOffset, c.z),
                    Time.deltaTime * bodyLerpSpeed);
                anchors.Body.rotation = Quaternion.Lerp(anchors.Body.rotation,
                    Quaternion.Euler(0, anchors.Head.eulerAngles.y, 0), Time.deltaTime * bodyLerpSpeed);
            }
            else
            {
                anchors.Body.position = new Vector3(c.x, c.y - bodyHeightOffset, c.z);
                anchors.Body.rotation = Quaternion.Euler(0, anchors.Head.eulerAngles.y, 0);
            }
        }

        public bool IsInStandingPosition()
        {
            return Math.Abs(_currentHeight - _personHeight) / _personHeight < standingInaccuracy;
        }
        public override bool isMoved() => true;


        public override void StartMove()
        {

        }

        protected virtual void Move()
        {
            anchors.Head.localPosition += _cameraDelta;
            var rootPosition = anchors.Root.position;
            var headPosition = anchors.Head.position;
            var velocity = (HeadManipulations.HeadVelocity( 
                rootPosition + new Vector3(0, _personHeight * 0.85f, 0),
                headPosition,
                moveBoards.x,
                moveBoards.y,
                moveForce,
                jumpForce) + Vector3.down * gravity) / 10;

            _characterController.Move(
                new Vector3(
                    Mathf.Clamp(velocity.x, -maxVelocityXZ, maxVelocityXZ),
                    Mathf.Clamp(velocity.y, -maxVelocityY, maxVelocityY),
                    Mathf.Clamp(velocity.z, -maxVelocityXZ, maxVelocityXZ)
                    )
                );
        }
        public override void StopMove()
        {
        }


   
        protected override void Centrize()
        {
            anchors.Head.localPosition = new Vector3(0, cameraAnchor.localPosition.y, 0);
            anchors.Root.position += new Vector3(_cameraDelta.x, 0, _cameraDelta.z);
            hands.transform.position -= _cameraDelta;
        }
    }
}