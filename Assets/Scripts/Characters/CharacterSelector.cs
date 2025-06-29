using UnityEngine;

namespace Scripts.Characters
{
    public class CharacterSelector : MonoBehaviour
    {
        
        [SerializeField] private CharacterController _characterController;

        private void OnValidate()
        {
            if (_characterController == null)
            {
                _characterController = this.gameObject.GetComponentInChildren<CharacterController>();
            }
        }

        private void Start()
        {
         //   _characterController.SetCharacter(currentCharacter.ToString());
        }
    }
}