using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot
{
    
    public delegate string SentimentModifier(string baseResponse);

   
    public class ChatBotLogic
    {
        private Random _rng = new Random();

        private readonly Dictionary<string, List<string>> _knowledgeBase = new Dictionary<string, List<string>>
        {
            { "password", new List<string> {
                "Always use a mix of uppercase, lowercase, numbers, and symbols.",
                "A passphrase (four random words) is often stronger than a complex short password.",
                "Never reuse passwords. Consider a trusted password manager!"
            }},
            { "phishing", new List<string> {
                "Always hover over links in emails to see the real destination URL.",
                "Banks will never ask for your PIN or password via email.",
                "Urgent threats (e.g., 'Your account will be suspended!') are classic phishing red flags."
            }}
        };

        public string ProcessInput(string input, UserSession session, SentimentModifier modifier)
        {
            string lowerInput = input.ToLower();

            if (lowerInput.Contains("i like") || lowerInput.Contains("interested in"))
            {
                string topic = lowerInput.Split(new[] { "in ", "like " }, StringSplitOptions.None).Last().Trim('.', '!');
                session.UserPreferences["FavoriteTopic"] = topic;
                return $"I'll remember that you are interested in {topic}. I'll focus on that in our chats!";
            }

            if (lowerInput.Contains("tell me more") || lowerInput.Contains("explain"))
            {
                if (!string.IsNullOrEmpty(session.LastTopic))
                {
                    string contextResponse = $"Building on what we discussed about {session.LastTopic}: " + GetRandomResponse(session.LastTopic);
                    return modifier(contextResponse);
                }
                return "I'd love to! What specific cybersecurity topic would you like me to explain?";
            }

            string? detectedKey = _knowledgeBase.Keys.FirstOrDefault(k => lowerInput.Contains(k));
            if (detectedKey != null)
            {
                session.LastTopic = detectedKey; 

                string prefix = "";
                if (session.UserPreferences.ContainsKey("FavoriteTopic") && session.UserPreferences["FavoriteTopic"].Contains(detectedKey))
                {
                    prefix = $"Since I know you like {detectedKey}, you'll find this interesting: ";
                }

                string baseResponse = prefix + GetRandomResponse(detectedKey);
                return modifier(baseResponse); 
            }

            return "I'm still learning! Could you ask me about 'passwords' or 'phishing'?";
        }

        private string GetRandomResponse(string topic)
        {
            var options = _knowledgeBase[topic];
            return options[_rng.Next(options.Count)];
        }
    }
}