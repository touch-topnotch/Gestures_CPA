using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts.PlayerLogic;
using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.Network;
using Scripts.Static;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TMPro;
using UI.KeyboardPack;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Scripts.Tests
{
    public class GestureFramesRecorder : MonoBehaviour

    {
        public Toggle leftToggle;
        public Toggle rightToggle;
        public XRInputField nameInput;
        public XRInputField characterNameInput;
        public Button newGestureButton;
        public Button continueRecording;
        public Text gestureLabelText;
        public Text characterLabelText;
        public Player _player;
        private SupportHandVisualiser _supportHdCreator;
       // private GesturesLibrary _library;

        private string _curCharacterName = "";
        private string _currentName = "";
        public string Name
        {
            get => _currentName;
            set
            {
                _currentName = value; 
                var words = value.Split('_');
                if (!int.TryParse(words[^1], out var suff))
                    Name += "_0";
                gestureLabelText.text = _currentName;
                LockButtons();
            }
        }

        private HandsStruct _recordedHandStruct = new();

        private bool taskCompleted = false;
        private void OnMessageReceived(Message message)
        {
            var text = message.Text;
            if (text == null)
                return;
            
            if(text.Contains("Char"))
            {
                string characterName =
                    text.Split(' ').Length > 0 ? text.Split(' ')[1] : Calculations.RandomString(6);
                characterNameInput.inputString = characterName;
                _curCharacterName = characterName;
                TelegramBotProcessor.SendTextToTelegram("Принято, теперь перса зовут " + characterName);
            }

            if (text.Contains("Gest"))
            {
                string gestureName = text.Split(' ').Length > 0 ? text.Split(' ')[1] : Calculations.RandomString(8);
                nameInput.inputString = gestureName;
                Name = gestureName;
                TelegramBotProcessor.SendTextToTelegram("Принято, теперь жест называется " + gestureName);
            }
        }

        void Awake()
        {
            TelegramBotProcessor.StartReceiving();
            TelegramBotProcessor.onMessageReceived += OnMessageReceived;
        }

    

        private void Start ()
        {
            //_rig = rig;
            _supportHdCreator = _player.data.hands.handVisualiser;
            
            leftToggle.onValueChanged.AddListener(RecordLeft);
            rightToggle.onValueChanged.AddListener(RecordRight);
            
            nameInput.OnExit.AddListener(RecordName);
    //        characterNameInput.OnExit.AddListener((e) => { characterLabelText.text = e;});
            
            newGestureButton.onClick.AddListener(NewGestureGroup);
            continueRecording.onClick.AddListener(ContinueRecording);
                
            Name = Calculations.RandomString(6)+ "_0";
            characterNameInput.inputString = Calculations.RandomString(8);
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
            SendToCompiler();
            ReloadToggles();
            AddIndexToName();
        }
        private void SendToCompiler()
        {
            _player.gestureCombiner.library.RecordFrame(_recordedHandStruct, _currentName, characterLabelText.text);
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
            _recordedHandStruct.LeftBones = isOn ? new BonesData(_player.data.hands.leftHand.points, HandType.left) : null;
            if (isOn)
            {
                _supportHdCreator.AddToStack(_recordedHandStruct.LeftBones);
//                Debug.Log("Left Ghost Hand Spawned");
            }
            else
            {
                // remove last Left Hand.
            }
        }

        public virtual void RecordRight(bool isOn)
        {
            _recordedHandStruct.RightBones = isOn ? new BonesData(_player.data.hands.rightHand.points, HandType.right) : null;
      
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