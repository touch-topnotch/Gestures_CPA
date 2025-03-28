using System;
using System.Collections.Generic;
using Design.RecordingScene;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.KeyboardPack
{
    public class XRInputField: MonoBehaviour
    {
        [SerializeField] private TMP_Text inputText;
        [SerializeField] private TMP_Text supportiveText;

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
            GetComponent<BubbleToggle>().onValueChanged.AddListener(ToggleKeyboard);
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
                if(otherInputFields[i].GetComponent<BubbleToggle>().isOn)
                {
                    otherInputFields[i].GetComponent<BubbleToggle>().isOn = false;
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
                if(supportiveText)
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
            GetComponent<BubbleToggle>().isOn = false;
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
