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
        private Rig _rig;
        private SupportHandVisualiser _supportHdCreator;
        private GesturesLibrary _library;
        
        private string _currentName = "";
        public string Name
        {
            get => _currentName;
            set
            {
                _currentName = value; 
                gestureName.text = _currentName;
                LockButtons();
            }
        }

        private HandsStruct _recordedHandStruct = new();
        [Inject]
        private void Construct (GesturesLibrary library, Rig rig)
        {
            _library = library;
            _rig = rig;
            _supportHdCreator = _rig.Hands.handVisualiser;
            
            leftToggle.onValueChanged.AddListener(RecordLeft);
            rightToggle.onValueChanged.AddListener(RecordRight);
            nameInput.onEndEdit.AddListener(RecordName);
            newGestureButton.onClick.AddListener(NewGestureGroup);
            continueRecording.onClick.AddListener(ContinueRecording);

            Name = Calculations.RandomString(6);

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
            bool interactable = _currentName != "";
            leftToggle.interactable = interactable;
            rightToggle.interactable = interactable;
            newGestureButton.interactable = interactable;
            continueRecording.interactable = interactable;
        }

        public virtual void NewGestureGroup()
        { 
            _supportHdCreator.CreateNewStack(_recordedHandStruct);
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
           
            _library.RecordFrame(_recordedHandStruct, _currentName);
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
            _recordedHandStruct.LeftBones = isOn ? new BonesData(_rig.Hands.leftHand.points, HandType.left) : null;
            if (isOn)
            {
                _supportHdCreator.AddToStack(_recordedHandStruct.LeftBones);
                Debug.Log("Left Ghost Hand Spawned");
            }
            else
            {
                // remove last Left Hand.
            }
        }

        public virtual void RecordRight(bool isOn)
        {
            _recordedHandStruct.RightBones = isOn ? new BonesData(_rig.Hands.rightHand.points, HandType.right) : null;
            if (isOn)
            {
                _supportHdCreator.AddToStack(_recordedHandStruct.RightBones);
            }
            else
            {
                // remove last Right Hand.
            }
        }
    }
}