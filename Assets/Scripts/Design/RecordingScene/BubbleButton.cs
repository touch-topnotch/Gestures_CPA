namespace Design.RecordingScene
{
using System;
using Scripts.HandsLogic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace Design.RecordingScene
{
    [RequireComponent(typeof(XRSimpleInteractable), typeof(MeshRenderer))]
    public class BubbleButton : MonoBehaviour
    {
        public Color colorEnabled;
        public Color colorDisabled;
        private XRSimpleInteractable _xrSimpleInteractable;

        private Material _mat;
        private float _defaultSize;
        private FromToProp emissive;
        private FromToProp size;
        private bool _interactable;
        public UnityEvent onClick;
        public bool interactable
        {
            get => _interactable;
            set
            {
                _interactable = value;
               _xrSimpleInteractable.hoverEntered.AddListener((a) => {if(_interactable) OnClick();});
            }
        }

        private void Awake()
        {
            
            _defaultSize = transform.localScale.x;
            size = new(_defaultSize, _defaultSize);
            emissive = new(1, 0);
            _xrSimpleInteractable.hoverEntered.AddListener((a) => { OnClick();});
            _mat.EnableKeyword("_EMISSION");
        }
        private void OnValidate()
        {
            _xrSimpleInteractable = GetComponent<XRSimpleInteractable>();
            _mat = GetComponent<MeshRenderer>().sharedMaterial;
            
            if (_xrSimpleInteractable.colliders.Count == 0)
                _xrSimpleInteractable.colliders.Add(GetComponent<SphereCollider>());
            else
                _xrSimpleInteractable.colliders[0] = GetComponent<SphereCollider>();
        }

 

        private void UpdateProp(ref FromToProp prop, float speed)
        {
            if(prop.isEqual) 
                return;
            
            prop.from = Mathf.Lerp(prop.from, prop.to, speed * Time.deltaTime);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OnClick();
                Debug.Log("Hover simulation");
            }
            
            UpdateProp(ref emissive, 15f);
            UpdateProp(ref size, 15f);
            if(!emissive.isEqual)
                _mat.SetColor("_EmissionColor", Color.Lerp(colorDisabled, colorEnabled, emissive.from));
            if (!size.isEqual)
                transform.localScale = Vector3.one * size.from;
            if (emissive.isEqual && size.isEqual && step < 2)
            {
                step++;
                AnimateByStep();
            }
        }

        private int step = 2;
        private void AnimateByStep()
        {
            switch (step)
            {
                case 0:
                    emissive.to = 1;
                    size.to = _defaultSize * 1.3f;
                    break;
                case 1:
                    emissive.to = 0;
                    size.to = _defaultSize * 1f;
                    break;
            }
            
          
        }
        private void OnClick()
        {
            step = 0;
            AnimateByStep();
            onClick?.Invoke();
        }

        struct FromToProp
        {
            private float _from;
            private float _to;
            public float from
            {
                get => _from;
                set
                {
                    _from = value;
                    isEqual = (Mathf.Abs(_from - _to) < 0.001);
                }
            }

            public float to
            {
                get => _to;
                set
                {
                    _to = value;
                    isEqual = (Mathf.Abs(_from - _to) < 0.001);
                }
            }
            public bool isEqual;
            public FromToProp(float from, float to)
            {
                _from = from;
                _to = to;
                isEqual =  (Mathf.Abs(_from - _to) < 0.001);
            }

            public override string ToString()
            {
                return "From: " + _from + ", To: " + to + ", IsEqual: " + isEqual;
            }
        }
    }
}

}