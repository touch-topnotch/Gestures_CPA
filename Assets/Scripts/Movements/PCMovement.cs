using Scripts.Static;
using UnityEngine;
using Zenject;

namespace Scripts.Movements
{
    public class PCMovement: XRMovement
    {
        
        protected override void UpdateVelocity()
        {
            var x = Input.GetAxis("Horizontal");
            var z = Input.GetAxis("Vertical");
            var y = -Input.GetAxis("Debug Horizontal");
            var input = new Vector3(x, y, z);
            headAnchor.position += input * 0.005f;
            if (input == Vector3.zero)
                headAnchor.position = Vector3.Lerp(headAnchor.position, pivot.position, 3f * Time.deltaTime);
            base.UpdateVelocity();
        }
    }
}