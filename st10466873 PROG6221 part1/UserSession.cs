using System.Collections.Generic;

namespace CybersecurityChatbot
{
    
    public class UserSession
    {
        public string UserName { get; set; } = "Guest";
        public string LastTopic { get; set; } = "";
        public string CurrentMood { get; set; } = "Neutral";

        public Dictionary<string, string> UserPreferences { get; set; } = new Dictionary<string, string>();
    }
}