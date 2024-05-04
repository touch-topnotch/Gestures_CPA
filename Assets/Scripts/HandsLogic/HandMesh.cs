using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Scripts.Design;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using Scripts.Systems;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.HandsLogic
{
    public enum HandType
    {
        left,
        right
    }
    public class HandMesh : MonoBehaviour, IQueueVisualised<BonesData>, IColorable
    {
        private enum HandMaterialType
        {
            Player,
            Ghost
        }
        [Header("Types")]
        [SerializeField] private HandMaterialType _handMaterialType;
        [SerializeField] private HandType _handType;
        [Space]
        [Header("Transforms")]
        public Transform[] points;

        [SerializeField] private List<Material> _materials = new List<Material>();
        public Material HandMaterial
        {
            get => _meshRenderer.sharedMaterials[1];
            set
            {
                _materials[1] = value;
                _meshRenderer.SetMaterials(_materials);
            }
        }

        [SerializeField] private SkinnedMeshRenderer _meshRenderer;
        
        private readonly List<TargetProp> _targets = new ();
        
        private readonly List<PinPongProp> _pinPongs = new ();
        
        private UpdateEvent onUpdate => UpdateEvent.Instance;

        private bool _isPlaced;
        private Action _onPlaced;
        private float _speed;
        private bool _isMoved;
        private BonesData _target;
        private Tween _tween;
        

        [Button("Add missing components")]
        public void AddMissingComponents()
        {
            _handType = name[^1] == 'L' ? HandType.left : HandType.right;
            
            _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            _materials = _meshRenderer.sharedMaterials.ToList();
            
            if (points == null ||points.Length == 0)
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

        private int AddAllChildren(Transform parent, int id=0)
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
                _onPlaced = null;
                StopMoveHand();
                return;
            }

            _target.ListenAnchors(PlayerData.local.bodyAnchors);

            var dist = Vector3.Distance(points[0].localPosition, _target.rootPos);
            var a1 = Quaternion.Angle(points[0].localRotation, _target.rotations[0]);
            var a2 = Quaternion.Angle(points[13].localRotation, _target.rotations[13]);
            if(dist < 0.05f && a1 < 0.05f&& a2< 0.05f)
            {
                if(!_isPlaced)
                    StopMoveHand();
                return;
            }
            
            points[0].localPosition = Vector3.Lerp(points[0].localPosition, _target.rootPos, _speed*Time.deltaTime);
            
            for(int i = 0; i < points.Length; i++)
            {
                points[i].localRotation = Quaternion.Lerp(points[i].localRotation, _target.rotations[i], _speed*Time.deltaTime);
            }
           
        }
        
        
        private void StopMoveHand()
        {
            _onPlaced?.Invoke();
            _isPlaced = true;
            //onUpdate.RemoveListener(MoveHand);
        }

        public bool IsActive() => gameObject.activeSelf;
        public void Show()
        {
            gameObject.SetActive(true);
            Debug.Log("SHOW HAND");
        }

        public void Hide()
        {
            ChangeColorForProps(Color.clear, HandShaderProps.AllColors, new ColorParams(0, 1, false));
            
        }

        public void Replace(BonesData target)
        {
            if (!target.Exists())
            {
                Hide();
                return;
            }

            points[0].localPosition = target.rootPos;
            for (int i = 0; i < target.rotations.Length; i++)
            {
                points[i].localRotation = target.rotations[i];
            }
        }
        public void Move(BonesData target, float speed, Action onPlaced)
        {
            if (target == null || target.rotations == null || target.rotations.Length == 0)
                return;
            _target = target;
            _speed = speed;
            _onPlaced = onPlaced;
            _isPlaced = false;
            if (!_isMoved)
                onUpdate.AddListener(MoveHand);
        }

        public void Destroy()
        {
            HandMaterial.DOKill();
            GameObject.Destroy(this);
        }

        public void ChangeColorForProps(in Color color, in int[] props, in ColorParams pColorParams)
        {
            foreach (int prop in props)
            {
                if (pColorParams.speed > 0)
                {
                    ChangeColorSmooth(color, new ColorParams(prop, pColorParams), () =>
                    {
                        HandMaterial.DOKill();
                        this.gameObject.SetActive(false);
                    });
                }
                else
                {
                    ChangeColor(color, prop);
                    this.gameObject.SetActive(false);
                }
            }
        }
        public void ChangeColor(in Color color, int id)
        {
            HandMaterial.SetColor(id, color);
        }

        public void ChangeColorPinPong(Color active, Color passive, ColorParams pColorParams)
        {
            if(pColorParams.kill)
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
            if(pColorParams.kill)
                HandMaterial.DOKill();

            HandMaterial.DOColor(color, pColorParams.id, 1 / pColorParams.speed).onComplete =
                onComplete;
        }

       
    }

   
    

 
}