using UnityEngine;

namespace Scripts.Tests
{
    [RequireComponent(typeof(Camera))]
    internal class DebugCamera : MonoBehaviour
    {
        private Camera _camera;
        private bool _enabled;

        private void Start()
        {
            _camera = GetComponent<Camera>();
        }

        // Update is called once per frame
        private void Update()
        {
            _camera.enabled = FindObjectsOfType<Camera>().Length < 2;
        }
    }
}