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

using Scripts.HandsLogic;
using UnityEngine;

namespace Scripts.Adapters
{
    public class OculusHandRenderer : MonoBehaviour
    {
        public interface IOVRMeshRendererDataProvider
        {
            MeshRendererData GetMeshRendererData();
        }

        public struct MeshRendererData
        {
            public bool IsDataValid { get; set; }
            public bool IsDataHighConfidence { get; set; }
            public bool ShouldUseSystemGestureMaterial { get; set; }
        }

        public enum ConfidenceBehavior
        {
            None,
            ToggleRenderer,
        }

        public enum SystemGestureBehavior
        {
            None,
            SwapMaterial,
        }

        [SerializeField]
        private IOVRMeshRendererDataProvider _dataProvider;

        [SerializeField]
        private HandMesh _handMesh;

        [SerializeField]
        private OVRSkeleton _ovrSkeleton;

        [SerializeField] private OVRMesh _ovrMesh; 
        public bool IsInitialized { get; private set; }
        private void Awake()
        {
            if (_dataProvider == null)
            {
                _dataProvider = GetComponent<IOVRMeshRendererDataProvider>();
            }

            if (_handMesh == null)
            {
                _handMesh= GetComponent<HandMesh>();
            }

            if (_ovrSkeleton == null)
            {
                _ovrSkeleton = GetComponent<OVRSkeleton>();
            }
        }

        private void Start()
        {
            if (_handMesh == null)
            {
                // disable if no mesh configured
                this.enabled = false;
                return;
            }

            if (ShouldInitialize())
            {
                Initialize();
            }
        }

        private bool ShouldInitialize()
        {
            if (IsInitialized)
            {
                return false;
            }

            if ((_handMesh == null) ||
                ((_ovrSkeleton != null) && !_ovrSkeleton.IsInitialized))
            {
                // do not initialize if mesh or optional skeleton are not initialized
                return false;
            }

            return true;
        }

        private void Initialize()
        {
            if ((_ovrSkeleton != null))
            {
                int numSkinnableBones = _ovrSkeleton.GetCurrentNumSkinnableBones();
                var bindPoses = new Matrix4x4[numSkinnableBones];
                var bones = new Transform[numSkinnableBones];
                var localToWorldMatrix = transform.localToWorldMatrix;
                for (int i = 0; i < numSkinnableBones && i < _ovrSkeleton.Bones.Count; ++i)
                {
                    bones[i] = _ovrSkeleton.Bones[i].Transform;
                    bindPoses[i] = _ovrSkeleton.BindPoses[i].Transform.worldToLocalMatrix * localToWorldMatrix;
                }
                _handMesh.Initialize(bones, bindPoses, _ovrMesh.Mesh);
                IsInitialized = true;
            }
        }
    }
}
