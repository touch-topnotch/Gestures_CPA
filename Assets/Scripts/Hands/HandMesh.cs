using System;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace Scripts.Hands
{
    public enum HandType
    {
        left,
        right
    }
    public class HandMesh : MonoBehaviour
    {
        public GameObject rootObject;
        public Material material;
        public Transform[] points;
        public HandType handType = HandType.left;

        private void OnValidate()
        {
            rootObject = this.gameObject;
        }
    }
}