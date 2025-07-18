#if USING_XR_MANAGEMENT && (USING_XR_SDK_OCULUS || USING_XR_SDK_OPENXR)
#define USING_XR_SDK
#endif

using System;
using Scripts.PlayerLogic;
using Scripts.Static.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using Node = UnityEngine.XR.XRNode;

namespace Scripts.Rigs
{
    /// <summary>
    /// A head-tracked stereoscopic virtual reality camera rig.
    /// </summary>
    [ExecuteInEditMode]
    [HelpURL("https://developer.oculus.com/reference/unity/latest/class_o_v_r_camera_rig")]
    public class OVRRig_new : Rig
    {

       // [SerializeField] private Transform targetHead;
        [Range(0.01f, 3f)] [SerializeField] private float _bodyHeightOffset = 1.8f;
        [SerializeField] private bool _lerpBody = true;
        [Range(0.1f, 10f)] [SerializeField] [ShowIf("_lerpBody")]
        private float _bodyLerpSpeed = 2f;
        
        [SerializeField] [Range(0, 1)] private float standingInaccuracy;
        private float personHeight;
        private float currentHeight; 
        private void Start()
        {
            // only for debugging
            anchors.Head.localPosition = new Vector3(0,1.8f,0);
            personHeight = anchors.Head.position.y - anchors.Root.position.y;
        }
        
        protected void FixedUpdate()
        {
            //remove targetHead
          //  anchors.Head.position = targetHead.position;
            currentHeight = anchors.Head.position.y - anchors.Root.position.y;
            if (IsInStandingPosition())
            {
                Debug.Log("Standing now");
                Centrize();
            }
            SynchronizeBodyAnchors();
        }
        private void SynchronizeBodyAnchors()
        {
            var c = anchors.Head.position;
            if (_lerpBody)
            {
                anchors.Body.position = Vector3.Lerp(anchors.Body.position,
                    new Vector3(c.x, c.y - _bodyHeightOffset, c.z),
                    Time.deltaTime * _bodyLerpSpeed);
                anchors.Body.rotation = Quaternion.Lerp(anchors.Body.rotation,
                    Quaternion.Euler(0, anchors.Head.eulerAngles.y, 0), Time.deltaTime * _bodyLerpSpeed);
            }
            else
            {
                anchors.Body.position = new Vector3(c.x, c.y - _bodyHeightOffset, c.z);
                anchors.Body.rotation = Quaternion.Euler(0, anchors.Head.eulerAngles.y, 0);
            }
        }

        public bool IsInStandingPosition()
        {
            return Math.Abs(currentHeight - personHeight) / personHeight < standingInaccuracy;
        }
        public override bool isMoved() => true;

        public override void StartMove()
        {
        }

        public override void StopMove()
        {
        }

        protected override void Centrize()
        {

            Vector3 offset = Vector3.Scale(anchors.Head.localPosition, VectorExtension.xz);
            anchors.Root.position = new Vector3(anchors.Head.position.x, anchors.Root.position.y, anchors.Head.position.z);
            
            anchors.Head.localPosition = new Vector3(0, anchors.Head.localPosition.y, 0);

            hands.transform.position -= offset;

        }
    }
}