using System;
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
using WebSocketSharp;

namespace Scripts.Tests
{
    public class GestureRecorder : MonoBehaviour

    {
        enum GestureRecordingCommand
        {
            CHAR,
            GEST,
            CONTINUE,
            LEFT,
            RIGHT,
            TYPE,
            NONE
        }

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

        public string Name
        {
            get => _recordedHandStruct.name;
            set
            {
                if (collectionLabel.text == "character")
                {
                    var words = value.Split('_');
                    if (!int.TryParse(words[^1], out var suff))
                        _recordedHandStruct.name += "_0";
                }

                _recordedHandStruct.name = value;
                gestureLabel.text = _recordedHandStruct.name;
                LockButtons();
            }
        }

        private FrameData _recordedHandStruct = new("");

        private GestureRecordingCommand lastCommand = GestureRecordingCommand.NONE;

        private GestureRecordingCommand MessageToCommand(Message message)
        {
            var text = message.Text;
            if (text == null)
                return GestureRecordingCommand.NONE;
            var tokens = text.Split('@');

            if (Enum.TryParse(typeof(GestureRecordingCommand), tokens[0].Substring(1).ToUpper(), out var v))
            {
                return (GestureRecordingCommand)v;
            }

            return GestureRecordingCommand.NONE;
        }

        private void OnMessageReceived(Message message)
        {
            // /char 
            bool getCommand;
            getCommand = lastCommand == GestureRecordingCommand.NONE;

            if (getCommand)
            {
                lastCommand = MessageToCommand(message);
                switch (lastCommand)
                {
                    case GestureRecordingCommand.CONTINUE:
                        ContinueRecording();
                        TelegramBotProcessor.Instance.SendTextToTelegramFunc("Nessun problema, caro amico!");
                        lastCommand = GestureRecordingCommand.NONE;
                        break;
                    case GestureRecordingCommand.LEFT:
                        leftToggle.isOn = !leftToggle.isOn;
                        TelegramBotProcessor.Instance.SendTextToTelegramFunc("Nessun problema, caro amico!");
                        lastCommand = GestureRecordingCommand.NONE;
                        break;
                    case GestureRecordingCommand.RIGHT:
                        rightToggle.isOn = !rightToggle.isOn;
                        TelegramBotProcessor.Instance.SendTextToTelegramFunc("Nessun problema, caro amico!");
                        lastCommand = GestureRecordingCommand.NONE;
                        break;
                }

                return;
            }


            var text = message.Text;
            if (text.IsNullOrEmpty())
                return;

            switch (lastCommand)
            {
                case GestureRecordingCommand.CHAR:
                    characterNameInput.inputString = text;
                    _curCharacterName = text;
                    TelegramBotProcessor.Instance.SendTextToTelegramFunc("Принято, теперь перса зовут " + text);
                    lastCommand = GestureRecordingCommand.NONE;
                    break;
                case GestureRecordingCommand.GEST:
                    nameInput.inputString = text;
                    Name = text;
                    TelegramBotProcessor.Instance.SendTextToTelegramFunc("Принято, теперь жест называется " + text);
                    lastCommand = GestureRecordingCommand.NONE;
                    break;
                case GestureRecordingCommand.TYPE:
                    collectionLabel.text = text;
                    lastCommand = GestureRecordingCommand.NONE;
                    break;
            }
        }

        private void Start()
        {
            TelegramBotProcessor.Instance.StartReceiving();
            TelegramBotProcessor.onMessageReceived += OnMessageReceived;
            _player.data.onHeadInteraction.AddListener((type) =>
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
            });
            _sequencedHandVisualizer = _player.data.hands.handVisualiser;

            leftToggle.onValueChanged.AddListener(RecordLeft);
            rightToggle.onValueChanged.AddListener(RecordRight);

            nameInput.OnExit.AddListener(RecordName);

            // newGestureButton.onClick.AddListener(NewGestureGroup);
            // continueRecording.onClick.AddListener(ContinueRecording);

            //  Name = Calculations.RandomString(6)+ "_0";
            //   characterNameInput.inputString = Calculations.RandomString(8);
        }


        private void ReloadToggles()
        {
            // мы отправляем аудио в нейронку, которая переводит в текст

            _recordedHandStruct.LeftBones = null;
            _recordedHandStruct.RightBones = null;
            leftToggle.isOn = false;
            rightToggle.isOn = false;
        }

        private void LockButtons()
        {
            bool interactable = _recordedHandStruct.name != "";
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
            try
            {
                await SendToCompiler(_recordedHandStruct);
            }
            catch
            {
                HintWindow.Log(
                    "Oops, it looks like you couldn't save the gesture(  Don't worry, just connect the Internet and try to send it again. All frames remained in place)");
                return;
            }

            ReloadToggles();
            AddIndexToName();
        }

        private async Task SendToCompiler(FrameData handStruct)
        {
            GestureCollections coll; //ch su // sy
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

            await _player.data.gesturesLibrary.RecordFrame(handStruct, coll, characterLabel.text);
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
            _recordedHandStruct.LeftBones =
                isOn ? new BonesData(_player.data.hands.leftHand.points, HandType.left) : null;
            if (isOn)
            {
                _sequencedHandVisualizer.leftHandVisualizer.Spawn(_recordedHandStruct.LeftBones);
//                Debug.Log("Left Ghost Hand Spawned");
            }
            else
            {
                // remove last Left Hand.
                _sequencedHandVisualizer.leftHandVisualizer.Hide();
            }
        }

        public void RecordRight(bool isOn)
        {
            _recordedHandStruct.RightBones =
                isOn ? new BonesData(_player.data.hands.rightHand.points, HandType.right) : null;

            if (isOn)
            {
                _sequencedHandVisualizer.rightHandVisualizer.Spawn(_recordedHandStruct.RightBones);
            }
            else
            {
                _sequencedHandVisualizer.rightHandVisualizer.Hide();
                // remove last Right Hand.
            }
        }
    }
}