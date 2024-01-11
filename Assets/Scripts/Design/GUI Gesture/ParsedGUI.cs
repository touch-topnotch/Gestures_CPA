using System.Collections.Generic;
using Scripts.Gestures;
using Scripts.Gestures.GGUI;

namespace Design.GUI_Gesture
{
    public class ParsedGUI:GUIGesture
    {
        internal List<Asset> assets;
        internal List<string> frameLogic;
        public ParsedGUI(List<Asset> assets, List<string> frameLogic )
        {
            this.assets = assets;
            this.frameLogic = frameLogic;
        }
        protected override void Construct()
        {
            
        }

        public override void ShowEffects(int frameId, GestureFrame gFrame) 
        {
           
        }

        protected override void OnDestroyed()
        {
          //  throw new System.NotImplementedException();
        }
    }
}