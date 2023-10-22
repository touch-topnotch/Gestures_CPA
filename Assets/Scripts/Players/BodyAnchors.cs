using System;
using Scripts.Hands;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    [Serializable]
    public class BodyAnchors: MonoBehaviour
    {
        public Transform Body;
        public Transform Head;
        public AnchorHand Left;
        public AnchorHand Right;
    }
}