using System.Collections.Generic;

namespace CybersecurityChatbot
{
    public class QuizManager
    {
        public int Score { get; private set; } = 0;

        public Dictionary<string, (string[] Options, string Answer, string Explanation)> Questions = new Dictionary<string, (string[], string, string)>
        {
            { "What should you do if an email asks for your password?", (new[] { "A) Reply", "B) Delete", "C) Report as phishing", "D) Ignore" }, "C", "Reporting helps prevent further scams." ) },
            { "True or False: Public Wi-Fi is safe for banking.", (new[] { "True", "False" }, "False", "Public networks can be easily intercepted." ) },
            { "Which is a strong password?", (new[] { "A) pass123", "B) admin", "C) P@ssw0rd!", "D) T!g3r$#99x" }, "D", "It uses a mix of cases, numbers, and symbols." ) },
            { "What does 2FA stand for?", (new[] { "A) Two-File Access", "B) Two-Factor Authentication", "C) Two-Face App", "D) To Find Anyone" }, "B", "2FA adds an extra layer of security." ) },
            { "True or False: Reuse passwords across sites.", (new[] { "True", "False" }, "False", "If one site breaches, all are at risk." ) },
            { "What is ransomware?", (new[] { "A) PC cleaner", "B) Malware that locks files for a fee", "C) VPN", "D) Firewall" }, "B", "It holds data hostage until paid." ) },
            { "How often to update software?", (new[] { "A) Never", "B) Yearly", "C) Immediately", "D) When broken" }, "C", "Updates patch security holes." ) },
            { "True or False: A padlock icon means 100% safe.", (new[] { "True", "False" }, "False", "It only means connection is encrypted; scammers use it too." ) },
            { "What is social engineering?", (new[] { "A) Networking", "B) Manipulating people for data", "C) Social apps", "D) Virus" }, "B", "Relies on human error." ) },
            { "Best way to back up files?", (new[] { "A) Desktop", "B) Email", "C) External drive/Cloud", "D) Print" }, "C", "Protects against ransomware." ) },
            { "True or False: Phishing is only via email.", (new[] { "True", "False" }, "False", "Can happen via SMS or calls." ) }
        };

        public string CheckAnswer(string question, string userAnswer)
        {
            var qData = Questions[question];
            if (qData.Answer.Substring(0, 1).Equals(userAnswer, System.StringComparison.OrdinalIgnoreCase))
            {
                Score++;
                return $"Correct! {qData.Explanation}";
            }
            return $"Incorrect. Answer: {qData.Answer}. {qData.Explanation}";
        }
    }
}