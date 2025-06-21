using UnityEngine;
using UnityEngine.Events;

namespace Design.RecordingScene
{
    public class BubbleToggle : BubbleItem
    {
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

        public UnityEvent<bool> onValueChanged;

        protected override void OnHoverEntered()
        {
            isOn = interactable ? !isOn : isOn;
        }

        protected override void OnHoverExited()
        {
            throw new System.NotImplementedException();
        }

        protected override void UpdateProperties()
        {
            // UpdateProp(ref emissive, 10f);
            // UpdateProp(ref size, 10f);
            // if (!emissive.isEqual)
            //     _mat.SetColor("_EmissionColor", Color.Lerp(colorDisabled, colorEnabled, emissive.from));
            // if (!size.isEqual)
            //     transform.localScale = Vector3.one * size.from;
        }

        private void OnValueChanged()
        {
            if (_isOn)
            {
                emissive.to = 1;
                size.to = defaultSize * 1.2f;
            }
            else
            {
                emissive.to = 0;
                size.to = defaultSize;
            }

            onValueChanged?.Invoke(_isOn);
        }
    }
}