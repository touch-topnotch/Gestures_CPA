using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gesture_Editor_SDK.EditorAttributes.InspectorButtonAttribute;
using Scripts.Design;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.Static;
using Sirenix.OdinInspector;
using UnityEngine;
using Timer = Scripts.Static.Timer;

namespace Scripts.HandsLogic
{
    public enum HandType
    {
        left,
        right
    }
    public class HandMesh : MonoBehaviour
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
        
        private Action _onPlaced;
        private float _speed;
        private bool _isMoved;
        private BonesData _target;


        [Button("Add missing components")]
        public void RefreshProperties()
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
        
        private void Start()
        {
            onUpdate.AddListener(UpdateProperties);
        }
        
        public void ChangePosition(BonesData data)
        {
            if (!data.Exists())
            {
                Hide();
                return;
            }

            Show();

            points[0].localPosition = data.rootPos;
            for (int i = 0; i < data.rotations.Length; i++)
            {
                points[i].localRotation = data.rotations[i];
            }
        }

        public void ChangePositionSmooth(in BonesData data, in float speed, in Action onPlaced = null)
        {
            if (data == null || data.rotations == null || data.rotations.Length == 0)
                return;
            _target = data;
            _speed = speed;
            _onPlaced = onPlaced;
            if (!_isMoved)
                onUpdate.AddListener(MoveHand);
        }

        
        private void MoveHand()
        {  if (_target == null || _target.rotations == null)
            {
                _onPlaced = null;
                StopMoveHand();
                return;
            }

            var dist = Vector3.Distance(points[0].localPosition, _target.rootPos);
            var a1 = Quaternion.Angle(points[0].localRotation, _target.rotations[0]);
            var a2 = Quaternion.Angle(points[13].localRotation, _target.rotations[13]);
            if(dist < 0.05f && a1 < 0.05f&& a2< 0.05f)
            {
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
            onUpdate.RemoveListener(MoveHand);
        }
        
        public void Show()
        {
            StopPinPongAll();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            StopPinPongAll();
            SetColorSmooth(HandShaderProps.EdgeColor, Color.clear);
            SetFingersColor(Color.clear, true);
            var timer = new Timer(0.4f, () =>
            {
                gameObject.SetActive(false);
            },onUpdate);
        }

        public void SetFingersColor(in Color color, in bool isSmooth = false)
        {
            foreach (int prop in HandShaderProps.FingerNames)
            { 
                if(isSmooth)
                    SetColorSmooth(prop, color);
                else
                    HandMaterial.SetColor(prop, color);
            }
        }

        public void SetColorSmooth(int property, in Color color, in float speed = 1)
        {
            for (int i = 0; i < _targets.Count; i++)
            {
                if (_targets[i].id == property)
                {
                    _targets[i] = new TargetProp(property, color, speed);
                    return;
                }
            }
            _targets.Add(new TargetProp(property, color, speed));
            
        }
        
        public void ChangeColorPinPong(in int id, in Color a, in Color b, in float speed)
        {
            _pinPongs.Add(new PinPongProp(id, a, b, speed));
        }

        public void StopPinPonging(in int id)
        {
            foreach (var pinPongProp in _pinPongs)
            {
                if(pinPongProp.target.id == id)
                    _pinPongs.Remove(pinPongProp);
            }
        }

        public void StopPinPongAll()
        {
            _pinPongs.Clear();
        }
        
        private void UpdateProperties()
        {
            if (_targets.Count != 0)
            {
                for (int i = 0; i < _targets.Count; i++)
                {
                    if (!TryLerpTargetProp(_targets[i]))
                    {
                        _targets.RemoveAt(i);
                        i--;
                    }
                }
            }

            if (_pinPongs.Count != 0){
                for (int i = 0; i < _pinPongs.Count; i++)
                {
                    if (!TryLerpTargetProp(_pinPongs[i].target))
                    {
                        _pinPongs[i].Revert();
                    }
                }
            }
        }

        private bool TryLerpTargetProp(in TargetProp prop)
        {
            HandMaterial.SetColor(prop.id, 
                Color.Lerp(HandMaterial.GetColor(prop.id), prop.value, 
                    Time.deltaTime * prop.speed));
            return Recognizer.OptimizedDistance(prop.value, HandMaterial.GetColor(prop.id)) >= 0.001f;
        }

        private void OnDestroy()
        {
          //  material
        }
        
        // public Transform[] GetTransforms()
        // {
        //     throw new NotImplementedException();
        // }
    }

    struct TargetProp
    {
        public readonly int id;
        public readonly Color value;
        public readonly float speed;
        public TargetProp(int id, Color value, float speed = 1)
        {
            this.id = id;
            this.value = value;
            this.speed = speed;
        }
    }

    class PinPongProp
    {
        public readonly TargetProp a;
        public readonly TargetProp b;
        public TargetProp target;
        public PinPongProp(int id, Color a, Color b, float speed = 1)
        {
            this.a = new TargetProp(id, a, speed);
            this.b = new TargetProp(id, b, speed);
            this.target = this.a;
        }

        public void Revert()
        {
            target = a.value == target.value ? b : a;
        }
        
    }
    

 
}