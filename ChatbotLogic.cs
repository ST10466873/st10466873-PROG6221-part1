using System;
using System.Collections.Generic;
using System.Text;

namespace st10466873_PROG6221_poE
{
    internal class ChatbotLogic
    {
        private Random _random = new Random();

        private Dictionary<string, List<string>> _topicTips = new Dictionary<string, List<string>>
        {
            { "phishing", new List<string> {
                "Always check if the sender's domain matches the official website.",
                "Be cautious of emails creating a false sense of urgency.",
                "Never enter your password on a site reached via an email link."
            }},
            { "password", new List<string> {
                "Use a mix of uppercase, lowercase, numbers, and symbols.",
                "Consider using a passphrase of four random words.",
                "Enable Multi-Factor Authentication (MFA) whenever possible."
            }}
        };

        private readonly string[] _worriedKeywords = { "worried", "scared", "nervous", "afraid", "unsure" };

        public string GetChatbotResponse(string input, UserSession session)
        {
            string lowerInput = input.ToLower();

            if (_worriedKeywords.Any(w => lowerInput.Contains(w)))
            {
                return "It's completely understandable to feel that way. Cyber threats are complex, but taking small steps makes a big difference. Let's start with a tip: " + GetRandomTip("phishing");
            }

            if (lowerInput.Contains("interested in"))
            {
                string topic = lowerInput.Split(new[] { "interested in " }, StringSplitOptions.None).Last().TrimEnd('!', '.');
                session.FavoriteTopic = topic;
                return $"Great! I'll remember that you're interested in {topic}. It's a crucial part of staying safe online.";
            }

            if (lowerInput.Contains("tell me more") || lowerInput.Contains("explain more") || lowerInput.Contains("another tip"))
            {
                if (!string.IsNullOrEmpty(session.LastTopic))
                    return $"Sure! Here is another tip regarding {session.LastTopic}: " + GetRandomTip(session.LastTopic);

                return "I'd love to explain more! What topic are you currently worried about? (Passwords, Phishing, etc.)";
            }

            if (lowerInput.Contains("phishing")) { session.LastTopic = "phishing"; return GetRandomTip("phishing"); }
            if (lowerInput.Contains("password")) { session.LastTopic = "password"; return GetRandomTip("password"); }

            if (lowerInput.Contains("what do i like") && !string.IsNullOrEmpty(session.FavoriteTopic))
            {
                return $"You mentioned earlier that you are interested in {session.FavoriteTopic}!";
            }

            return "I'm not sure I understand. Can you try rephrasing? You can ask me about 'passwords', 'phishing', or tell me what you are 'interested in'.";
        }

        private string GetRandomTip(string topic)
        {
            if (_topicTips.ContainsKey(topic))
            {
                var list = _topicTips[topic];
                return list[_random.Next(list.Count)];
            }
            return "Stay alert and always keep your software updated!";
        }
    }
}

/*References:
 
 OpenAI. 2026. ChatGPT. [Generative AI]. Available at: https://chatgpt.com [Accessed 11 May 2026].
Patorjk.com. 2026. TAAG - Text to ASCII Art Generator. [Online]. Available at: https://patorjk.com/software/taag/ [Accessed 11 May 2026].
*/