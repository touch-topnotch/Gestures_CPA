using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Scripts.Design;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using Scripts.Static;
using Scripts.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace Scripts.HandsLogic
{
    public enum HandType
    {
        left,
        right
    }
    public interface IHandInteraction
    {
        Vector3 positionOffset { get; set; }
        Quaternion rotationOffset { get; set; }
        
        void UpdateJoint(XRHandJointID index, in Vector3 position);
        void UpdateJoint(XRHandJointID index, in Quaternion rotation);
        void UpdateJoint(int handMeshId, in Vector3 position);
        void UpdateJoint(int handMeshId, in Quaternion rotation);
        void UpdateJoints(in Quaternion[] rotation);
    }

    public class HandMesh : MonoBehaviour, IQueueVisualised<BonesData>, IColorable, IHandInteraction
    {
        private enum HandMaterialType
        {
            Player,
            Ghost
        }

        [Header("Settings")] [SerializeField][Range(0f, 100f)] private float positionSpeed;
        [SerializeField] [Range(0, 100f)] private float rotationSpeed;
        [Header("Types")]
        [SerializeField] private HandMaterialType _handMaterialType;
        [SerializeField] private HandType _handType;
        [Space] [Header("Transforms")] 
        public Transform grabPoint;
        public Transform palmCenter;
        public Transform[] points;

        [SerializeField] private List<Material> _materials = new List<Material>();
        public static readonly int[] XRHandJointIdToCustom = new[]         // this array converts XRHandJointID to our metrics
            { 0, 16, 22, 23, 24, 25, 1, 2, 3, 4, 5, 11, 12, 13, 14, 15, 17, 18, 19, 20, 21, 6, 7, 8, 9, 10 };
        public static readonly int[] CustomToXRHandJointId = new[]        // this array converts our to XRHandJointID metrics
            { 0, 6, 7, 8, 9, 10, 21, 22, 23, 24, 25, 11, 12, 13, 14, 15, 1, 16, 17, 18, 19, 20, 2, 3, 4, 5 };
        public Material HandMaterial
        {
            get => _meshRenderer.materials[1];
            set
            {
                _materials[1] = value;
                _meshRenderer.SetMaterials(_materials);
            }
        }

        [SerializeField] private SkinnedMeshRenderer _meshRenderer;

        private readonly List<TargetProp> _targets = new();

        private readonly List<PinPongProp> _pinPongs = new();

        private UpdateEvent onUpdate => Global.updateEvent;

        private bool _isPlaced;
        private float _progress;
        private bool _isMoved;
        private BonesData _target;
        private Tween _tween;
        private HandMoveProps _lastProps;

#if UNITY_EDITOR
        [Button("Add missing components")]
        public void AddMissingComponents()
        {
            _handType = name[^1] == 'L' ? HandType.left : HandType.right;

            _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            _materials = _meshRenderer.sharedMaterials.ToList();

            if (points == null || points.Length == 0)
            {
                points = new Transform[26];
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).name.Contains("Wrist"))
                    {
                        AddAllChildren(transform.GetChild(i));
                        break;
                    }
                }
            }
        }
#endif

        private int AddAllChildren(Transform parent, int id = 0)
        {
            if (id == 26)
                return 0;
            points[id] = parent;
            id++;

            for (int i = 0; i < parent.childCount; i++)
            {
                id = AddAllChildren(parent.GetChild(i), id);
            }

            return id;
        }

        public void SetRotations(in Vector3[] rotations)
        {
            if (rotations == null)
            {
                return;
            }

            for (int i = 0; i < points.Length; i++)
            {
                points[i].rotation = Quaternion.Euler(rotations[i]);
            }
        }

        private void MoveHand()
        {
          

            if (_target == null || _target.rotations == null || _target.rotations?.Length != 26)
            {
               
                return;
            }
            _progress += Time.deltaTime * _lastProps.speed;
            if (_progress > 1)
            {
                _isPlaced = true;
                _progress = 1;
            }

            if (_lastProps.changePosition)
                points[0].localPosition = Vector3.Lerp(points[0].localPosition, _target.rootPos,
                    _lastProps.animationCurve.Evaluate(_progress));
            for (int i = 0; i < 26; i++)
            {
                points[i].localRotation = Quaternion.Lerp(points[i].localRotation,
                    _target.rotations[i], _lastProps.animationCurve.Evaluate(_progress));
            }
        }


        private void StopMoveHand()
        {
            _progress = 0; 
            _lastProps.onPlaced?.Invoke();
            _isPlaced = true;
        }

        public bool IsActive() => gameObject.activeSelf;

        public void Show()
        {
            gameObject.SetActive(true);
            ChangeColor(new Color(0.6f, 0.6f, 0.6f, 0.6f), HandShaderProps.EdgeColor);
        }

        public void Hide(bool immediately)
        {
            if (immediately)
                HideImmediately();
            else
                HideTween();
        }


        public void HideTween()
        {
            ChangeColorForProps(Color.clear, HandShaderProps.AllColors, new ColorParams(0, 1, false), prop =>
            {
                ChangeColor(Color.clear, prop);
                HandMaterial.DOKill();
                this.gameObject.SetActive(false);
            });
        }
        public void HideImmediately()
        {
            this.gameObject.SetActive(false);
        }

        public void Replace(BonesData target)
        {
            if (!target.Exists())
            {
                HideImmediately();
                return;
            }

            points[0].localPosition = target.rootPos;
            for (int i = 0; i < target.rotations.Length; i++)
            {
                points[i].localRotation = target.rotations[i];
            }
        }
        public void Move(BonesData target, HandMoveProps props)
        {
            if (target == null || target.rotations == null || target.rotations.Length == 0)
                return;
            _target = target;
            _isPlaced = false;
            _progress = 0;
            _lastProps = props;
            if (!_isMoved)
                onUpdate.AddListener(MoveHand);
        }

        public void Destroy()
        {
            HandMaterial.DOKill();
            GameObject.Destroy(this);
        }

        public void ChangeColorForProps(in Color color, in int[] props, in ColorParams pColorParams,
            Action<int> onComplete)
        {
            foreach (int prop in props)
            {
                if (pColorParams.speed > 0)
                {
                    ChangeColorSmooth(color, new ColorParams(prop, pColorParams), () => { onComplete?.Invoke(prop); });
                }
                else
                {
                    onComplete?.Invoke(prop);
                }
            }
        }

        public void ChangeColor(in Color color, int id)
        {
            HandMaterial.SetColor(id, color);
        }

        public void ChangeColorPinPong(Color active, Color passive, ColorParams pColorParams)
        {
            if (pColorParams.kill)
                HandMaterial.DOKill();
            HandMaterial.DOColor(active, pColorParams.id, 1 / pColorParams.speed).onComplete = () =>
            {
                HandMaterial.DOColor(passive, pColorParams.id, 1 / pColorParams.speed).onComplete = () =>
                {
                    ChangeColorPinPong(active, passive,
                        new ColorParams(pColorParams.id, pColorParams.speed, false));
                };
            };
        }

        public void ChangeColorSmooth(Color color, ColorParams pColorParams, TweenCallback onComplete = null)
        {
            if (pColorParams.kill)
                HandMaterial.DOKill();

            HandMaterial.DOColor(color, pColorParams.id, 1 / pColorParams.speed).onComplete =
                onComplete;
        }

        public bool inSameLocation(in BonesData target)
        {
            if (target == null)
                return true;
            var dist = Vector3.Distance(points[0].localPosition, target.rootPos);
            var a1 = Quaternion.Angle(points[0].localRotation, target.rotations[0]);
            var a2 = Quaternion.Angle(points[13].localRotation, target.rotations[13]);
            var a3 = Quaternion.Angle(points[24].localRotation, target.rotations[24]);
            if (a1 < 0.001f && a2 < 0.001f && a3 < 0.001f)
            {
                return true;
            }
            return false;
        }

        public Vector3 positionOffset { get; set; } = Vector3.zero;
        public Quaternion rotationOffset { get; set; } = Quaternion.identity;
        public void UpdateJoint(XRHandJointID index, in Vector3 position)=>
            UpdateJoint(XRHandJointIdToCustom[index.ToIndex()], position);
        public void UpdateJoint(XRHandJointID index, in Quaternion rotation) =>
            UpdateJoint(XRHandJointIdToCustom[index.ToIndex()], rotation);
        public void UpdateJoint(int handMeshId, in Vector3 position)
        {
            points[handMeshId].localPosition = positionOffset + Vector3.Lerp( points[handMeshId].localPosition, positionOffset + position, Time.deltaTime*positionSpeed);
        }

        public void UpdateJoint(int handMeshId, in Quaternion rotation)
        {
            points[handMeshId].localRotation = Quaternion.Lerp(points[handMeshId].localRotation,
                rotation * rotationOffset, Time.deltaTime * rotationSpeed);
        }
        public void UpdateJoints(in Quaternion[] rotation)
        {
            
            for(int i = 0; i < rotation.Length; i ++)
            {
                UpdateJoint(i, rotation[i]);
            }
        }
    }
}