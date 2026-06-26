using System.Collections.Generic;
using System.Text;

namespace chatbot_ai
{
    public class UserMemory
    {
        public string Name { get; set; }
        private readonly List<string> _interests = new List<string>();

        public void AddInterest(string topic)
        {
            if (!_interests.Contains(topic))
                _interests.Add(topic);
        }

        public string RecallInterests()
        {
            if (_interests.Count == 0) return null;
            var sb = new StringBuilder("I remember you're interested in ");
            sb.Append(string.Join(", ", _interests));
            sb.Append(". ");
            return sb.ToString();
        }
    }
}