using System.Collections.Generic;
using Scripts.Events;
using Scripts.Hands;
using Scripts.Static;
using UnityEngine;
using Zenject;

namespace Scripts.Gestures.GGUI
{

    public abstract class GUIGesture
    {
        protected PlayerHands hands;
        protected abstract void Construct();
        public abstract void ShowEffects(int frameId, GestureFrame gFrame);

        protected GUIGesture()
        {
        }

        public void Construct(PlayerHands Hands)
        {
            hands = Hands;
            Construct();
        }
        protected GameObject LoadAsset(in Object asset,Transform parent)
        {
            var prefab = Spawner.SpawnPrefab(asset as GameObject,parent, true);
            return prefab;
        }
        protected GameObject LoadAsset(in Object asset, Transform parent, in Vector3 offset)
        {
            var transform = parent;
            transform.position += offset;
            var prefab = Spawner.SpawnPrefab(asset as GameObject, transform, true);
            return prefab;
        }
        protected GameObject LoadAsset(in Object asset, Transform parent, in Vector3 offset, in Vector3 rotation)
        {
            var transform = parent;
            transform.position += offset;
            transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            var prefab = Spawner.SpawnPrefab(asset as GameObject, transform, true);
            return prefab;
        }
    }
}