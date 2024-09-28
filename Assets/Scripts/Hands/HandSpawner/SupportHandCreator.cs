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
    public enum VisualizationType {Mesh, Bones}
    public class SupportHandCreator: MonoBehaviour
    {
        public VisualizationType type;
        public GameObject LeftHand;
        public GameObject RightHand;
        public GameObject Bones;

        public Transform parent;
        private int _hiddenHands = 0;

        [HideInInspector]
        public List<IHandVisualiser> ActiveHands = new List<IHandVisualiser>();

        private UpdateEvent _onUpdate;

        [Inject]
        private void Construct(UpdateEvent onUpdate)
        {
            l.rl("CONSTRUCTED");
            _onUpdate = onUpdate;
        }

        public void CreateNewStack(BonesData data)
        {
            HideHands();
            AddToStack(data);
        }

        public void CreateNewStack(HandsStruct hands)
        {
            CreateNewStack(hands.LeftBones);
            AddToStack(hands.RightBones);
           // RefreshLinesPosition();
        }
        
        
        public void AddToStack(BonesData data)
        {
            if (_hiddenHands == 0)
            {
                SpawnNew(data);
                return;
            }
            ActiveHands[^_hiddenHands].ChangePosition(data, parent);
            ActiveHands[^_hiddenHands].Show();
            _hiddenHands--;
            l.rl("override prev");
        }

        public void AddToStack(HandsStruct hands)
        {
            AddToStack(hands.LeftBones);
            AddToStack(hands.RightBones);
        }

        public void OverrideHand(BonesData data, int index)
        {
            if (index >= ActiveHands.Count - _hiddenHands)
            {
                AddToStack(data);
                return;
            }
            ActiveHands[index].ChangePosition(data, parent);
        }

        public void OverrideHands(HandsStruct hands)
        {
            if (ActiveHands.Count >= 2)
            {
                OverrideHand(hands.LeftBones, 0);
                OverrideHand(hands.RightBones, 1);
            }
            else
            {
                if (ActiveHands.Count == 1)
                {
                    OverrideHand(hands.LeftBones, 0);
                    AddToStack(hands.RightBones);
                    return;
                }
                AddToStack(hands);
            }
        }

        public void MoveHand(BonesData data, int index)
        {
            if (index >= ActiveHands.Count - _hiddenHands)
            {
                Debug.LogAssertion("Index out of range");
                return;
            }
            ActiveHands[index].ChangePositionSmooth(data);
        }

        // public void RefreshLinesPosition()
        // {
        //     foreach (var hand in ActiveHands)
        //     {
        //         hand.RefreshLines();
        //     }
        // }
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

        private void SpawnNew(BonesData points)
        {
            IHandVisualiser hand = null;
            if (type == VisualizationType.Bones)
            {
                var newHand = GameObject.Instantiate(Bones, parent);
                newHand.gameObject.name = newHand.gameObject.name.Replace("(Clone)", $"_{ActiveHands.Count}");
                hand = newHand.GetComponent<SupportHandVisualizer>();
            }
            else
            {
                if (points.Type() == HandType.left)
                {
                    var newHand = GameObject.Instantiate(LeftHand, parent);
                    newHand.gameObject.name = newHand.gameObject.name.Replace("(Clone)", $"_{ActiveHands.Count}");
                    hand = newHand.GetComponent<GhostHandVisualiser>();
                }
                else
                {
                    var newHand = GameObject.Instantiate(RightHand, parent);
                    newHand.gameObject.name = newHand.gameObject.name.Replace("(Clone)", $"_{ActiveHands.Count}");
                    hand = newHand.GetComponent<GhostHandVisualiser>();
                }
                  
            }
          
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