using System.Collections;
using CrossPlatform.Gestures;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace CrossPlatform.Scripts.Design
{
    public class CreatedGesturesVisualizer: MonoBehaviour
    {
        public GesturesLibrary Library;

        public ScrollRect scrollRect;

        public SupportHandCreator creator;
        public GameObject togglePrefab;
        
        private readonly WaitForSeconds _waitGesture = new WaitForSeconds(5f);
        public void VisualizeScrollView()
        {
            foreach (var frame in Library.DynamicGestures)
            {
               
                GameObject toggleObject = Instantiate(togglePrefab, scrollRect.content);
                scrollRect.content.SetParent(toggleObject.transform);
                toggleObject.GetComponent<Toggle>().GetComponentInChildren<Text>().text = frame.Name;
                toggleObject.GetComponent<ToggleContainer>().Visualizer = this;
                toggleObject.GetComponent<ToggleContainer>().Gesture = frame;
            }
        }
        private IEnumerator DynamicAnimation(DynamicGesture dGesture, Toggle toggle)
        {
            creator.CreateNewStack(dGesture.Frames[0].LeftPoints);
            creator.AddToStack(dGesture.Frames[0].RightPoints);
            int i = 0;
            while (i < dGesture.Frames.Count)
            {
                creator.MoveHand(dGesture.Frames[i].LeftPoints, 0);
                creator.MoveHand(dGesture.Frames[i].RightPoints, 1);
                i += 1;
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