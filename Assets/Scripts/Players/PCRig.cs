using System;
using System.Collections;
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
        }
        protected override void Start()
        {
            base.Start();
            
            _handsParent = hands.leftHand.transform.parent;
            ui.gestureInput.image.color = _palette.clear;
            playerStateChangedEvent?.Invoke(playerState = PlayerState.ACTIVE);
            hands.HandEnabled();
        }
        
        //Simulate Gestures
        public void TryGetGestureFrame(string frameName)
        {
            foreach (var DyGr in _library.DynamicGestures)
            {
                if (frameName == DyGr.Name)
                {
                    ui.gestureInput.image.color = _palette.active;
                    // play Dynamic Gesture
                    _targetFrame = DyGr.GetGestureFrame();
                    SimulateDynamicGesture();
                    return;
                }
              
            }
            
            foreach (var gestureFrame in _library.GestureFrames)
            {
                if (frameName == gestureFrame.name)
                {
                    ui.gestureInput.image.color = _palette.enabled;
                    // play Gesture Frame
                    hands.MoveHands(gestureFrame, handsProperties.handSpeed, () =>
                    {
                        ui.gestureInput.image.color = _palette.clear;
                    });
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

            var dynamic = _library.GetDynamicGesture(_targetFrame.baseName);
            
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
                    movement.StopMove();
                    ui.Show();
                    break;
                case PlayerState.ACTIVE:
                    movement.StartMove();
                    ui.Hide();
                    break;
            }
        }
        private void ToggleMenu()
        {
            if ((Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl)) &&
                Input.GetKeyDown(KeyCode.G)) 
            {
                playerState = playerState == PlayerState.MENU ? PlayerState.ACTIVE : PlayerState.MENU;
                playerStateChangedEvent?.Invoke(playerState);
            }
        }
        public void ToggleParentingHands(bool toggle)
        {
            if (hands == null)
                return;
            hands.leftHand.transform.SetParent(toggle ? _handsParent : null);
            hands.rightHand.transform.SetParent(toggle ? _handsParent : null);
        }
    }
}