using Scripts.Hands;

namespace Scripts.PlayerLogic
{
    public class LocalVRRig: PlayerRig
    {
        protected BonesData _left = new BonesData(HandType.left);
        protected BonesData _right = new BonesData(HandType.right);
        
    }
}