using UnityEngine;

namespace Scripts.Tests
{
    public class GestureCreationTest : MonoBehaviour
    {
        public Transform parent;
        public Transform child;
        public Transform rezult;

        public void Update()
        {
            rezult.position = parent.InverseTransformPoint(child.position);
        }
    }
}