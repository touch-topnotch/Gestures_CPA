namespace CrossPlatform.UI
{
    public class UIText:UIElement
    {
        private string _text;
        
        public string GetText()
        {
            return _text;
        }

        public void SetText(string text)
        {
            _text = text;
        }
    }
}