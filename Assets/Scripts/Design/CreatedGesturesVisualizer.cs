using System.Collections;
using Scripts.Gestures;
using Scripts.HandsLogic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Scripts.Design
{
    public class CreatedGesturesVisualizer: MonoBehaviour
    {
      

        public ScrollRect scrollRect;
        public SupportHandVisualiser creator;
        public GameObject togglePrefab;
        public float waitTime;

        private GesturesLibrary _library;
        
        private WaitForSeconds _waitGesture;
        [Inject]
        private void Construct(GesturesLibrary library)
        {
            _library = library;
            _waitGesture = new WaitForSeconds(waitTime);
        }

        public void VisualizeScrollView()
        {
            foreach (var frame in _library.characterGestures)
            {
                GameObject toggleObject = Instantiate(togglePrefab, scrollRect.content);
                scrollRect.content.SetParent(toggleObject.transform);
                toggleObject.GetComponent<Toggle>().GetComponentInChildren<Text>().text = frame.Key;
                toggleObject.GetComponent<ToggleContainer>().Visualizer = this;
                toggleObject.GetComponent<ToggleContainer>().Gesture = frame.Value;
            }
        }
        private IEnumerator DynamicAnimation(DynamicGesture dGesture, Toggle toggle)
        {
            creator.CreateNewStack(dGesture.frames[0].Hands.LeftBones);
            creator.AddToStack(dGesture.frames[0].Hands.RightBones);
            int i = 0;
            while (i < dGesture.frames.Count)
            {
                creator.MoveHand(dGesture.frames[i].Hands.LeftBones, 0);
                creator.MoveHand(dGesture.frames[i].Hands.RightBones, 1);
                i += 1;
                print("try to move");
                yield return _waitGesture;
            }
            creator.HideHands();
            toggle.isOn = false;
        }
        
        public void ShowDynamicGesture(DynamicGesture dGesture, Toggle toggle)
        {
            StartCoroutine(DynamicAnimation(dGesture, toggle));
        }
    }
}