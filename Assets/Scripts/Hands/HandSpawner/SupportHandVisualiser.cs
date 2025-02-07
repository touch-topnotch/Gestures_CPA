using System;
using System.Collections.Generic;
using Gesture_Editor_SDK.EditorAttributes.SerializeByTypeAttribute;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.Static;
using UnityEngine;
using Zenject;

namespace Scripts.Hands
{
    public enum VisualizationType {Mesh, Bones}
    public class SupportHandVisualiser: MonoBehaviour
    {
        public VisualizationType visualizationType;
        [SerializeByType("Scripts.Hands.VisualizationType",0)]
        public GameObject LeftHand;
        [SerializeByType("Scripts.Hands.VisualizationType","Mesh")]
        public GameObject RightHand;
        [SerializeByType("Scripts.Hands.VisualizationType","Bones")]
        public GameObject Bones; 
        public Transform Parent; 
        public float speed; 
        [HideInInspector]
        public List<HandMesh> activeHands = new List<HandMesh>();
        [HideInInspector]
        public List<HandMesh> hiddenHands = new List<HandMesh>();
        private UpdateEvent _onUpdate => UpdateEvent.Instance;

        private void OnValidate()
        {
            if (Parent == null)
            {
                Parent = transform;
            }
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
            if (!data.Exists())
                return;
            
            if (hiddenHands.Count == 0)
            {
                SpawnNew(data);
                return;
            }

            if (hiddenHands.Count > 0)
            {
                // move hand from hidden to active
                activeHands.Add(hiddenHands[0]);
                hiddenHands.RemoveAt(0);
                activeHands[^1].Show();
                activeHands[^1].ChangePosition(data);
            }
        }

        public void AddToStack(HandsStruct hands)
        {
            AddToStack(hands.LeftBones);
            AddToStack(hands.RightBones);
        }

        public void OverrideHand(BonesData data, int index = -1)
        {
            if (!data.Exists())
                return;
            if (index >= activeHands.Count)
            {
                AddToStack(data);
                return;
            }
            if (index == -1)
            {
                index = activeHands.Count - 1;
            }
            activeHands[index].ChangePosition(data);
        }

        public void OverrideHands(HandsStruct hands)
        {
            if (activeHands.Count >= 2)
            {
                OverrideHand(hands.LeftBones, 0);
                OverrideHand(hands.RightBones, 1);
            }
            else
            {
                if (activeHands.Count == 1)
                {
                    OverrideHand(hands.LeftBones, 0);
                    AddToStack(hands.RightBones);
                    return;
                }
                AddToStack(hands);
            }
        }

        public void MoveHand(BonesData data, int index = -1)
        {
            if (index >= activeHands.Count)
            {
                Debug.LogAssertion("Index out of range");
                return;
            }

            if (index == -1)
            {
                index = activeHands.Count - 1;
            }
            
            activeHands[index].ChangePositionSmooth(data, speed, ()=>{});
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
            if(activeHands.Count == 0)
                return;
            
            foreach (HandMesh hand in activeHands)
            {
                hand.Hide();
            }
            hiddenHands.AddRange(activeHands);
            activeHands.Clear();
        }

        private void SpawnNew(BonesData points)
        {
            if (points == null)
            {
                Debug.Log("Cannot Spawn new Support Hand without Bones Data!");
                return;
            }
            HandMesh hand = null;
            if (visualizationType == VisualizationType.Bones)
            {
                var newHand = GameObject.Instantiate(Bones, Parent);
                newHand.gameObject.name = newHand.gameObject.name.Replace("(Clone)", $"_{activeHands.Count}");
                hand = newHand.GetComponent<HandMesh>();
            }
            else
            {
                if (points.Type() == HandType.left)
                {
                    var newHand = GameObject.Instantiate(LeftHand, Parent);
                    newHand.gameObject.name = newHand.gameObject.name.Replace("(Clone)", $"_{activeHands.Count}");
                    hand = newHand.GetComponent<HandMesh>();
                }
                else
                {
                    var newHand = GameObject.Instantiate(RightHand, Parent);
                    newHand.gameObject.name = newHand.gameObject.name.Replace("(Clone)", $"_{activeHands.Count}");
                    hand = newHand.GetComponent<HandMesh>();
                }
                  
            }
            
            hand.ChangePosition(points);
            hand.Show();
            activeHands.Add(hand);
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