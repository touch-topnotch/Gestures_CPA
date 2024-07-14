using System.Collections.Generic;
using CrossPlatform.PlayerLogic;
using UnityEngine;

namespace CrossPlatform.Gestures
{
    public class SupportHandCreator: MonoBehaviour
    {
        [SerializeField]
        private GameObject SupHandPrefab;
        
        private int _hiddenHands = 0;
       
        private List<SupportHandVisualizer> _activeHHands = new List<SupportHandVisualizer>();
        private GameObject _parent;

        public void CreateNewStack(Vector3[] points)
        {
            HideHands();
            AddToStack(points);
        }
        
        public void AddToStack(Vector3[] points)
        {
            if (_hiddenHands == 0)
            {
                SpawnNew(points);
                return;
            }
            _activeHHands[^_hiddenHands].ChangePosition(points);
            _activeHHands[^_hiddenHands].Show();
            _hiddenHands--;
            
        }

        public void OverrideHand(Vector3[] points, int index)
        {
            if (index >= _activeHHands.Count - _hiddenHands)
            {
                Debug.LogAssertion("Index out of range");
                return;
            }
            _activeHHands[index].ChangePosition(points);
        }

        public void MoveHand(Vector3[] points, int index)
        {
            if (index >= _activeHHands.Count - _hiddenHands)
            {
                Debug.LogAssertion("Index out of range");
                return;
            }
            _activeHHands[index].ChangePositionSmooth(points);
        }

        public void HideHands()
        {
            if(_activeHHands.Count == 0)
                return;
            
            foreach (SupportHandVisualizer hand in _activeHHands)
            {
                hand.Hide();
            }
            _hiddenHands = _activeHHands.Count;
        }

        private void SpawnNew(Vector3[] points)
        {
            var newHand = GameObject.Instantiate(SupHandPrefab);
            newHand.gameObject.name = newHand.gameObject.name.Replace("(Clone)", $"_{_activeHHands.Count}");
            SupportHandVisualizer hand = newHand.GetComponent<SupportHandVisualizer>();
            hand.Initialize();
            hand.ChangePosition(points);
            hand.Show();
            
            _activeHHands.Add(hand);
        }
        private void DebugPoints(Vector3[] points, int index)
        {
            var toDebug = index.ToString() + " hand: ";
            foreach (var VARIABLE in points)
            {
                toDebug += VARIABLE.ToString() + ", ";
            }
            Debug.Log(toDebug);
        }
    }
}