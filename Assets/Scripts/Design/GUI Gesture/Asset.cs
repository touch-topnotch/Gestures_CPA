namespace Design.GUI_Gesture
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
        private string spawnPoint;
        public Asset(AssetType type, string path, string spawnPoint)
        {
            this.type = type;
            this.path = path;
            this.spawnPoint = spawnPoint;
        }
        public void Disable()
        {
            
        }
    }
}