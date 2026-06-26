using System;
using System.Collections.Generic;
using System.Linq;

namespace chatbot_ai
{
    public class ResponseManager
    {
        private readonly Dictionary<string, List<string>> _responses = new()
        {
            ["password"] = new List<string>
            {
                "Use a unique, long password for each account. Consider a passphrase like 'Correct-Horse-Battery-Staple'.",
                "Enable two-factor authentication wherever possible – it adds a second layer of security.",
                "Never reuse passwords across different sites. A password manager helps you keep track."
            },
            ["phishing"] = new List<string>
            {
                "Phishing emails often create urgency ('Your account will be closed!'). Hover over links before clicking.",
                "Never enter personal info after clicking a link in an email – go directly to the official website.",
                "Look for spelling mistakes and mismatched sender addresses – common signs of phishing."
            },
            ["scam"] = new List<string>
            {
                "If an offer sounds too good to be true, it probably is. Scammers prey on greed and fear.",
                "Never send money or gift cards to someone you've only met online – a classic scam tactic.",
                "Verify caller identity by hanging up and calling the official number of the company."
            },
            ["privacy"] = new List<string>
            {
                "Review your social media privacy settings regularly. Limit who can see your posts.",
                "Use a VPN on public Wi-Fi to encrypt your traffic and protect your data.",
                "Be mindful of what you share online – once posted, it's hard to take back."
            },
            ["safe browsing"] = new List<string>
            {
                "Keep your browser updated and use extensions that block malicious scripts.",
                "Check for 'https://' and the padlock icon before entering any sensitive information.",
                "Avoid clicking on pop-up ads – they can lead to drive-by downloads."
            }
        };

        public string DetectKeyword(string input)
        {
            foreach (var keyword in _responses.Keys)
            {
                if (input.Contains(keyword))
                    return keyword;
            }
            return null;
        }

        public string GetRandomResponse(string topic)
        {
            if (_responses.TryGetValue(topic, out var list) && list.Count > 0)
            {
                var random = new Random();
                return list[random.Next(list.Count)];
            }
            return null;
        }

        public bool HasFollowUpIntent(string input)
        {
            string[] followPhrases = { "another tip", "more", "tell me more", "elaborate" };
            return followPhrases.Any(phrase => input.Contains(phrase));
        }

        public string ExtractIntent(string input)
        {
            if (input.Contains("add task") || input.Contains("create task") ||
                input.Contains("new task") || input.Contains("save task"))
                return "ADD_TASK";

            if (input.Contains("show tasks") || input.Contains("view tasks") ||
                input.Contains("list tasks") || input.Contains("my tasks"))
                return "VIEW_TASKS";

            if (input.Contains("remind me") || input.Contains("set reminder") ||
                input.Contains("remind about"))
                return "SET_REMINDER";

            if (input.Contains("quiz") || input.Contains("play quiz") ||
                input.Contains("start quiz") || input.Contains("take quiz"))
                return "START_QUIZ";

            if (input.Contains("activity log") || input.Contains("show log") ||
                input.Contains("what have you done") || input.Contains("show history"))
                return "SHOW_LOG";

            if (input.Contains("complete task") || input.Contains("finish task") ||
                input.Contains("mark done"))
                return "COMPLETE_TASK";

            if (input.Contains("delete task") || input.Contains("remove task") ||
                input.Contains("clear task"))
                return "DELETE_TASK";

            return "UNKNOWN";
        }
    }
}