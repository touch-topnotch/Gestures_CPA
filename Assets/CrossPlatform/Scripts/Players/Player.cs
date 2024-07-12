using System.Collections;
using UnityEngine;

namespace CrossPlatform.PlayerLogic
{
    public abstract class Player : MonoBehaviour
    {
        public abstract void Initialize();
        public abstract Vector3[] GetLeftHandPoints();
        public abstract Vector3[] GetRightHandPoints();
        
    }

}
