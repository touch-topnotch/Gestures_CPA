using System;
using System.Collections;
using Gesture_Editor_SDK.Realtime;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.UI;
using UnityEngine;

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
        
        [SerializeField] protected PCUI _ui;
        [SerializeField] protected Palette _palette;
        [SerializeField] protected FirstPersonController _personController;

        private WaitForSeconds _waitUntilNextFrame;
        private Transform _handsParent;
        
        private GestureFrame _targetFrame;
        
        public override void Initialize(PlayerData data)
        {
            base.Initialize(data);
            
            UpdateEvent.Instance?.AddListener(ToggleMenu);
            UpdateEvent.Instance?.AddListener(SimulateHit);
            _waitUntilNextFrame= new WaitForSeconds(handsProperties.delayOnFrame);
            
            
            _handsParent = hands.leftHand.transform.parent;
            _ui.gestureInput.image.color = _palette.clear;
            
            playerStateChangedEvent.AddListener((state) =>
            { 
                //Cursor.visible = state == PlayerState.MENU;
            });
            playerStateChangedEvent?.Invoke(playerState = PlayerState.ACTIVE);
            hands.OnEnabled();
        }

        //Simulate Gestures
        public void TryGetGestureFrame(string frameName)
        {
            if (playerData.library.characterGestures.TryGetValue(frameName, out var dynamicGesture))
            {
                _ui.gestureInput.image.color = _palette.active;
                // play Dynamic Gesture
                _targetFrame = dynamicGesture.frames[0];
                SimulateDynamicGesture();
                return;
            }
    
            if(playerData.library.allAvailableFrames.TryGetValue(frameName, out var frame))
            {
//                    print(gestureFrame.name);
                    _ui.gestureInput.image.color = _palette.enabled;
                    // play Gesture Frame
                    
                    hands.MoveHands(frame, handsProperties.handSpeed,
                        () => { _ui.gestureInput.image.color = _palette.clear; });
                    return;
                
            }
            _ui.gestureInput.image.color = _palette.wrong;
        }
        
        
        public void SimulateDynamicGesture()
        {
            StopCoroutine(WaitUntilNextFrame());
            Debug.Log("Simulating...");
           
            if (_targetFrame == null)
            {
                _ui.gestureInput.image.color = _palette.clear;
                return;
            }

            var dynamic = playerData.library.characterGestures[_targetFrame.baseName];
            
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
                    _ui.Show();
                  //  Cursor.visible = true;
                    break;
                case PlayerState.ACTIVE:
                    StartMove();
                    _ui.Hide();
                  //  Cursor.visible = false;
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

        public void SimulateHit()
        {
            // if (Input.GetKeyDown(KeyCode.H))
            // {
            //     StartCoroutine(HitCoroutine());
            // }
        }

        private IEnumerator HitCoroutine()
        {
            var p = hands.rightHand.points[0];
            Vector3 previousPos = p.localPosition;
            Quaternion previousRot = p.localRotation;
            Vector3 targetPos = new Vector3(0.078f, 1.712f, 0.056f);
            Quaternion targetRot = Quaternion.Euler(new Vector3(290.106018f, 121.231873f, 212.849854f));
            var frameTime = new WaitForFixedUpdate();
            while (Vector3.Distance(targetPos, p.localPosition) > 0.04f)
            {
                p.localPosition = Vector3.Lerp(p.localPosition, targetPos, Time.deltaTime * 1f);
                p.localRotation = Quaternion.Lerp(p.localRotation, targetRot, Time.deltaTime * 1f);
                yield return frameTime;
            }

            targetPos = new Vector3(0.187000006f, 1.63600004f, 0.197999999f);
            targetRot = Quaternion.Euler(new Vector3(27.9578094f,360 - 334.099945f,169.45488f));
           
            while (Vector3.Distance(targetPos, p.localPosition) > 0.0001f)
            {
                p.localPosition = Vector3.Lerp(p.localPosition, targetPos, Time.deltaTime * 5f);
                p.localRotation = Quaternion.Lerp(p.localRotation, targetRot, Time.deltaTime * 6f);
                yield return frameTime;
            }

            targetPos = previousPos;
            targetRot = previousRot;
            
            while (Vector3.Distance(targetPos, p.localPosition) > 0.04f)
            {
                p.localPosition = Vector3.Lerp(p.localPosition, targetPos, Time.deltaTime * 1f);
                p.localRotation = Quaternion.Lerp(p.localRotation, targetRot, Time.deltaTime * 2f);
                yield return frameTime;
            }
            
        }
    }
}