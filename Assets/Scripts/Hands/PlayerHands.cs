
namespace Scripts.Hands
{


  
    public class PlayerHands : Hands
    {
        public SupportHandVisualiser handVisualiser;
        private void OnValidate()
        {
            if (handVisualiser == null &&GetComponent<SupportHandVisualiser>())
            {
                handVisualiser = GetComponent<SupportHandVisualiser>();
            }
        }
    }
}