using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CybersecurityChatbot
{
    /// <summary>
    /// Simulates natural language processing for the chatbot.
    /// Detects keywords and phrases to create tasks, view logs, and manage activities.
    /// Maintains an activity log for tracking user interactions.
    /// </summary>
    public class NLPChatbot
    {
        private List<string> _activityLog = new List<string>();

        /// <summary>Gets the database manager instance for task operations.</summary>
        public DatabaseManager Db = new DatabaseManager();

        /// <summary>Gets a read-only view of the full activity log.</summary>
        public IReadOnlyList<string> ActivityLog => _activityLog.AsReadOnly();

        /// <summary>Adds a timestamped entry to the activity log.</summary>
        /// <param name="action">Description of the action to log.</param>
        public void AddLog(string action) => _activityLog.Add($"[{DateTime.Now:HH:mm:ss}] {action}");

        /// <summary>Gets the most recent log entries.</summary>
        /// <param name="count">Number of recent entries to return (default 5).</param>
        /// <returns>List of recent log entries.</returns>
        public List<string> GetRecentLogs(int count = 5)
        {
            return _activityLog.Skip(Math.Max(0, _activityLog.Count - count)).ToList();
        }

        /// <summary>Gets all log entries as a single formatted string.</summary>
        public string GetAllLogs()
        {
            return string.Join("\n", _activityLog);
        }

        /// <summary>Clears all log entries and records the clear action.</summary>
        public void ClearLogs()
        {
            _activityLog.Clear();
            AddLog("Activity log cleared");
        }

        /// <summary>
        /// Processes natural language input and returns an appropriate response.
        /// Supports: adding tasks, viewing logs, clearing logs, listing tasks.
        /// </summary>
        /// <param name="input">The user's input text.</param>
        /// <returns>Response string based on detected intent.</returns>
        public string ProcessNLP(string input)
        {
            string lower = input.ToLower();

            if (Regex.IsMatch(lower, @"\b(add|create|new|remind)\b.*\b(task|me|todo)\b"))
            {
                string task = Regex.Replace(lower, @"\b(add|create|a|new|task|remind|me|to|todo)\b", "").Trim();
                if (string.IsNullOrWhiteSpace(task))
                    return "What task would you like me to add? Please include details.";
                Db.AddTask(task, "NLP Task", null);
                AddLog($"NLP Task added: '{task}'");
                return $"Added '{task}' to your tasks. You can view all tasks in the Tasks tab.";
            }

            if (Regex.IsMatch(lower, @"\b(log|actions|history|done)\b"))
            {
                AddLog("Viewed Activity Log");
                var recent = GetRecentLogs(5);
                return "Recent actions:\n" + string.Join("\n", recent) + "\n\n(Type 'show more' for full history)";
            }

            if (lower.Contains("show more") || lower.Contains("full log") || lower.Contains("all history"))
            {
                return "Full Log:\n" + GetAllLogs();
            }

            if (lower.Contains("clear log") || lower.Contains("reset log"))
            {
                ClearLogs();
                return "Activity log has been cleared.";
            }

            if (lower.Contains("list tasks") || lower.Contains("show tasks") || lower.Contains("my tasks"))
            {
                var tasks = Db.ReadActiveTasks();
                if (tasks.Count == 0)
                    return "You have no active tasks. Add one by saying 'add task [description]'.";
                return "Your active tasks:\n" + string.Join("\n", tasks);
            }

            return "I didn't catch that. Try asking to 'add a task', 'show log', 'list tasks', or 'clear log'.";
        }
    }
}
