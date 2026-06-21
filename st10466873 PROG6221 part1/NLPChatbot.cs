using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CybersecurityChatbot
{
    public class NLPChatbot
    {
        private List<string> _activityLog = new List<string>();
        public DatabaseManager Db = new DatabaseManager();

        public void AddLog(string action) => _activityLog.Add($"[{DateTime.Now:HH:mm:ss}] {action}");

        public string ProcessNLP(string input)
        {
            string lower = input.ToLower();

            if (Regex.IsMatch(lower, @"\b(add|create|remind)\b.*\b(task|me)\b"))
            {
                string task = Regex.Replace(lower, @"\b(add|create|a|task|remind|me|to)\b", "").Trim();
                Db.AddTask(task, "NLP Task", null);
                AddLog($"NLP Task added: '{task}'");
                return $"Added '{task}' to your tasks.";
            }

            if (Regex.IsMatch(lower, @"\b(log|actions|done)\b"))
            {
                AddLog("Viewed Activity Log");
                var recent = _activityLog.Skip(Math.Max(0, _activityLog.Count - 5)).ToList();
                return "Recent actions:\n" + string.Join("\n", recent) + "\n\n(Type 'show more' for full history)";
            }

            if (lower.Contains("show more"))
            {
                return "Full Log:\n" + string.Join("\n", _activityLog);
            }

            return "I didn't catch that. Try asking to 'add a task' or 'show log'.";
        }
    }
}