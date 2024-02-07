namespace Scripts.Design.GUI_Gesture
{
    public enum AssetType
    {
        RESOURCE,
        ADDRESSABLE,
        
    }
    public class Asset
    {
        private AssetType type;
        private string path;
        public Asset(AssetType type, string path)
        {
            this.type = type;
            this.path = path;
        }
    }
}