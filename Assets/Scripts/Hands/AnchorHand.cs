using Unity.Netcode;
using UnityEngine;

namespace Scripts.Hands
{
    public class AnchorHand: MonoBehaviour
    {

        public Transform[] DebugPoints;
        
        private NetworkVariable<Vector3> rootPos = new NetworkVariable<Vector3>();
        private NetworkVariable<Vector3[]> _rotations = new NetworkVariable<Vector3[]>();
        private HandType _type;
        
        public HandType Type() => _type;
    }

    
}

