using System.Linq;
using Scripts.Static;
using Unity.Netcode;
using UnityEngine;

namespace Scripts.HandsLogic
{
    public struct HandAnchor: INetworkSerializable
    {
        public Vector3 rootPos;
        public Vector3[] rotations;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref rootPos);
            
            int length = rotations != null ? rotations.Length : 26;
            serializer.SerializeValue(ref length);
 
            if(serializer.IsReader)
            {
                rotations = new Vector3[length];
            }
 
            for (int n = 0; n < length; ++n)
            {
                if (rotations != null)
                {
                    serializer.SerializeValue(ref rotations[n]);
                }
                else
                {
                    Vector3 temp = new Vector3();
                    serializer.SerializeValue(ref temp);
                }
            }
        }
        public HandAnchor(Vector3 rootPos, Vector3[] rotations)
        {
            this.rootPos = rootPos;
            this.rotations = rotations ?? new Vector3[26];
        }
       
        // public override string ToString()
        // {
        //     var s = $"{rootPos.ToString()}\n";
        //     if (rotations != null)
        //     {
        //         for (int i = 0; i < rotations.Length; i++)
        //         {
        //             s = $"{s}{rotations[i].ToString()}\n";
        //         }
        //     }
        //
        //     return s.Remove(s.Length - 1);
        // }
        public static implicit operator string(HandAnchor anchor) => anchor.ToString();
        
        // public static implicit operator HandAnchor(string serialization)
        // {
        //     var vectors = VectorConverter.convertToVector3(serialization.Split('\n'));
        //     return new HandAnchor()
        //     {
        //         rootPos = vectors[0],
        //         rotations = vectors.Skip(0).ToArray()
        //     };
        // }
        
    }

    public class HandsSynchronizer: NetworkBehaviour
    {

        private NetworkVariable<HandAnchor> _left = new NetworkVariable<HandAnchor>();
        private NetworkVariable<HandAnchor> _right = new NetworkVariable<HandAnchor>();
        private Vector3 tempLeftRot = new Vector3();
        
        [ServerRpc(RequireOwnership = false)]
        public void RecordHandAnchorServerRpc(Vector3 _rootPos, Vector3[] _rotations, HandType type)
        {
            if (type == HandType.left)
            {
                _left.Value = new HandAnchor(_rootPos, _rotations);
                if (tempLeftRot != _rotations[0])
                {
                    Debug.Log($"Trying to save new position of {transform.name} - {_left.Value.rotations[0]}");
                    tempLeftRot = _rotations[0];
                }
            }
            else
            {
                _right.Value = new HandAnchor(_rootPos, _rotations);
            }
        }

        public HandAnchor GetLeftAnchor()
        {
            // if (_left.Value.rotations == null)
            // {
            //     return new HandAnchor() { rootPos = Vector3.up, rotations = new Vector3[26] };
            // }
            if (tempLeftRot != _left.Value.rotations[0])
            {
                Debug.Log($"Left rotation of Enemy {transform.name} changed - {_left.Value.rotations[0]}");
                tempLeftRot = _left.Value.rotations[0];
            }
            return _left.Value;
        }

        public HandAnchor GetRightAnchor() => _right.Value;
    }
}
