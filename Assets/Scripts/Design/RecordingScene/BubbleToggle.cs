using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace Design.RecordingScene
{
    [RequireComponent(typeof(XRPokeInteractor), typeof(MeshRenderer))]
    public class BubbleToggle : MonoBehaviour
    {
        public Color colorEnabled;
        public Color colorDisabled;
        private bool _isOn = false;
        public bool isOn
        {
            get => _isOn;
            set
            {
                _isOn = value;
                OnValueChanged();
            }
        }
        private XRPokeInteractor _xrSimpleInteractable;

        private Material _mat;
        private float _defaultSize;
        private FromToProp emissive;
        private FromToProp size;
        private bool _interactable;
        public UnityEvent<bool> onValueChanged;
        public bool interactable
        {
            get => _interactable;
            set
            {
                _interactable = value;
               _xrSimpleInteractable.hoverEntered.AddListener((a) => { isOn = interactable ? !isOn : isOn;});
            }
        }

        private void Awake()
        {
            
            _defaultSize = transform.localScale.x;
            size = new(_defaultSize, _defaultSize);
            emissive = new(1, 0);
            _xrSimpleInteractable.hoverEntered.AddListener((a) => { isOn = interactable ? !isOn : isOn;});
            _mat.EnableKeyword("_EMISSION");
        }
        private void OnValidate()
        {
            _xrSimpleInteractable = GetComponent<XRPokeInteractor>();
            _mat = GetComponent<MeshRenderer>().sharedMaterial;
        }

 

        private void UpdateProp(ref FromToProp prop, float speed)
        {
            if(prop.isEqual) 
                return;
            
            prop.from = Mathf.Lerp(prop.from, prop.to, speed * Time.deltaTime);
        }
        private void Update()
        {
            UpdateProp(ref emissive, 10f);
            UpdateProp(ref size, 10f);
            if(!emissive.isEqual)
                _mat.SetColor("_EmissionColor", Color.Lerp(colorDisabled, colorEnabled, emissive.from));
            if (!size.isEqual)
                transform.localScale = Vector3.one * size.from;
        }
        private void OnValueChanged()
        {
            if (_isOn)
            {
                emissive.to = 1;
                size.to = _defaultSize * 1.2f;
            }
            else
            {
                emissive.to = 0;
                size.to = _defaultSize;
            }

            onValueChanged?.Invoke(_isOn);

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
