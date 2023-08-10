using UnityEngine;

namespace Scripts.Movements
{
    public class BoardsVisualizer: MonoBehaviour
    {
        [SerializeField] private XRMovement _movement;
        public Transform mainCamera;
        public Transform xzBoard;
        public Transform yBoard;
        public bool GoToMainCamera;

        private void Update()
        {
            if(GoToMainCamera)
                xzBoard.position = new Vector3(xzBoard.position.x, mainCamera.position.y, xzBoard.position.z);
            xzBoard.localScale = new Vector3(_movement.XZBoard, xzBoard.localScale.y, _movement.XZBoard);
            yBoard.position = new Vector3(yBoard.position.x, _movement.YBoard, yBoard.position.z);
        }


    }
}