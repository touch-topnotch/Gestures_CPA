using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

namespace CrossPlatform.Gestures
{
    public static class GestureGraphManager
    {   
        
        public static GestureGraph InitializeGestureGraph(List<DynamicGesture> all_gestures)
        {
           
            // write graph of _all_gestures in GestureGraph.json;
            GestureGraph heap = new GestureGraph();
            AddToGraph(new GestureGraph(), all_gestures);
            return heap;
            // в каждой ветке будем рекурсивно создавать новые, пока не закончится массив
        }

        private static void AddToGraph(GestureGraph branch, List<DynamicGesture> all_gestures)
        {
            foreach (DynamicGesture gest in all_gestures)
            {

                var ost = all_gestures;
                ost.Remove(gest);
                if(ost.Count == 0)
                    continue;
                for (int i = 0; i > ost.Count; i++)
                {
                    branch.PossibleBranches.Add(new GestureGraph());
                    branch.PossibleBranches[-1].OwnGesture = ost[i];
                    AddToGraph(branch.PossibleBranches[-1], ost);
                }
            }
        }
        
        public static DynamicGesture DynamicGestureByPath(string path)
        {
            // names = splited path by '/'
            return new DynamicGesture();
        }

        public static List<DynamicGesture> PossibleGestures() // change the name!
        {
            // go by path in GestureGraph.json and return all names of this path
            return null;
        }

       

    }

 
}