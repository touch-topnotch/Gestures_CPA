using Scripts.PlayerLogic;
using Scripts.Gestures;
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
        private Player _player;
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

        private HandsStruct _handsPoints = new();
        
        public void Construct (GesturesLibrary library, Player player)
        {
            _player = player;
            _library = library;
            leftToggle.onValueChanged.AddListener(RecordLeft);
            rightToggle.onValueChanged.AddListener(RecordRight);
            nameInput.onEndEdit.AddListener(RecordName);
            newGestureButton.onClick.AddListener(NewGestureGroup);
            continueRecording.onClick.AddListener(ContinueRecording);
        }

        private void ReloadToggles()
        {
            _handsPoints.LeftPoints = null;
            _handsPoints.RightPoints = null;
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
            _player.supHandCreator.CreateNewStack(_handsPoints);
            
            SendToCompiler();
            ReloadToggles();
            Name = "";
        }
        public void ContinueRecording()
        {
            _player.supHandCreator.AddToStack(_handsPoints);
            if (Name.Split('_').Length == 1)
                Name += "_0";
            SendToCompiler();
            ReloadToggles();
            AddIndexToName();
        }
        private void SendToCompiler()
        {
            _library.Record(_handsPoints, _currentName);
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

        public virtual void RecordLeft(bool isOn) =>_handsPoints.LeftPoints = isOn ? _player.playerHands.LeftSkeleton.GetPositions(): null;
        public virtual void RecordRight(bool isOn) => _handsPoints.RightPoints = isOn ? _player.playerHands.RightSkeleton.GetPositions() : null;

    }
}