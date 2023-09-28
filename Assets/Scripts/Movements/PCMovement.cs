using Scripts.Static;
using UnityEngine;
using Zenject;

namespace Scripts.Movements
{
    public class PCMovement: Movement
    {
        public float playerSpeed = 2f;
        public float mouseSensitivity = 2f;
        public float jumpHeight = 3f; 
        private bool isMoving = false;
        private bool isSprinting =false;
        private float yRot;
        
        protected override void UpdateVelocity()
        {
            yRot += Input.GetAxis("Mouse X") * mouseSensitivity;
            anchors.Body.localEulerAngles = new Vector3(transform.localEulerAngles.x, yRot, transform.localEulerAngles.z);
 
            isMoving = false;
            Vector3 velocity = new Vector3();
            if (Input.GetAxisRaw("Horizontal") > 0.5f || Input.GetAxisRaw("Horizontal") < -0.5f)
            {
                //transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * playerSpeed);
                velocity += transform.right * Input.GetAxisRaw("Horizontal") * playerSpeed;
                isMoving = true;
            }
            if (Input.GetAxisRaw("Vertical") > 0.5f || Input.GetAxisRaw("Vertical") < -0.5f)
            {
                //transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * playerSpeed);
                velocity += transform.forward * Input.GetAxisRaw("Vertical") * playerSpeed;
                isMoving = true;
            }
 
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity += Vector3.up * jumpHeight;
            }

            parentMoveController.Move(velocity);
        }
    }
}