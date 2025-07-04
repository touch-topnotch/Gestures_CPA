/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#if USING_XR_MANAGEMENT && (USING_XR_SDK_OCULUS || USING_XR_SDK_OPENXR)
#define USING_XR_SDK
#endif

using System;
using Scripts.Movements;
using Scripts.PlayerLogic;
using Scripts.Systems;
using Scripts.XR;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Node = UnityEngine.XR.XRNode;

namespace Scripts.Rigs
{
    /// <summary>
    /// A head-tracked stereoscopic virtual reality camera rig.
    /// </summary>
    [ExecuteInEditMode]
    [HelpURL("https://developer.oculus.com/reference/unity/latest/class_o_v_r_camera_rig")]
    public class OVRRig : Rig
    {
        [Header("OVR Rig Components")] [SerializeField]
        private XRMovement _movement;

        [Range(0.01f, 3f)] [SerializeField] private float _bodyHeightOffset = 1.8f;

        [SerializeField] private bool _lerpBody = true;

        [Range(0.1f, 10f)] [SerializeField] [ShowIf("_lerpBody")]
        private float _bodyLerpSpeed = 2f;

        [SerializeField] private CustomXRPokeInteractor _leftPokeInteractor;
        
        [SerializeField] private CustomXRPokeInteractor _rightPokeInteractor;
        
        // interaction manager
        [SerializeField] private CustomXRInteractionManager _interactionManager;
        public override void Initialize()
        {
            base.Initialize();
            headInteraction.onHeadInteraction.AddListener((e) =>
            {
                if (e == HeadInteractionType.Shaking)
                {
                    if (_movement.isMoved())
                        _movement.StopMove();
                    else
                    {
                        Centrize();
                        _movement.StartMove();
                    }
                }
            });
            

            _interactionManager.Initialize();
            _leftPokeInteractor.Initialize();
            _rightPokeInteractor.Initialize();
        }
        protected virtual void FixedUpdate()
        {
            SynchronizeBodyAnchors();
        }

        protected virtual void Update()
        {
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


        public override bool isMoved() => _movement.isMoved();

        public override void StartMove() => _movement.StartMove();

        public override void StopMove() => _movement.StopMove();

        protected override void Centrize()
        {
            Update();
            _movement.Centrize();
        }
    }
}