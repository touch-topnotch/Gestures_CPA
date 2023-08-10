namespace Scripts.Gestures
{
    public delegate void FrameDetected(int frameId, GestureFrame frame);

    public delegate IGNode FramesEnded();
    public interface IGNode
    {
        GestureFrame GetGestureFrame();
        void FrameRecognized();
    }
}