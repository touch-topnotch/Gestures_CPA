using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.Static;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Scripts.Hands
{
    public enum HandType
    {
        left,
        right
    }
    public class HandMesh : MonoBehaviour
    {
        public Material material;
        public Transform[] points;
        [SerializeField] private Material _defaultMaterial;
        public Color selectedFingerColor;
        public Color selectedEdgeColor;
        private readonly List<TargetProp> _targets = new ();
        private readonly List<PinPongProp> _pinPongs = new ();
        protected UpdateEvent onUpdate;

        private void OnValidate()
        {
            ResetMaterial();
        }

        [Inject]
        protected void Construct(UpdateEvent updateEvent)
        {
            onUpdate = updateEvent;
            onUpdate.AddListener(UpdateProperties);
        }
        private void Start()
        {
            if(_defaultMaterial != null)
                ResetMaterial();
        }

        public void ResetMaterial()
        {
            material.SetColor(HandShaderProps.MainColor, _defaultMaterial.GetColor(HandShaderProps.MainColor));
            material.SetColor(HandShaderProps.EdgeColor, _defaultMaterial.GetColor(HandShaderProps.EdgeColor));
            material.SetFloat(HandShaderProps.EdgeHighlightPower, _defaultMaterial.GetFloat(HandShaderProps.EdgeHighlightPower));
            material.SetColor(HandShaderProps.ThumbColor, _defaultMaterial.GetColor(HandShaderProps.ThumbColor));
            material.SetColor(HandShaderProps.FingerColor1, _defaultMaterial.GetColor(HandShaderProps.FingerColor1));
            material.SetColor(HandShaderProps.FingerColor2, _defaultMaterial.GetColor(HandShaderProps.FingerColor2));
            material.SetColor(HandShaderProps.FingerColor3, _defaultMaterial.GetColor(HandShaderProps.FingerColor3));
            material.SetColor(HandShaderProps.FingerColor4, _defaultMaterial.GetColor(HandShaderProps.FingerColor4));
            material.SetVector(HandShaderProps.FadeCenter, _defaultMaterial.GetVector(HandShaderProps.FadeCenter));
            material.SetVector(HandShaderProps.FadeScale, _defaultMaterial.GetVector(HandShaderProps.FadeScale));
            material.SetFloat(HandShaderProps.FadeStart, _defaultMaterial.GetFloat(HandShaderProps.FadeStart));
            material.SetFloat(HandShaderProps.NoiseScale, _defaultMaterial.GetFloat(HandShaderProps.NoiseScale));
            material.SetFloat(HandShaderProps.NoiseStrength, _defaultMaterial.GetFloat(HandShaderProps.NoiseStrength));
        }

        public void SetFingersColor(in Color color, in bool isSmooth = false)
        {
            foreach (int prop in HandShaderProps.FingerNames)
            { 
                if(isSmooth)
                    SetColorSmooth(prop, color);
                else
                    material.SetColor(prop, color);
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

        public void SetSelectedStyle()
        {
            ResetMaterial();
            material.SetColor(
                HandShaderProps.EdgeColor, selectedEdgeColor);
            material.SetColor(HandShaderProps.FingerColor1, selectedFingerColor);
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
            material.SetColor(prop.id, 
                Color.Lerp(material.GetColor(prop.id), prop.value, 
                    Time.deltaTime * prop.speed));
            return Recognizer.OptimizedDistance(prop.value, material.GetColor(prop.id)) >= 0.001f;
        }

        private void OnDestroy()
        {
            ResetMaterial();
        }
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

    public static class HandShaderProps
    { 
        public static readonly int MainColor = Shader.PropertyToID("_MainColor");
        public static readonly int EdgeColor = Shader.PropertyToID("_EdgeColor");
        public static readonly int EdgeHighlightPower = Shader.PropertyToID("_EdgeHighlightPower");
        public static readonly int ThumbColor = Shader.PropertyToID("_ThumbColor");
        public static readonly int FingerColor1 = Shader.PropertyToID("_FingerColor_1");
        public static readonly int FingerColor2 = Shader.PropertyToID("_FingerColor_2");
        public static readonly int FingerColor3 = Shader.PropertyToID("_FingerColor_3");
        public static readonly int FingerColor4 = Shader.PropertyToID("_FingerColor_4");
        public static readonly int FadeCenter = Shader.PropertyToID("_FadeCenter");
        public static readonly int FadeScale = Shader.PropertyToID("_FadeScale");
        public static readonly int FadeStart = Shader.PropertyToID("_FadeStart");
        public static readonly int NoiseScale = Shader.PropertyToID("_NoiseScale");
        public static readonly int NoiseStrength = Shader.PropertyToID("_NoiseStrength");
        public static readonly int[] FingerNames = new[] { FingerColor1, FingerColor2, FingerColor3, FingerColor4 };
        public static readonly int[] AllColors = new[] { MainColor, EdgeColor, ThumbColor, FingerColor1, FingerColor2, FingerColor3, FingerColor4 };
    }
}