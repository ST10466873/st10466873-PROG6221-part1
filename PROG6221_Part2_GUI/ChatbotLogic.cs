using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot
{
    /// <summary>
    /// Delegate for modifying chatbot responses based on detected user sentiment.
    /// Allows dynamic tone adjustment (e.g., soothing for anxious, energetic for curious).
    /// </summary>
    /// <param name="baseResponse">The original response text.</param>
    /// <returns>Modified response with sentiment-aware prefix/suffix.</returns>
    public delegate string SentimentModifier(string baseResponse);

    /// <summary>
    /// Core chatbot logic with a knowledge base of cybersecurity topics.
    /// Provides keyword detection, random responses, topic memory, and conversation flow.
    /// Covers 10 cybersecurity topics with varied random responses each.
    /// </summary>
    public class ChatBotLogic
    {
        private Random _rng = new Random();

        /// <summary>
        /// Knowledge base mapping keywords to lists of possible responses.
        /// Each topic has 5-6 unique responses for variety in conversation.
        /// </summary>
        private readonly Dictionary<string, List<string>> _knowledgeBase = new Dictionary<string, List<string>>
        {
            { "password", new List<string> {
                "Always use a mix of uppercase, lowercase, numbers, and symbols for strong passwords.",
                "A passphrase of four random words is often stronger than a complex short password.",
                "Never reuse passwords across sites. Use a trusted password manager!",
                "Enable Two-Factor Authentication (2FA) wherever possible for extra security.",
                "Change your passwords immediately if you suspect a breach.",
                "Avoid using personal info like birthdays or pet names in your passwords."
            }},
            { "phishing", new List<string> {
                "Always hover over links in emails to see the real destination URL before clicking.",
                "Legitimate banks will never ask for your PIN or password via email.",
                "Urgent threats like 'Your account will be suspended!' are classic phishing red flags.",
                "Check for spelling errors and generic greetings, which are common in phishing emails.",
                "Report phishing emails to your IT department or use the 'Report Phishing' feature.",
                "Phishing can also happen via SMS (smishing) or phone calls (vishing)."
            }},
            { "safe browsing", new List<string> {
                "Ensure websites use HTTPS - look for the padlock icon in your browser's address bar.",
                "Avoid clicking on pop-up ads or downloading software from untrusted sources.",
                "Use a VPN when connecting to public Wi-Fi to encrypt your traffic.",
                "Keep your browser and extensions updated to patch security vulnerabilities.",
                "Enable 'Do Not Track' and use private browsing for sensitive activities.",
                "Clear your cookies and cache regularly to remove tracking data."
            }},
            { "malware", new List<string> {
                "Malware includes viruses, worms, trojans, ransomware, and spyware.",
                "Never download attachments from unknown email senders.",
                "Use reputable antivirus software and keep it updated.",
                "Regular scans can detect and remove malware before it causes damage.",
                "Be cautious with USB drives from unknown sources - they can carry malware."
            }},
            { "2fa", new List<string> {
                "2FA adds a second layer of security beyond just your password.",
                "Common 2FA methods include SMS codes, authenticator apps, and biometrics.",
                "Authenticator apps like Google Authenticator are more secure than SMS-based 2FA.",
                "Even if your password is stolen, 2FA can prevent unauthorized access.",
                "Enable 2FA on all accounts that support it - email, banking, social media."
            }},
            { "ransomware", new List<string> {
                "Ransomware encrypts your files and demands payment for their release.",
                "Never pay the ransom - it doesn't guarantee you'll get your data back.",
                "Regular backups to an external drive or cloud protect against ransomware.",
                "Keep your operating system and software updated to prevent ransomware infections.",
                "Be wary of email attachments and suspicious links - common ransomware delivery methods."
            }},
            { "social engineering", new List<string> {
                "Social engineering manipulates people into revealing confidential information.",
                "Attackers often impersonate IT support, executives, or trusted vendors.",
                "Always verify identities through a different communication channel before sharing info.",
                "Be suspicious of unsolicited requests for sensitive data, even from internal contacts.",
                "Security awareness training is the best defense against social engineering."
            }},
            { "backup", new List<string> {
                "Follow the 3-2-1 rule: 3 copies, 2 different media, 1 off-site backup.",
                "Automate your backups so you never forget to protect your data.",
                "Test your backups regularly to ensure they can be restored.",
                "Cloud backups are convenient but encrypt sensitive files before uploading.",
                "Backups protect against ransomware, hardware failure, and accidental deletion."
            }},
            { "wifi", new List<string> {
                "Public Wi-Fi networks are convenient but can be easily intercepted by attackers.",
                "Use a VPN when connecting to public Wi-Fi at cafes, airports, or hotels.",
                "Disable automatic Wi-Fi connectivity to prevent connecting to rogue hotspots.",
                "Ensure your home Wi-Fi uses WPA3 encryption with a strong password.",
                "Change your router's default admin credentials to prevent unauthorized access."
            }},
            { "update", new List<string> {
                "Software updates often contain critical security patches for known vulnerabilities.",
                "Enable automatic updates for your operating system and applications.",
                "Zero-day exploits target unpatched software - updates protect you from these.",
                "Don't postpone updates - attackers move fast once vulnerabilities are disclosed.",
                "Keep all devices updated: phones, tablets, laptops, and IoT devices."
            }}
        };

        /// <summary>
        /// Processes user input through keyword detection and returns a contextual response.
        /// Supports follow-up questions, topic memory, user preferences, and sentiment modification.
        /// </summary>
        /// <param name="input">The user's raw input text.</param>
        /// <param name="session">The current user session for memory/context.</param>
        /// <param name="modifier">Sentiment modifier delegate for tone adjustment.</param>
        /// <returns>Appropriate chatbot response string.</returns>
        public string ProcessInput(string input, UserSession session, SentimentModifier modifier)
        {
            string lowerInput = input.ToLower().Trim();

            if (lowerInput.Contains("i like") || lowerInput.Contains("interested in"))
            {
                string topic = lowerInput.Split(new[] { "in ", "like " }, StringSplitOptions.None).Last().Trim('.', '!');
                session.UserPreferences["FavoriteTopic"] = topic;
                return $"I'll remember that you are interested in {topic}. I'll focus more on that in our chats!";
            }

            if (lowerInput.Contains("tell me more") || lowerInput.Contains("explain") || lowerInput.Contains("elaborate"))
            {
                if (!string.IsNullOrEmpty(session.LastTopic))
                {
                    string contextResponse = $"Building on what we discussed about {session.LastTopic}: " + GetRandomResponse(session.LastTopic);
                    return modifier(contextResponse);
                }
                return "I'd love to! What specific cybersecurity topic would you like me to explain? Try asking about passwords, phishing, safe browsing, or malware.";
            }

            if (lowerInput.Contains("hello") || lowerInput.Contains("hi ") || lowerInput.Contains("hey"))
            {
                string moodPrefix = session.CurrentMood == "Anxious" ? "I can see you're concerned, but don't worry! " : "";
                return modifier($"{moodPrefix}Hello {session.UserName}! How can I help you with cybersecurity today? You can ask me about passwords, phishing, safe browsing, and more.");
            }

            if (lowerInput.Contains("thank") || lowerInput.Contains("thanks"))
            {
                return $"You're welcome, {session.UserName}! Stay safe online, and feel free to ask me anything about cybersecurity.";
            }

            if (lowerInput.Contains("how are you") || lowerInput.Contains("how do you work"))
            {
                return $"I'm functioning optimally, {session.UserName}! I'm a cybersecurity awareness chatbot. I can answer questions, run quizzes, manage tasks, and track our conversation history.";
            }

            if (lowerInput.Contains("quiz") || lowerInput.Contains("test me") || lowerInput.Contains("challenge"))
            {
                return "I'd be happy to quiz you! Switch to the Quiz tab to test your cybersecurity knowledge with 11 questions.";
            }

            if (lowerInput.Contains("task") || lowerInput.Contains("remind"))
            {
                return "You can manage tasks in the Tasks tab! Add cybersecurity tasks with reminders, track completion, and stay organized.";
            }

            if (lowerInput.Contains("log") || lowerInput.Contains("history") || lowerInput.Contains("activity"))
            {
                return "Check the Activity Log tab to see a record of our conversation and your actions.";
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

            return $"I'm still learning! Could you ask me about passwords, phishing, safe browsing, malware, 2FA, ransomware, social engineering, backups, Wi-Fi security, or software updates?";
        }

        /// <summary>
        /// Gets a random response for a given topic from the knowledge base.
        /// Provides varied, non-repetitive answers to keep conversations natural.
        /// </summary>
        /// <param name="topic">The knowledge base topic key.</param>
        /// <returns>A randomly selected response string.</returns>
        private string GetRandomResponse(string topic)
        {
            var options = _knowledgeBase[topic];
            return options[_rng.Next(options.Count)];
        }
    }
}
