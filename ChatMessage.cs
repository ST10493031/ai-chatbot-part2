using System.Windows.Media;

namespace chatbot_ai
{
    public class ChatMessage
    {
        public string Text { get; set; }
        public Brush Color { get; set; }
        public bool IsUser { get; set; }
    }
}