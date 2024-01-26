using System;
using System.Collections;
using Gesture_Editor_SDK.Realtime;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.Hands;
using Scripts.UI;
using UnityEngine;
using Zenject;

namespace Scripts.PlayerLogic
{
    public class PCRig : Rig
    {
        [Serializable] private struct PCHandsProperties
        {
            [Range(0.01f, 10f)] public float delayOnFrame;
            [Range(0.01f, 6f)] public float handSpeed;
        }
        
        [SerializeField]  private PCHandsProperties handsProperties;
        
        [SerializeField] protected PCUI ui;
        [SerializeField] protected Palette _palette;
        [SerializeField] protected FirstPersonController _personController;
        
        protected GesturesLibrary _library;
        
        private WaitForSeconds _waitUntilNextFrame;
        private Transform _handsParent;
        
        private GestureFrame _targetFrame;
        
        
        [Inject]
        private void Construct(UpdateEvent onUpdate, GesturesLibrary gesturesLibrary)
        {
            onUpdate?.AddListener(ToggleMenu);
            _waitUntilNextFrame= new WaitForSeconds(handsProperties.delayOnFrame);
            _library = gesturesLibrary;
            Engine.Instance().stats.hands = hands;
            Engine.Instance().stats.bodyAnchors = anchors;

        }
        protected override void Start()
        {
            base.Start();
            
            _handsParent = hands.leftHand.transform.parent;
            ui.gestureInput.image.color = _palette.clear;
            
            playerStateChangedEvent.AddListener((state) =>
            {
                Cursor.visible = state == PlayerState.MENU;
            });
            playerStateChangedEvent?.Invoke(playerState = PlayerState.ACTIVE);
            hands.OnEnabled();
        }
        
        //Simulate Gestures
        public void TryGetGestureFrame(string frameName)
        {
            if(_library.DynamicGestures.TryGetValue(frameName, out var dynamicGesture))
            {
                ui.gestureInput.image.color = _palette.active;
                // play Dynamic Gesture
                _targetFrame =  dynamicGesture.GetGestureFrame();
                SimulateDynamicGesture();
                return;
            }
            
            print("TryGetGestureFrame: " + GestureMapper.PrefixOfName(frameName));
            if(_library.DynamicGestures.TryGetValue(GestureMapper.PrefixOfName(frameName), out dynamicGesture))
            {
                if (dynamicGesture.TryGetGestureFrame(frameName, out var gestureFrame))
                {
                    print(gestureFrame.name);
                    ui.gestureInput.image.color = _palette.enabled;
                    // play Gesture Frame
                    hands.MoveHands(gestureFrame, handsProperties.handSpeed,
                        () => { ui.gestureInput.image.color = _palette.clear; });
                    return;
                }
            }
            ui.gestureInput.image.color = _palette.wrong;
        }
        
        public void SimulateDynamicGesture()
        {
            StopCoroutine(WaitUntilNextFrame());
            if (_targetFrame == null)
            {
                ui.gestureInput.image.color = _palette.clear;
                return;
            }

            var dynamic = _library.DynamicGestures[_targetFrame.baseName];
            
            hands.MoveHands(_targetFrame, handsProperties.handSpeed, ()=>{StartCoroutine(WaitUntilNextFrame());});
            
            _targetFrame = dynamic.GetNextFrameOf(_targetFrame);
        }
        private IEnumerator WaitUntilNextFrame()
        {
            yield return _waitUntilNextFrame;
            SimulateDynamicGesture();
        }
        
        // Player State
        protected override void OnPlayerStateChaned(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.MENU:
                    StopMove();
                    ui.Show();
                    break;
                case PlayerState.ACTIVE:
                    StartMove();
                    ui.Hide();
                    break;
            }
        }

        
        public override bool isMoved() => _personController.enabled;

        public override void StartMove()
        {
            _personController.playerCanMove = true;
            _personController.cameraCanMove = true;
        }

        public override void StopMove()
        {
            _personController.playerCanMove = false;
            _personController.cameraCanMove = false;
        }

        private void ToggleMenu()
        {
            if ((Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl)) &&
                Input.GetKeyDown(KeyCode.G)) 
            {
                playerState = playerState == PlayerState.MENU ? PlayerState.ACTIVE : PlayerState.MENU;
                playerStateChangedEvent?.Invoke(playerState);
            }

            if (playerState == PlayerState.MENU)
                _personController.cameraCanMove = Input.GetKey(KeyCode.LeftShift);
        }
        public void ToggleParentingHands(bool toggle)
        {
            if (hands == null)
                return;
            hands.transform.SetParent(toggle ? _handsParent : null);
        }
    }
}