using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    public class Loading : MonoBehaviour
    {
        [SerializeField] private Image rotatedImage;
        public Vector3 Offset = new Vector3(0, 0, 1);

        void Update()
        {
            rotatedImage.rectTransform.transform.eulerAngles += Offset;
        }
    }
}