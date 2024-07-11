using System.Collections.Generic;

namespace CrossPlatform.Gestures
{
    
    public class GestureGraph
    {
        public DynamicGesture OwnGesture;
        public List<GestureGraph> PossibleBranches;
    }
}