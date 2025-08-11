using System.Collections;
using System.Collections.Generic;
using Scripts.PlayerLogic;
using Scripts.Players;
using UnityEngine;

namespace Scripts
{
    public class CharacterReflection : MonoBehaviour
    {
        private BodyAnchors _bodyAnchors;
        private const string LayerName = "NotVIsible";
        private readonly float _scaleMultiplier = 1.3f;
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _bodyAnchors = GetComponent<BodyAnchors>();
            
            int notVisibleLayer = LayerMask.NameToLayer(LayerName);
            var children = transform.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var child in children)
            {
                child.gameObject.layer = notVisibleLayer;
            }
            
            transform.localScale *= _scaleMultiplier;
        }

        
        private void LateUpdate()
        {
            var playerData = PlayerData.local;
            _bodyAnchors.Root.transform.SetPositionAndRotation(playerData.anchors.Root.transform.position, playerData.anchors.Root.transform.rotation);
            _bodyAnchors.Body.transform.SetPositionAndRotation(playerData.anchors.Body.transform.position, playerData.anchors.Body.transform.rotation);

            var headTransformPosition = new Vector3(playerData.anchors.Head.transform.position.x,
                playerData.anchors.Head.transform.position.y * _scaleMultiplier,
                playerData.anchors.Head.transform.position.z);
            _bodyAnchors.Head.transform.SetPositionAndRotation(headTransformPosition, playerData.anchors.Head.transform.rotation);
        }
    }
}
