using Scripts.PlayerLogic;
using Scripts.Gestures;
using Scripts.Hands;
using Scripts.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Scripts.Tests
{
    public class GestureFramesRecorder : MonoBehaviour

    {
        public Toggle leftToggle;
        public Toggle rightToggle;
        public TMP_InputField nameInput;
        public Button newGestureButton;
        public Button continueRecording;
        public TMP_Text gestureName;
        public PlayerRig rig;
        public SupportHandCreator SupportHdCreator;
        private GesturesLibrary _library;
        
        private string _currentName = "";
        public string Name
        {
            get => _currentName;
            set
            {
                if (value.Split('_').Length == 1)
                    _currentName = value + "_0";
                else
                    _currentName = value; 
                gestureName.text = _currentName;
                LockButtons();
            }
        }

        private HandsStruct _recordedHandStruct= new();
        [Inject]
        private void Construct (GesturesLibrary library)
        {
            _library = library;
            leftToggle.onValueChanged.AddListener(RecordLeft);
            rightToggle.onValueChanged.AddListener(RecordRight);
            nameInput.onEndEdit.AddListener(RecordName);
            newGestureButton.onClick.AddListener(NewGestureGroup);
            continueRecording.onClick.AddListener(ContinueRecording);
        }

        private void ReloadToggles()
        {
            _recordedHandStruct.LeftBones = null;
            _recordedHandStruct.RightBones = null;
            leftToggle.isOn = false;
            rightToggle.isOn = false;
        }

        private void LockButtons()
        {
            newGestureButton.interactable = _currentName != "";
            continueRecording.interactable = _currentName != "";
        }

        public virtual void NewGestureGroup()
        { 
            SupportHdCreator.CreateNewStack(_recordedHandStruct);
            SendToCompiler();
            ReloadToggles();
            Name = "";
        }
        public void ContinueRecording()
        {
            if (Name.Split('_').Length == 1)
                Name += "_0";
            SendToCompiler();
            ReloadToggles();
            AddIndexToName();
        }
        private void SendToCompiler()
        {
            l.rl(_recordedHandStruct.LeftBones.rootPos.ToString());
            _library.Record(_recordedHandStruct, _currentName);
        }
        
        public virtual void RecordName(string name)
        {
            Name = name;
        }

        public virtual void AddIndexToName()
        {
            // split name by _
            // add index to last word
            if (Name.Split('_').Length == 1)
            {
               Name += "_1";
               return;
            }
            
            var words = Name.Split('_');
            words[^1] = (int.Parse(words[^1]) + 1).ToString();
            // join words
            Name = string.Join("_", words);
        }

        public virtual void RecordLeft(bool isOn)
        {
            _recordedHandStruct.LeftBones = isOn ? new BonesData(rig.hands.leftHand.points, HandType.left) : null;
            if (isOn)
            {
                SupportHdCreator.AddToStack(_recordedHandStruct);
            }
            else
            {
                // remove last Left Hand.
            }
        }

        public virtual void RecordRight(bool isOn)
        {
            _recordedHandStruct.RightBones = isOn ? new BonesData(rig.hands.rightHand.points, HandType.right) : null;
            if (isOn)
            {
                SupportHdCreator.AddToStack(_recordedHandStruct);
            }
            else
            {
                // remove last Right Hand.
            }
        }
    }
}