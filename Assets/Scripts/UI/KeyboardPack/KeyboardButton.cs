using System;
using UnityEngine;
using UnityEngine.UI;
using XRInteraction;

namespace UI.KeyboardPack
{
   public enum ButtonType
    {
        capslock,
        enter, 
        backspace,
        simple
    }
    [RequireComponent(typeof(XRPokeFollowAffordance))]
    public class KeyboardButton : MonoBehaviour
    {
        public ButtonType type;
        [SerializeField] private Button button;
        [SerializeField] private Text text;

        public void Awake()
        {
            if(button)
                button.onClick.AddListener(() => OnClick?.Invoke(text.text));
        }
        public void SetText(string value)
        {
            text.text = value;
        }

        public char GetText()
        {
            return text.text[0];
        }

        public event Action<string> OnClick;
    }
}
