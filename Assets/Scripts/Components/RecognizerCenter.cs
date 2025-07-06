using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Scripts
{
    public class RecognizerCenter : MonoBehaviour
    {
        public Vector3 offset;
        public Transform body;

        private void Start()
        {
            offset = this.transform.localPosition;
        }
        // Update is called once per frame
        void Update()
        {
            var transform1 = this.transform;
            transform1.position = body.TransformPoint(offset);
            transform1.rotation = body.rotation;
        }
    }
}
