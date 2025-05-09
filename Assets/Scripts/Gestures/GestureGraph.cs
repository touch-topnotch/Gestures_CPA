using System.Collections.Generic;

namespace Scripts.Gestures
{
    public class GestureGraph
    {
        public DynamicGesture OwnGesture;
        public List<GestureGraph> PossibleBranches;
    }
}