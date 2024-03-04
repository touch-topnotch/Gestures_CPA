using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.KeyboardPack
{
    public class XRInputField: MonoBehaviour
    {
        [SerializeField] private Text inputText;
        [SerializeField] private Text supportiveText;

        private string _inputString;
        public string inputString
        {
            get => _inputString;
            set
            {
                _inputString = value;
                inputText.text = value;
            }
        }
        [SerializeField] private string supportiveString = "Click to write";
        [SerializeField] private XRInputField[] otherInputFields;
        public void Awake()
        {
            GetComponent<Toggle>().onValueChanged.AddListener(ToggleKeyboard);
            otherInputFields = FindObjectsOfType<XRInputField>();
            OnExit.AddListener((e) =>
            {
                UnLink();
            });
        }

        public void OffOthers()
        {
            for (int i = 0; i < otherInputFields.Length; i++)
            {
                if(otherInputFields[i] == this)
                    continue;
                if(otherInputFields[i].GetComponent<Toggle>().isOn)
                {
                    otherInputFields[i].GetComponent<Toggle>().isOn = false;
                }
            }
        }

        public void ToggleKeyboard(bool enabled)
        {
            if (enabled)
            {
                OffOthers();
                Link();
                OnStartEdit.Invoke();
                supportiveText.gameObject.SetActive(false);
                inputText.gameObject.SetActive(true);
            }

            if (!enabled)
            {
                OnExit.Invoke(inputString);
            }
            
            XRKeyboard.instance.gameObject.SetActive(enabled);
            
        }

        private void Link()
        {
            XRKeyboard.instance.OnButtonClick += OnButtonClick;
            XRKeyboard.instance.OnBackSpaceClick += BackspaceText;
            XRKeyboard.instance.OnEnterClick += OnEnterClick;
            Debug.Log("LINK");
        }

        private void UnLink()
        {
            XRKeyboard.instance.OnButtonClick -= OnButtonClick;
            XRKeyboard.instance.OnBackSpaceClick -= BackspaceText;
            XRKeyboard.instance.OnEnterClick -= OnEnterClick;
            Debug.Log("UNLINK");
        }

        public void OnEnterClick()
        {
            ToggleKeyboard(false);
            GetComponent<Toggle>().isOn = false;
            UnLink();
            XRKeyboard.instance.gameObject.SetActive(false);
        }
        public void OnButtonClick(string value)
        {
            inputString += value;
            inputText.text = inputString;
            OnTextChanged.Invoke(inputString);
        }
        public void BackspaceText()
        {
            if (inputString.Length > 0)
            {
                inputString = inputString.Remove(inputString.Length - 1);
                inputText.text = inputString;
                OnTextChanged.Invoke(inputString);
            }
        }

        public UnityEvent OnStartEdit = new UnityEvent();
        public UnityEvent<string>  OnExit = new UnityEvent<string>();
        public UnityEvent<string> OnTextChanged = new UnityEvent<string>();
    }
}
