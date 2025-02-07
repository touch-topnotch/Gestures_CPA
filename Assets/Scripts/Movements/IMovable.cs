namespace Scripts.Movements
{
    public interface IMovable
    {
        public bool isMoved();
        public void StartMove();
        public void StopMove();
    }
}