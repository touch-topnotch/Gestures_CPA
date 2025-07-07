using UnityEngine;
namespace Scripts
{
    public class RecognizerCenter : MonoBehaviour
    {
        public Vector3 offset;
        public Transform body;
        private Transform root;
        private void Start()
        {
            offset = this.transform.localPosition;
            root = body.parent;
        }
        // Update is called once per frame
        void Update()
        {
            var transform1 = this.transform;
            var tr = body.TransformPoint(offset);
            transform1.position = new Vector3(tr.x, root.position.y, tr.z);
            transform1.rotation = body.rotation;
        }
    }
}
