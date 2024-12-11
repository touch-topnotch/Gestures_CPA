using System.Collections.Generic;
using Scripts.Events;
using Scripts.Gestures;
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
        public Transform Parent;
        
        [HideInInspector]
        public List<IHandVisualiser> activeHands = new List<IHandVisualiser>();
        [HideInInspector]
        public List<IHandVisualiser> hiddenHands = new List<IHandVisualiser>();
        private UpdateEvent _onUpdate;

        [Inject]
        private void Construct(UpdateEvent onUpdate)
        {
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
                activeHands[^1].ChangePosition(data, Parent);
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
            activeHands[index].ChangePosition(data, Parent);
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
            
            activeHands[index].ChangePositionSmooth(data);
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
            
            foreach (IHandVisualiser hand in activeHands)
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
            IHandVisualiser hand = null;
            if (type == VisualizationType.Bones)
            {
                var newHand = GameObject.Instantiate(Bones, Parent);
                newHand.gameObject.name = newHand.gameObject.name.Replace("(Clone)", $"_{activeHands.Count}");
                hand = newHand.GetComponent<SupportHandVisualizer>();
            }
            else
            {
                if (points.Type() == HandType.left)
                {
                    var newHand = GameObject.Instantiate(LeftHand, Parent);
                    newHand.gameObject.name = newHand.gameObject.name.Replace("(Clone)", $"_{activeHands.Count}");
                    hand = newHand.GetComponent<GhostHandVisualiser>();
                }
                else
                {
                    var newHand = GameObject.Instantiate(RightHand, Parent);
                    newHand.gameObject.name = newHand.gameObject.name.Replace("(Clone)", $"_{activeHands.Count}");
                    hand = newHand.GetComponent<GhostHandVisualiser>();
                }
                  
            }
          
            hand.Initialize(ref _onUpdate);
            hand.ChangePosition(points, Parent);
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