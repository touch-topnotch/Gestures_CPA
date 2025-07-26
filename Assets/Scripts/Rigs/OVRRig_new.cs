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
using UnityEngine.InputSystem.Controls;
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
        [BoxGroup("Locomotion Settings")]
            [SerializeField]
        private Transform cameraAnchor;

        [BoxGroup("Locomotion Settings")]
            [SerializeField]
        private UnityEngine.CharacterController _characterController;
        [BoxGroup("Locomotion Settings")] [SerializeField][MinMaxSlider(0, 1)]
        private Vector2 moveBoards;
        
        [BoxGroup("Locomotion Settings")] 
        [SerializeField] 
        [MinMaxSlider(0,1, true)] 
        private Vector2 moveZone;
        [BoxGroup("Locomotion Settings")] 
        [SerializeField]
        [Range(0, 1)] 
        private float smoothMoveToCentrizing;
        
        [BoxGroup("Locomotion Settings")] 
        [SerializeField] [Range(0, 4)]
        private float maxVelocityXZ;

        [BoxGroup("Locomotion Settings")] 
        [SerializeField] [Range(0, 4)]
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
    
            if (cameraAnchor.GetComponent<Camera>())
                cameraAnchor.GetComponent<Camera>().enabled = false;

            if (Application.platform == RuntimePlatform.OSXPlayer)
                cameraAnchor.position = new Vector3(0, 1.8f, 0);
            Centrize();
            hands.transform.localPosition = Vector3.zero;
            _previousCameraPosition = cameraAnchor.position;
            RememberHeight();
            headInteraction.onHeadInteraction.AddListener((e)=>{if(e == HeadInteractionType.Shaking) RememberHeight();});
        }

        protected virtual void RememberHeight()
        {
            _personHeight = cameraAnchor.position.y;
            Debug.Log("Current height - " + _personHeight);
        }
        
        protected void Update()
        {
            var cameraAnchorPosition = cameraAnchor.position;
            _cameraDelta = cameraAnchorPosition - _previousCameraPosition;
            _currentHeight = cameraAnchorPosition.y;
            
            anchors.Head.rotation = cameraAnchor.rotation;
            if (InMoveZone())
            { 
                Move();
            }
            else
            {
                Centrize();
            }

            _previousCameraPosition = cameraAnchorPosition;
        }
        

        private bool InMoveZone() =>  (_currentHeight > (_personHeight * moveZone.x)) &&
                                      (_currentHeight < (_personHeight * moveZone.y)) ;
        public override bool isMoved() => true;


        public override void StartMove()
        {

        }

        protected virtual void Move()
        {
            anchors.Head.localPosition += _cameraDelta;
            float forceK = 1;
            if ((_currentHeight / _personHeight) > ((moveZone.x + moveZone.y) / 2))
                forceK = Math.Clamp(
                    (_personHeight * (moveZone.y) - _currentHeight) / _personHeight /
                    smoothMoveToCentrizing, 0, 1);
            else
                forceK = Math.Clamp(
                    ( _currentHeight - _personHeight * moveZone.x) / _personHeight /
                    smoothMoveToCentrizing, 0, 1);
            
                    
            var rootPosition = anchors.Root.position;
            var headPosition = anchors.Head.position;
            var velocity = (HeadManipulations.HeadVelocity(
                rootPosition + new Vector3(0, _personHeight * 0.85f, 0),
                headPosition,
                moveBoards.x,
                moveBoards.y,
                moveForce * forceK,
                ySpeed: jumpForce * forceK) + gravity * 80 * Vector3.down / _personHeight / 1.8f);
            _characterController.Move(
                new Vector3(
                    Mathf.Clamp(velocity.x, -maxVelocityXZ, maxVelocityXZ),
                    Mathf.Clamp(velocity.y, 0, maxVelocityY),
                    Mathf.Clamp(velocity.z, -maxVelocityXZ, maxVelocityXZ)
                    )
                );
           // hands.transform.position += (anchors.Root.position - rootPosition);
        }
        public override void StopMove()
        {
        }


   
        protected override void Centrize()
        {
            var headPosition = anchors.Head.position;
            var rootPosition = anchors.Root.position;
            
            var xz = new Vector3(headPosition.x + _cameraDelta.x, rootPosition.y,
                headPosition.z + _cameraDelta.z);
            anchors.Head.localPosition = new Vector3(0,cameraAnchor.position.y, 0);
            var delta = rootPosition - xz;
            anchors.Root.position = xz;

            hands.transform.position += delta;
        }
    }
}