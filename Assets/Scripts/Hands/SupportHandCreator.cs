using System;
using System.Collections.Generic;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using UnityEngine;
using Zenject;

namespace Scripts.Hands
{
    public class SupportHandCreator: MonoBehaviour
    {
        [SerializeField]
        private GameObject SupHandPrefab;
        
        public Transform parent;
        private int _hiddenHands = 0;

        [HideInInspector]
        public List<SupportHandVisualizer> ActiveHands = new List<SupportHandVisualizer>();

        

        public void CreateNewStack(Vector3[] points)
        {
            HideHands();
            AddToStack(points);
        }

        public void CreateNewStack(HandsStruct hands)
        {
            CreateNewStack(hands.LeftPoints);
            AddToStack(hands.RightPoints);
            RefreshLinesPosition();
        }
        
        
        public void AddToStack(Vector3[] points)
        {
            if (_hiddenHands == 0)
            {
                SpawnNew(points);
                return;
            }
            ActiveHands[^_hiddenHands].ChangePosition(points, parent);
            ActiveHands[^_hiddenHands].Show();
            _hiddenHands--;
            
        }

        public void AddToStack(HandsStruct hands)
        {
            AddToStack(hands.LeftPoints);
            AddToStack(hands.RightPoints);
        }

        public void OverrideHand(Vector3[] points, int index)
        {
            if (index >= ActiveHands.Count - _hiddenHands)
            {
                Debug.LogAssertion("Index out of range");
                return;
            }
            ActiveHands[index].ChangePosition(points);
        }

        public void MoveHand(Vector3[] points, int index)
        {
            if (index >= ActiveHands.Count - _hiddenHands)
            {
                Debug.LogAssertion("Index out of range");
                return;
            }
            ActiveHands[index].ChangePositionSmooth(points);
        }

        public void RefreshLinesPosition()
        {
            foreach (var hand in ActiveHands)
            {
                hand.RefreshLines();
            }
        }
        public void HideHands()
        {
            if(ActiveHands.Count == 0)
                return;
            
            foreach (SupportHandVisualizer hand in ActiveHands)
            {
                hand.Hide();
            }
            _hiddenHands = ActiveHands.Count;
        }

        private void SpawnNew(Vector3[] points)
        {
            var newHand = GameObject.Instantiate(SupHandPrefab, parent);
            newHand.gameObject.name = newHand.gameObject.name.Replace("(Clone)", $"_{ActiveHands.Count}");
            SupportHandVisualizer hand = newHand.GetComponent<SupportHandVisualizer>();
            hand.Initialize();
            hand.ChangePosition(points, parent);
            hand.Show();
            
            ActiveHands.Add(hand);
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