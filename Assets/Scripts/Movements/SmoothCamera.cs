using UnityEngine;

namespace Scripts.Movements
{
    public class SmoothCamera : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private Transform target;

        // Update is called once per frame
        void Update()
        {
            transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, speed * Time.deltaTime);
        }
    }
}