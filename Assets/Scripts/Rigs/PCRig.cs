using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.Static.Definitions;
using Scripts.Static.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    public class PCRig : Rig
    {
        [SerializeField][BoxGroup("Settings")]
        [Range(0.01f, 10f)] public float delayOnFrame;
        [SerializeField][BoxGroup("Components")]
        protected FirstPersonController _personController;
        [SerializeField] protected RingMenu gestureMenu;
        private WaitForSeconds _waitUntilNextFrame;
        private WaitForUpdate _waitForUpdate;
        private GesturesLibrary _library;
        private bool isInitialized;
        private FrameData _targetFrameData;
        public override void Initialize()
        {
            base.Initialize();

            if (hands)
                hands.OnEnabled();
            
            _waitUntilNextFrame = new WaitForSeconds(delayOnFrame);
            _library = inherited.data.gesturesLibrary;

            PrepareRingData();
            inherited.data.onPlayerModeChanged.AddListener(ps => { gestureMenu.isActive = ps == PlayerMode.MENU; });
            
            inherited.playerMode = PlayerMode.ACTIVE;
        }

        private void PrepareRingData()
        {
            var rings = new List<Ring>()
            {
                new Ring("Types", new List<RingProps>()
                {
                    new RingProps("Characters", gestureMenu.OpenRing),
                    new RingProps("Supportive", gestureMenu.OpenRing),
                    new RingProps("System", gestureMenu.OpenRing)
                }),
                new Ring("Characters", RingProps.GetFromDictionary(_library.characterGestures, gestureMenu.OpenRing)),
                new Ring("Supportive", RingProps.GetFromDictionary(_library.supportiveGestures, SimulateFrameAnClose)),
                new Ring("System", RingProps.GetFromDictionary(_library.systemGestures, SimulateFrameAnClose))
            };
            foreach (var dg in _library.characterGestures.Keys)
            {
                // create new ring (dynamic gesture, simple gestures.)
                var sectors = new List<RingProps> { new(dg, SimulateGestureAnClose) };

                foreach (var f in _library.characterGestures[dg].frames)
                {
                    sectors.Add(new RingProps(f.name, SimulateFrameAnClose));
                }

                rings.Add(new Ring(dg, sectors));
            }

            gestureMenu.SetRings(rings, "Types");
        }

        private void SimulateFrameAnClose(string key)
        {
            SimulateFrame(key);
            gestureMenu.OpenRing("Types");
            inherited.playerMode = PlayerMode.ACTIVE;
        }

        private void SimulateGestureAnClose(string key)
        {
            SimulateDynamicGesture(key);
            gestureMenu.OpenRing("Types");
            inherited.playerMode = PlayerMode.ACTIVE;
        }

        private void SimulateFrame(string key)
        {
            if (!_library.allAvailableFrames.ContainsKey(key))
            {
                Debug.Log("Key: " + key + " don't found");
                return;
            }
            _targetFrameData = _library.allAvailableFrames[key].ParentedFrame(inherited.data.anchors.Body);
        }

        private void SimulateDynamicGesture(string key)
        {
            var dynamicName = GestureMapper.PrefixOfName(key);
            var indexOfName = dynamicName == key ? 0 : GestureMapper.IndexOfName(key);
            var frameName = dynamicName + '_' + indexOfName;
            var nextFrame = dynamicName + '_' + (indexOfName + 1);
            if (indexOfName >= _library.characterGestures[dynamicName].frames.Count)
                return;
            if (!_library.allAvailableFrames.ContainsKey(frameName))
            {
                Debug.Log("Key: " + frameName + " don't found");
                return;
            }
            _targetFrameData = _library.allAvailableFrames[frameName].ParentedFrame(inherited.data.anchors.Body);
            StartCoroutine(WaitUntilNextFrame(nextFrame));
        }

        private IEnumerator WaitUntilNextFrame(string next)
        {
            while(!hands.leftHand.inSameLocation(_targetFrameData?.LeftBones) || !hands.rightHand.inSameLocation(_targetFrameData?.RightBones))
                yield return _waitForUpdate;
            yield return _waitUntilNextFrame;
            SimulateDynamicGesture(next);
        }

        // Player State
        protected override void OnPlayerStateChanged(PlayerMode state)
        {
            switch (state)
            {
                case PlayerMode.MENU:
                    StopMove();
                    //  Cursor.visible = true;
                    break;
                case PlayerMode.ACTIVE:
                    StartMove();
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

        protected override void Centrize()
        {
        }

        private void Update()
        {
            ToggleMenu();
            
            if(_targetFrameData != null){
                UpdateHand(hands.leftHand, _targetFrameData.LeftBones);
                UpdateHand(hands.rightHand, _targetFrameData.RightBones);
            }

        }

        private void UpdateHand(HandMesh hand, BonesData target)
        {
            if(target == null)
                return;
            
            if (!InputExtension.CtrlOrCmd() ) 
                hand.UpdateJoint(0, target.rootPos);
            hand.UpdateJoints(target.rotations);
        }

        private void ToggleMenu()
        {
            if (InputExtension.GetKeyWithCtrlOrCmd(KeyCode.G))
            {
                inherited.playerMode = inherited.playerMode == PlayerMode.MENU
                    ? PlayerMode.ACTIVE
                    : PlayerMode.MENU;
            }

            if (inherited.playerMode == PlayerMode.MENU)
                _personController.cameraCanMove = Input.GetKey(KeyCode.LeftShift);
        }

        public override void OnDisable()
        {
            gestureMenu.isActive = false;
            base.OnDisable();
        }
    }
}