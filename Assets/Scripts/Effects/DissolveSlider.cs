using UnityEngine;

namespace Scripts.Effects
{
    public class DissolveSlider : MonoBehaviour
    {
        [SerializeField] private float dissolveToValue = 0f;

        private float _dissolvePreviousValue;

        private bool _activated = true;
        private Material _material;


        private float _fraction = 0;
        private static readonly int Disappear = Shader.PropertyToID("_Disappear");

        void Start()

        {
            // Debug.Log("Disappear id: " + Disappear);
            _material = GetComponent<Renderer>().material;
            _material.SetFloat(Disappear, dissolveToValue);

            _dissolvePreviousValue = dissolveToValue;
        }

        void FixedUpdate()
        {
            if (!_activated) return;

            if (_fraction >= 1) return;


            _fraction += Time.deltaTime;
            if (_fraction >= 1) _fraction = 1;
            float newValue = Mathf.Lerp(_dissolvePreviousValue, dissolveToValue, _fraction);
            _material.SetFloat(Disappear, newValue);
        }

        public void UpdateDisolveValue(float val)
        {
            _fraction = 0;
            _dissolvePreviousValue = dissolveToValue;
            if (val > 1) val = 1;
            else if (val < 0) val = 0;
            else dissolveToValue = val;
        }

        public void Activate()
        {
            _activated = true;
        }
    }
}