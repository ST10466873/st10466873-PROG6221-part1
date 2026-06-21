using System.Collections.Generic;

namespace CybersecurityChatbot
{
    /// <summary>
    /// Tracks the current user's session state including identity,
    /// conversation history, mood, preferences, and topic memory.
    /// Enables personalized and context-aware chatbot interactions.
    /// </summary>
    public class UserSession
    {
        /// <summary>Gets or sets the user's display name.</summary>
        public string UserName { get; set; } = "Guest";

        /// <summary>Gets or sets the last cybersecurity topic discussed.</summary>
        public string LastTopic { get; set; } = "";

        /// <summary>Gets or sets the detected sentiment/mood: Neutral, Anxious, Curious, Frustrated, Excited.</summary>
        public string CurrentMood { get; set; } = "Neutral";

        /// <summary>Gets or sets the running count of messages exchanged in this session.</summary>
        public int ConversationCount { get; set; } = 0;

        /// <summary>Stores user preferences such as favorite topics for personalized responses.</summary>
        public Dictionary<string, string> UserPreferences { get; set; } = new Dictionary<string, string>();

        /// <summary>Increments the conversation counter on each user message.</summary>
        public void RecordMessage()
        {
            ConversationCount++;
        }

        /// <summary>Indicates whether this user has sent more than one message.</summary>
        public bool IsReturningUser => ConversationCount > 1;

        /// <summary>Generates a personalized greeting based on conversation history.</summary>
        public string GetGreeting()
        {
            if (IsReturningUser)
                return $"Welcome back, {UserName}! We've had {ConversationCount} conversations so far.";
            return $"Hello, {UserName}! I'm here to help you stay safe online.";
        }
    }
}
