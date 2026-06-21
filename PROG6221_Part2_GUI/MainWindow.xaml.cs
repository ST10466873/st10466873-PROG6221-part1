using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CybersecurityChatbot;

namespace PROG6221_Part2_GUI
{
    public partial class MainWindow : Window
    {
        private ChatBotLogic _bot = new ChatBotLogic();
        private UserSession _session = new UserSession();
        private DatabaseManager _db = new DatabaseManager();
        private QuizManager _quiz = new QuizManager();
        private NLPChatbot _nlp = new NLPChatbot();

        private string _currentQuestionKey = "";
        private int _currentQuestionIndex = 0;
        private List<string> _questionKeys = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _nlp.AddLog("Application started");
                _nlp.AddLog("Database connection established");
                AppendChat("System", "==================================================");
                AppendChat("System", "  CYBER-SHIELD ASSISTANT v1.0");
                AppendChat("System", "  Database: MySQL 8.0 | Status: Connected");
                AppendChat("System", "==================================================");
                AppendChat("Bot", "Hello! I'm your cybersecurity awareness assistant. What is your name?");
                UpdateStatusBar("Connected | MySQL Active", "#4CAF50");
            }
            catch (Exception ex)
            {
                UpdateStatusBar("DB Disconnected", "#F44336");
                AppendChat("System", $"Warning: Database connection failed - {ex.Message}");
                AppendChat("Bot", "Hello! I'm your cybersecurity awareness assistant. What is your name?");
            }
        }

        private void UpdateStatusBar(string text, string color)
        {
            StatusBar.Text = text;
            StatusBar.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        }

        private void AppendChat(string sender, string msg)
        {
            ChatDisplay.Text += $"[{sender}]: {msg}\n\n";
            ChatScroll.ScrollToBottom();
        }

        private void Send_Click(object sender, RoutedEventArgs e) => ProcessChat();

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ProcessChat();
        }

        private void ProcessChat()
        {
            string input = UserInput.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            AppendChat("You", input);
            _nlp.AddLog($"User said: \"{input}\"");
            UserInput.Clear();

            if (_session.UserName == "Guest")
            {
                _session.UserName = input;
                _nlp.AddLog($"User identified as: {input}");
                AppendChat("Bot", $"Welcome, {_session.UserName}! I'm here to help you stay safe online.\n\n" +
                    $"You can ask me about:\n" +
                    $"  • Passwords • Phishing • Safe Browsing • Malware\n" +
                    $"  • 2FA • Ransomware • Social Engineering\n" +
                    $"  • Backups • Wi-Fi Security • Software Updates\n\n" +
                    $"Or use the TASKS, QUIZ, and LOG tabs above!");
                return;
            }

            _session.RecordMessage();
            UpdateSentiment(input.ToLower());

            SentimentModifier toneAdjuster = (baseText) =>
            {
                switch (_session.CurrentMood)
                {
                    case "Anxious":
                        return $"It's completely normal to be worried, {_session.UserName}. Stay calm and let's learn how to protect you: {baseText}";
                    case "Curious":
                        return $"Great question, {_session.UserName}! Curiosity is your best defense against cyber threats. {baseText}";
                    case "Frustrated":
                        return $"I understand this can be frustrating, {_session.UserName}. Let me break it down simply: {baseText}";
                    case "Excited":
                        return $"I love your enthusiasm, {_session.UserName}! {baseText}";
                    default:
                        return baseText;
                }
            };

            string nlpResponse = _nlp.ProcessNLP(input);
            if (!nlpResponse.StartsWith("I didn't catch that"))
            {
                AppendChat("Bot", nlpResponse);
                _nlp.AddLog($"NLP response: {nlpResponse}");
                return;
            }

            string response = _bot.ProcessInput(input, _session, toneAdjuster);
            AppendChat("Bot", response);
            _nlp.AddLog($"Chatbot response: {response}");
        }

        private void UpdateSentiment(string input)
        {
            if (input.Contains("worried") || input.Contains("scared") || input.Contains("hack") || input.Contains("stolen") || input.Contains("afraid"))
                _session.CurrentMood = "Anxious";
            else if (input.Contains("why") || input.Contains("how") || input.Contains("curious") || input.Contains("what") || input.Contains("explain"))
                _session.CurrentMood = "Curious";
            else if (input.Contains("annoy") || input.Contains("frustrat") || input.Contains("stupid") || input.Contains("useless") || input.Contains("bad"))
                _session.CurrentMood = "Frustrated";
            else if (input.Contains("great") || input.Contains("awesome") || input.Contains("cool") || input.Contains("nice") || input.Contains("love"))
                _session.CurrentMood = "Excited";
            else
                _session.CurrentMood = "Neutral";
        }

        // ======================== TASK MANAGEMENT ========================

        private void LoadTasks()
        {
            try
            {
                var tasks = _db.ReadActiveTasks();
                var taskItems = new List<TaskItem>();

                foreach (var taskStr in tasks)
                {
                    var match = System.Text.RegularExpressions.Regex.Match(taskStr, @"^\[(\d+)\]\s(.+)$");
                    if (match.Success)
                    {
                        taskItems.Add(new TaskItem
                        {
                            Id = int.Parse(match.Groups[1].Value),
                            DisplayText = taskStr
                        });
                    }
                }

                TaskList.ItemsSource = taskItems;
                TaskStatus.Text = $"{taskItems.Count} active task(s)";
                _nlp.AddLog($"Loaded {taskItems.Count} tasks from database");
            }
            catch (Exception ex)
            {
                TaskStatus.Text = $"Error loading tasks: {ex.Message}";
                _nlp.AddLog($"Task load error: {ex.Message}");
            }
        }

        private void TaskRefresh_Click(object sender, RoutedEventArgs e) => LoadTasks();

        private void TaskComplete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is int id)
            {
                _db.MarkComplete(id);
                _nlp.AddLog($"Task #{id} marked as complete");
                LoadTasks();
            }
        }

        private void TaskDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is int id)
            {
                var result = MessageBox.Show($"Delete task #{id}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _db.DeleteTask(id);
                    _nlp.AddLog($"Task #{id} deleted");
                    LoadTasks();
                }
            }
        }

        // ======================== QUIZ SYSTEM ========================

        private void QuizStart_Click(object sender, RoutedEventArgs e)
        {
            _quiz = new QuizManager();
            _questionKeys = _quiz.Questions.Keys.ToList();
            _currentQuestionIndex = 0;
            QuizStartBtn.IsEnabled = false;
            QuizNextBtn.IsEnabled = false;
            _nlp.AddLog("Quiz started");
            ShowQuizQuestion();
        }

        private void ShowQuizQuestion()
        {
            if (_currentQuestionIndex >= _questionKeys.Count)
            {
                EndQuiz();
                return;
            }

            _currentQuestionKey = _questionKeys[_currentQuestionIndex];
            var qData = _quiz.Questions[_currentQuestionKey];

            QuizQuestion.Text = $"{_currentQuestionIndex + 1}. {_currentQuestionKey}";
            QuizScore.Text = $"Score: {_quiz.Score}/{_currentQuestionIndex}";
            QuizProgress.Text = $"Q: {_currentQuestionIndex + 1}/{_questionKeys.Count}";
            QuizProgressBar.Value = _currentQuestionIndex;
            QuizFeedback.Text = "Select an answer above.";
            QuizFeedbackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C2C34"));

            var options = new List<QuizOption>();
            for (int i = 0; i < qData.Options.Length; i++)
            {
                string val = qData.Options[i].Substring(0, 1);
                options.Add(new QuizOption { Text = qData.Options[i], Value = val });
            }
            QuizOptionsList.ItemsSource = options;
            QuizNextBtn.IsEnabled = false;
            _nlp.AddLog($"Quiz: showing question {_currentQuestionIndex + 1}");
        }

        private void QuizOption_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is string choice)
            {
                string result = _quiz.CheckAnswer(_currentQuestionKey, choice);
                QuizFeedback.Text = result;
                QuizScore.Text = $"Score: {_quiz.Score}/{_currentQuestionIndex + 1}";

                if (result.StartsWith("Correct"))
                    QuizFeedbackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1B5E20"));
                else
                    QuizFeedbackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B71C1C"));

                _currentQuestionIndex++;
                QuizNextBtn.IsEnabled = true;
                _nlp.AddLog($"Quiz: {_currentQuestionKey} - {result}");
            }
        }

        private void QuizNext_Click(object sender, RoutedEventArgs e)
        {
            ShowQuizQuestion();
        }

        private void EndQuiz()
        {
            int total = _questionKeys.Count;
            int score = _quiz.Score;
            string grade = score >= 9 ? "Excellent! Outstanding cybersecurity knowledge!" :
                           score >= 7 ? "Great job! You know your cybersecurity well!" :
                           score >= 5 ? "Good effort! Review the topics you missed and try again." :
                           "Keep studying! Cybersecurity is a journey, not a destination.";

            QuizQuestion.Text = "QUIZ COMPLETE!";
            QuizFeedback.Text = $"Final Score: {score}/{total} ({score * 100 / total}%)\n\n{grade}";
            QuizFeedbackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E24"));
            QuizScore.Text = $"Score: {score}/{total}";
            QuizProgress.Text = $"Done!";
            QuizProgressBar.Value = total;
            QuizOptionsList.ItemsSource = null;
            QuizStartBtn.IsEnabled = true;
            QuizNextBtn.IsEnabled = false;
            _nlp.AddLog($"Quiz completed: score {score}/{total}");
        }

        // ======================== ACTIVITY LOG ========================

        private void LogRecent_Click(object sender, RoutedEventArgs e)
        {
            var logs = _nlp.GetRecentLogs(5);
            LogList.ItemsSource = logs.ToList();
            LogCount.Text = $"{logs.Count} entries shown (recent 5)";
            _nlp.AddLog("Viewed recent activity log");
        }

        private void LogAll_Click(object sender, RoutedEventArgs e)
        {
            var logs = _nlp.ActivityLog.ToList();
            LogList.ItemsSource = logs;
            LogCount.Text = $"{logs.Count} entries (all)";
            _nlp.AddLog("Viewed full activity log");
        }

        private void LogClear_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Clear the entire activity log?", "Confirm Clear", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _nlp.ClearLogs();
                LogList.ItemsSource = null;
                LogCount.Text = "0 entries";
            }
        }
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string DisplayText { get; set; } = "";
    }

    public class QuizOption
    {
        public string Text { get; set; } = "";
        public string Value { get; set; } = "";
    }
}
