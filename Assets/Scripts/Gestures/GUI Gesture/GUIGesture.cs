using System.Collections.Generic;
using Scripts.Hands;
using Scripts.Static;
using UnityEngine;
using Zenject;

namespace Scripts.Gestures.GGUI
{

    public abstract class GUIGesture
    {
        protected UserHands PlayerHands;
        protected abstract void ShowEffects(int frameId, GestureFrame gFrame);
        protected GUIGesture(ref FrameDetected onFrameDetected)
        {
            onFrameDetected += ShowEffects;
        }
        public virtual void Construct(UserHands hands)
        {
            PlayerHands = hands;
        }
        
        protected GameObject LoadAsset(in Object asset,Transform parent)
        {
            var prefab = Spawner.SpawnPrefab(asset as GameObject,parent, true);
            prefab.SetActive(false);
            return prefab;
        }
        protected GameObject LoadAsset(in Object asset, Transform parent, in Vector3 offset)
        {
            var transform = parent;
            transform.position += offset;
            var prefab = Spawner.SpawnPrefab(asset as GameObject, transform, true);
            prefab.SetActive(false);
            return prefab;
        }
        protected GameObject LoadAsset(in Object asset, Transform parent, in Vector3 offset, in Vector3 rotation)
        {
            var transform = parent;
            transform.position += offset;
            transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            var prefab = Spawner.SpawnPrefab(asset as GameObject, transform, true);
            prefab.SetActive(false);
            return prefab;
        }
    }
}