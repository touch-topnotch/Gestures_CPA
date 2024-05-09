using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace Design.RecordingScene
{
    [RequireComponent(typeof(XRSimpleInteractable), typeof(MeshRenderer))]
    public abstract class BubbleItem : MonoBehaviour
    {
        public Color colorEnabled;
        public Color colorDisabled;
        protected float animSpeed;
        protected XRSimpleInteractable _xrSimpleInteractable;

        protected Material _mat;
        protected float defaultSize;
        protected FromToProp emissive;
        protected FromToProp size;

        private bool _interactable;

        public bool interactable
        {
            get => _interactable;
            set
            {
                _interactable = value;
                _xrSimpleInteractable.hoverEntered.AddListener((a) =>
                {
                    if (_interactable)
                        OnHoverEntered();
                });
            }
        }

        protected abstract void OnHoverEntered();
        protected abstract void OnHoverExited();

        private void Awake()
        {
            defaultSize = transform.localScale.x;
            size = new(defaultSize, defaultSize);
            emissive = new(1, 0);
            if (!_xrSimpleInteractable)
                _xrSimpleInteractable = GetComponent<XRSimpleInteractable>();
            if (!_mat)
                _mat = GetComponent<MeshRenderer>().sharedMaterial;
            _xrSimpleInteractable.hoverEntered.AddListener((a) =>
            {
                if (_interactable)
                    OnHoverEntered();
            });
            _mat.EnableKeyword("_EMISSION");
        }

        protected void UpdateProp(ref FromToProp prop, float speed)
        {
            if (prop.isEqual)
                return;

            prop.from = Mathf.Lerp(prop.from, prop.to, speed * Time.deltaTime);
        }

        protected abstract void UpdateProperties();

        private void Update()
        {
            UpdateProperties();
        }


        protected struct FromToProp
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
                isEqual = (Mathf.Abs(_from - _to) < 0.001);
            }

            public override string ToString()
            {
                return "From: " + _from + ", To: " + to + ", IsEqual: " + isEqual;
            }
        }
    }
}