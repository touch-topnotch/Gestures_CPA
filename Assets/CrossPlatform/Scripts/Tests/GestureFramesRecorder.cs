using System.Linq;
using CrossPlatform.PlayerLogic;
using CrossPlatform.Gestures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CrossPlatform.Tests
{
    public class GestureFramesRecorder : MonoBehaviour

    {
        public RuntimeXRInteractor XRInteractor;
        public Toggle leftToggle;
        public Toggle rightToggle;
        public TMP_InputField nameInput;
        public Button newGestureButton;
        public Button continueRecording;
        public TMP_Text gestureName;
        private Player _player;
        private GFramesCompiler compiler = new GFramesCompiler();
        
        private string current_name = "";
        public string Name
        {
            get => current_name;
            set
            {
                current_name = value; 
                gestureName.text = current_name;
                LockButtons();
            }
        }
        
        private string lastGestureName = "";
        private Vector3[] left;
        private Vector3[] right;
        
        public void Initialize(Player player)
        {
            _player = player;
            compiler.Initialize(XRInteractor);
            leftToggle.onValueChanged.AddListener(RecordLeft);
            rightToggle.onValueChanged.AddListener(RecordRight);
            nameInput.onEndEdit.AddListener(RecordName);
            newGestureButton.onClick.AddListener(NewGestureGroup);
            continueRecording.onClick.AddListener(ContinueRecording);
        }

        private void ReloadToggles()
        {
            left = null;
            right = null;
            leftToggle.isOn = false;
            rightToggle.isOn = false;
        }

        private void LockButtons()
        {
            newGestureButton.interactable = current_name != "";
            continueRecording.interactable = current_name != "";
        }

        public virtual void NewGestureGroup()
        {
            SendToCompiler();
            ReloadToggles();
            Name = "";
        }
        public void ContinueRecording()
        {
            SendToCompiler();
            ReloadToggles();
            AddIndexToName();
        }

        private void SendToCompiler()
        {
            compiler.Record(left, right, current_name);
            if (_player is CrossPlatformPlayer)
            {
                _player.GetComponent<CrossPlatformPlayer>().pointsHandGen.ReplaceLeft(left, Color.blue);
                _player.GetComponent<CrossPlatformPlayer>().pointsHandGen.ReplaceRight(right, Color.red);
            }
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

        public virtual void RecordLeft(bool isOn) =>left = isOn ? _player.GetLeftHandPoints() : null;
        public virtual void RecordRight(bool isOn) => right = isOn ? _player.GetRightHandPoints() : null;

    }
}