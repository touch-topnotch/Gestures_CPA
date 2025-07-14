using UnityEngine;
namespace Scripts
{
    public class RecognizerCenter : MonoBehaviour
    {
        public Vector3 offset;
        public Transform body;
        private Transform root;
        [Range(0, 10f)] private float positionLerp = 0.8f;
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
            transform1.position = Vector3.Lerp(transform1.position, new Vector3(tr.x, root.position.y, tr.z),
                positionLerp * Time.deltaTime);
            transform1.rotation = Quaternion.Lerp(transform1.rotation, body.rotation, positionLerp * Time.deltaTime);
           
        }
    }
}
