using System;
using Scripts.Static.Definitions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    public class Avatar : MonoBehaviour
    {
        public AvatarType type;
        [Space] public BodyAnchors Anchors;
        [DisableInPlayMode] [SerializeField] private float startHeight;
        [SerializeField] private float currentHeight;
        private float maxScale = 1f;

        private void Start()
        {
            startHeight = 1.8f;
            if (Physics.Raycast(Anchors.Head.position, Vector3.down, out var hit, 100, layerMask: ~0 & (1 << 9)))
            {
                startHeight = hit.distance;
            }

            currentHeight = startHeight;
        }

        private void Update()
        {
            if (Anchors.Head && Anchors.Body)
            {
                if (Physics.Raycast(Anchors.Head.position, Vector3.down, out var hit, 100, layerMask: ~0 & (1 << 9)))
                {
                    currentHeight = hit.distance;
                    var scale = Anchors.Body.localScale;

                    var difference = Mathf.Clamp(hit.distance / startHeight, 0.2f, maxScale);
                    Anchors.Body.localScale = new Vector3(scale.x, difference, scale.z);
                }
            }
        }

        void OnDrawGizmos()
        {
            // Draws a blue line from this transform to the target
            Gizmos.color = Color.red;
            var position = Anchors.Head.position;
            Gizmos.DrawLine(position, new Vector3(position.x, position.y - currentHeight, position.z));
        }
    }
}