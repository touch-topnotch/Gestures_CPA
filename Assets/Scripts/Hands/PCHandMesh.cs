
using Scripts.Events;
using UnityEngine;
using Zenject;

namespace Scripts.Hands
{
    public class PCHandMesh: HandMesh
    {
        [SerializeField] private float speed;
        private bool isMoved;
        private BonesData target;
      
        
        public void SetBonesSmooth(in BonesData data)
        {

            target = data;
            if (!isMoved)
                onUpdate.AddListener(MoveHand);
        }

        private void MoveHand()
        {  if (target == null || target.rotations == null)
            {
                StopMoveHand();
                return;
            }
            if(Vector3.Distance(points[0].position, target.rootPos) < 0.01f)
            {
                StopMoveHand();
                return;
            }
            
            points[0].localPosition = Vector3.Lerp(points[0].localPosition, target.rootPos, speed*Time.deltaTime);
            for(int i = 0; i < points.Length; i++)
            {
                points[i].localRotation = Quaternion.Lerp(points[i].localRotation, target.rotations[i], speed*Time.deltaTime);
            }
        }
        private void StopMoveHand()
        {
            onUpdate.RemoveListener(MoveHand);
        }
    }
}