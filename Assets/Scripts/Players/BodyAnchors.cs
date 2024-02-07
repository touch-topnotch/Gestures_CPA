using System;
using Scripts.Hands;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    public class BodyAnchors: MonoBehaviour
    {
        public Transform Body;
        public Transform Head;
        public HandsInformation HandsInformation;

        private void OnValidate()
        {
            if (Body == null)
                Body = transform.Find("Body");
            if (Head == null)
                Head = transform.Find("Head");
        }
    }
}