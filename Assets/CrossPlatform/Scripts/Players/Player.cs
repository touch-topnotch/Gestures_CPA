using System.Collections;
using CrossPlatform.Gestures;
using UnityEngine;

namespace CrossPlatform.PlayerLogic
{
    [RequireComponent(typeof(SupportHandCreator))]
    public abstract class Player : MonoBehaviour
    {
        public SupportHandCreator SupHandCreator { get; private set; }
        public virtual void Initialize()
        {
            SupHandCreator = GetComponent<SupportHandCreator>();
        }

        public abstract Vector3[] GetLeftHandPoints();
        public abstract Vector3[] GetRightHandPoints();
        
    }

}
