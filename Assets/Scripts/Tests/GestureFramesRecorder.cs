using System.Threading.Tasks;
using Design.RecordingScene;
using Scripts.PlayerLogic;
using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.Network;
using Scripts.Static;
using Scripts.Systems;
using Telegram.Bot.Types;
using TMPro;
using UI.KeyboardPack;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Tests
{
    public class GestureFramesRecorder : MonoBehaviour

    {
        
        public BubbleToggle leftToggle;
        public BubbleToggle rightToggle;
        public XRInputField nameInput;
        public XRInputField characterNameInput;
        public BubbleButton newGestureButton;
        public BubbleButton continueRecording;
        public TMP_Text gestureLabel;
        public TMP_Text characterLabel;
        public TMP_Text collectionLabel;
        public Player _player;

        private SequencedHandVisualizer _sequencedHandVisualizer;
        
        private string _curCharacterName = "";
        private string _currentName = "";
        public string Name
        {
            get => _currentName;
            set
            {
                _currentName = value;

                if (collectionLabel.text == "character")
                {
                    var words = value.Split('_');
                    if (!int.TryParse(words[^1], out var suff))
                        _currentName += "_0";
                }
                
                gestureLabel.text = _currentName;
                LockButtons();
            }
        }

        private HandsStruct _recordedHandStruct = new();
        private void OnMessageReceived(Message message)
        {
            var text = message.Text;
            if (text == null)
                return;
            var tokens = text.Split(' ');
            int i = 0;
            while(i < tokens.Length)
            {
                switch (tokens[i])
                {
                    case"/char":
                        string characterName = i + 1 < tokens.Length ? tokens[i+1] : Calculations.RandomString(6);
                        characterNameInput.inputString = characterName;
                        _curCharacterName = characterName;
                        TelegramBotProcessor.Instance.SendTextToTelegramFunc("Принято, теперь перса зовут " + characterName);
                        i += 2;
                        break;
                    case "/gest":
                        string gestureName= i + 1 < tokens.Length ? tokens[i+1] : Calculations.RandomString(6);
                        nameInput.inputString = gestureName;
                        Name = gestureName;
                        TelegramBotProcessor.Instance.SendTextToTelegramFunc("Принято, теперь жест называется " + gestureName);
                        i += 2;
                        break;
                    case "/continue":
                        ContinueRecording();
                        TelegramBotProcessor.Instance.SendTextToTelegramFunc("Nessun problema, caro amico!");
                        i++;
                        break;
                    case "/left":
                        leftToggle.isOn = !leftToggle.isOn;
                        TelegramBotProcessor.Instance.SendTextToTelegramFunc("Nessun problema, caro amico!");
                        i++;
                        break;
                    case "/right":
                        rightToggle.isOn = !rightToggle.isOn;
                        TelegramBotProcessor.Instance.SendTextToTelegramFunc("Nessun problema, caro amico!");
                        i++;
                        break;
                    case "/type":
                        collectionLabel.text = ++i < tokens.Length ? tokens[i++] : collectionLabel.text;
                        break;
                    default:
                        i++;
                        break;
                }
            }
        }
        
        private void Start ()
        {
            TelegramBotProcessor.Instance.StartReceiving();
            TelegramBotProcessor.onMessageReceived += OnMessageReceived;
            _player.curRig.headInteraction.onHeadInteraction += (type) =>
            {
                switch (type)
                {
                    case HeadInteractionType.Left:
                        leftToggle.isOn = true;
                        break;
                    case HeadInteractionType.Right:
                        rightToggle.isOn = true;
                        break;
                    case HeadInteractionType.Shaking:
                        leftToggle.isOn = true;
                        rightToggle.isOn = true;
                        break;
                    case HeadInteractionType.DoubleNod:
                        ContinueRecording();
                        break;
                }
            };
            _sequencedHandVisualizer = _player.data.hands.handVisualiser;
            
            leftToggle.onValueChanged.AddListener(RecordLeft);
            rightToggle.onValueChanged.AddListener(RecordRight);
            
            nameInput.OnExit.AddListener(RecordName);
            
            newGestureButton.onClick.AddListener(NewGestureGroup);
            continueRecording.onClick.AddListener(ContinueRecording);
                
          //  Name = Calculations.RandomString(6)+ "_0";
          //   characterNameInput.inputString = Calculations.RandomString(8);
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

        public async void NewGestureGroup()
        {
            _sequencedHandVisualizer.Spawn(_recordedHandStruct);
            await SendToCompiler(_recordedHandStruct);
            ReloadToggles();
            Name = "";
        }
        public async void ContinueRecording()
        {
            await SendToCompiler(_recordedHandStruct);
            ReloadToggles();
            AddIndexToName();
        }
        private async Task SendToCompiler(HandsStruct handStruct)
        {
            GestureCollections coll;//ch su // sy
            if (collectionLabel.text[1] == 'h')
            {
                coll = GestureCollections.characters;
            }
            else if (collectionLabel.text[1] == 'u')
            {
                coll = GestureCollections.supportive;
            }
            else
            {
                coll = GestureCollections.system;
            }
            await _player.gestureCombiner.library.RecordFrame(handStruct, _currentName, coll,  characterLabel.text);
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

        public void RecordLeft(bool isOn)
        {
            _recordedHandStruct.LeftBones = isOn ? new BonesData(_player.data.hands.leftHand.points, HandType.left) : null;
            if (isOn)
            {
                _sequencedHandVisualizer.leftHandVisualizer.Spawn(_recordedHandStruct.LeftBones);
//                Debug.Log("Left Ghost Hand Spawned");
            }
            else
            {
                // remove last Left Hand.
            }
        }

        public void RecordRight(bool isOn)
        {           
            _recordedHandStruct.RightBones = isOn ? new BonesData(_player.data.hands.rightHand.points, HandType.right) : null;
         
            if (isOn)
            {
                _sequencedHandVisualizer.rightHandVisualizer.Spawn(_recordedHandStruct.RightBones);
            }
            else
            {
                // remove last Right Hand.
            }
        }
    }
}