using System.Linq;

namespace chatbot_ai
{
    public class SentimentAnalyzer
    {
        private readonly string[] _negativeWords = { "hate", "angry", "frustrated", "useless", "stupid", "bad", "terrible" };
        private readonly string[] _positivePrefixes = { "I understand", "I see", "Thanks for sharing" };

        public string DetectSentiment(string input, out bool isNegative)
        {
            isNegative = _negativeWords.Any(word => input.Contains(word));
            if (isNegative)
            {
                return "I'm sorry you feel that way. Cybersecurity can be frustrating, but let me help. ";
            }
            return string.Empty;
        }
    }
}