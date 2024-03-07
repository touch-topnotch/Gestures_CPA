
using System;
using UnityEditor;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    public enum AvatarType
    {
        Local,
        Enemy,
        None
    } 
    public class Avatar: MonoBehaviour
    {
        public AvatarType type;
        [Space]
        public BodyAnchors Anchors;
        private float startHeight;
        [SerializeField]
        private float currentHeight;

        private void Start()
        {
            startHeight = Anchors.Head.position.y;
            currentHeight = startHeight;
        }
        private void Update()
        {
            if (Anchors.Head && Anchors.Body)
            {
                
                if (Physics.Raycast(Anchors.Head.position, Vector3.down, out var hit, 100, layerMask:7))
                {
                    currentHeight = hit.distance;
                    var scale = Anchors.Body.localScale;
                    var difference = hit.distance / startHeight;
                    Anchors.Body.localScale = new Vector3(scale.x,difference , scale.z);
                }

            }
        }
        void OnDrawGizmos()
        {
            // Draws a blue line from this transform to the target
            Gizmos.color = Color.red;
            var position = Anchors.Head.position;
            Gizmos.DrawLine(position,  new Vector3(position.x, position.y - currentHeight, position.z));
            
        }
    }
} 