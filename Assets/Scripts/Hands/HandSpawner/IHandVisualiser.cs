using Scripts.Events;
using UnityEngine;

namespace Scripts.Hands
{
    public interface IHandVisualiser
    {
        public void Initialize(ref UpdateEvent onUpdate);
        public void ChangePosition(BonesData data, Transform parent = null);
        public void ChangePositionSmooth(BonesData data, Transform parent = null);
        public Transform[] GetTransforms();
        public void Show();
        public void Hide();
        
    }
}