using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{

    public class PCUI : MonoBehaviour
    {

        [SerializeField] private TMP_InputField _gestureInput;
        [SerializeField] private Toggle _handParentedToggle;
        [SerializeField] private GameObject _rootObject;
        public TMP_InputField gestureInput => _gestureInput;
        public Toggle handParentedToggle => _handParentedToggle;

        public void Show()
        {
            _rootObject.SetActive(true);
        }

        public void Hide()
        {
            _rootObject.SetActive(false);
        }

    }



}