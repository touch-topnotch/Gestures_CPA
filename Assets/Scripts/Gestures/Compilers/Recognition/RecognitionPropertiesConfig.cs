using UnityEngine;

namespace Scripts.Gestures
{
    [CreateAssetMenu(fileName = "_Recognition_Properties", menuName = "Config/RecognitionProperties")]
    public class RecognitionPropertiesConfig : ScriptableObject
    {
        [SerializeField] private RecognitionProperties playerProperties;
        [SerializeField] private RecognitionProperties supportiveProperties;

        public RecognitionProperties PlayerProperties => playerProperties;
        public RecognitionProperties SupportiveProperties => supportiveProperties;
    }
}