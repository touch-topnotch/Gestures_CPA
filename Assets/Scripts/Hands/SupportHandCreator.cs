using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using Scripts.Static;
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

        private UpdateEvent _onUpdate;

        [Inject]
        private void Construct(UpdateEvent onUpdate)
        {
            _onUpdate = onUpdate;
        }

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
            l.rl("override prev");
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
                AddToStack(points);
                return;
            }
            ActiveHands[index].ChangePosition(points, parent);
        }

        public void OverrideHands(HandsStruct hands)
        {
            if (ActiveHands.Count >= 2)
            {
                OverrideHand(hands.LeftPoints, 0);
                OverrideHand(hands.RightPoints, 1);
            }
            else
            {
                if (ActiveHands.Count == 1)
                {
                    OverrideHand(hands.LeftPoints, 0);
                    AddToStack(hands.RightPoints);
                    return;
                }
                AddToStack(hands);
            }
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
            l.rl("Spawn new");
            var newHand = GameObject.Instantiate(SupHandPrefab, parent);
            newHand.gameObject.name = newHand.gameObject.name.Replace("(Clone)", $"_{ActiveHands.Count}");
            SupportHandVisualizer hand = newHand.GetComponent<SupportHandVisualizer>();
            hand.Initialize(ref _onUpdate);
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