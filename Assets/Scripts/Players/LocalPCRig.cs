using System.Collections;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.Hands;
using Scripts.UI;
using UnityEngine;
using Zenject;

namespace Scripts.PlayerLogic
{
    public class LocalPCRig : PlayerRig
    {
        [SerializeField] [Range(0.01f, 10f)] private float _delayBetweenFrames;
        [SerializeField] protected PCUI ui;
        protected GesturesLibrary _library;
        private Transform _handsParent;

        [Inject]
        private void Construct(UpdateEvent onUpdate, GesturesLibrary gesturesLibrary, GestureCombiner gestureCombiner)
        {
            onUpdate?.AddListener(ToggleMenu);
            _library = gesturesLibrary;
            gestureCombiner.AddRecognitionButton("StartRecognizion Button");
        }
        protected override void Start()
        {
            base.Start();
            
            _handsParent = hands.leftHand.transform.parent;
            ui.GetGestureInput().image.color = Color.white;
            playerStateChangedEvent?.Invoke(_state = PlayerState.ACTIVE);
            hands.HandEnabled();
        }
        
        //Simulate Gestures
        public void TryGetGestureFrame(string frameName)
        {
            #if(UNITYEDITOR)
                return;
            #endif
            foreach (var DyGr in _library.DynamicGestures)
            {

                if (frameName == DyGr.Name)
                {
                    ui.GetGestureInput().image.color = Color.green;
                    // play Dynamic Gesture
                    StopCoroutine(SimulateDynamicGesture(DyGr));
                    StartCoroutine(SimulateDynamicGesture(DyGr));
                    return;
                }
              
            }

            foreach (var GestureFrame in _library.GestureFrames)
            {
                if (frameName == GestureFrame.name)
                {
                    ui.GetGestureInput().image.color = Color.yellow;
                    // play Gesture Frame
                    SimulateGestures(GestureFrame);
                    return;
                }
            }

            ui.GetGestureInput().image.color = Color.red;
        }
        
        public IEnumerator SimulateDynamicGesture(DynamicGesture gestures)
        {
            var wait = new WaitForSeconds(_delayBetweenFrames);
            foreach (var frame in gestures.Frames)
            {
                //if HandMesh is PCHandMesh
                (hands.leftHand as PCHandMesh)?.SetBonesSmooth(frame.Hands.LeftBones);
                (hands.rightHand as PCHandMesh)?.SetBonesSmooth(frame.Hands.RightBones);
                
                yield return wait;
            }

            ui.GetGestureInput().image.color = Color.white;
        }
        public void SimulateGestures(GestureFrame frame)
        {
            (hands.leftHand as PCHandMesh)?.SetBonesSmooth(frame.Hands.LeftBones);
            (hands.rightHand as PCHandMesh)?.SetBonesSmooth(frame.Hands.RightBones);
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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _state = _state == PlayerState.MENU ? PlayerState.ACTIVE : PlayerState.MENU;
                playerStateChangedEvent?.Invoke(_state);
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