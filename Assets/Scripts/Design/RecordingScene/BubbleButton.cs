using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace Design.RecordingScene
{
    public class BubbleButton : BubbleItem
    {
        public UnityEvent onClick;

        protected override void OnHoverEntered() => OnClick();

        protected override void OnHoverExited()
        {
            throw new System.NotImplementedException();
        }

        protected override void UpdateProperties()
        {
            UpdateProp(ref emissive, animSpeed);
            UpdateProp(ref size, animSpeed);
            if (!emissive.isEqual)
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
                    size.to = defaultSize * 1.3f;
                    break;
                case 1:
                    emissive.to = 0;
                    size.to = defaultSize * 1f;
                    break;
            }
        }

        private void OnClick()
        {
            step = 0;
            AnimateByStep();
            onClick?.Invoke();
        }
    }
}