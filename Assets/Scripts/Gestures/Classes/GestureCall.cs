using System;
using System.Collections.Generic;
using Design.GUI_Gesture;

namespace Scripts.Gestures.Classes
{
    public abstract class GestureCall
    {
        private List<Asset> _assets = new();
        public Action onEnded;

        public virtual void Start()
        {
            
        }
        
        internal void Destroy()
        {
            foreach (var asset in _assets)
            {
                asset.Disable();
            }
        }
    }
}