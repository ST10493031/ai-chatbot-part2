using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace chatbot_ai
{
    public partial class MainWindow : Window
    {
        private delegate void BotAction(string message);
        private BotAction _logAction;

        private UserMemory _userMemory;
        private ResponseManager _responseManager;
        private SentimentAnalyzer _sentimentAnalyzer;
        private ObservableCollection<ChatMessage> _messages;
        private string _lastTopic;

        private List<string> activityLog;
        private const int MaxLogEntries = 50;

        private enum AppMode { AwaitingName, MainMenu, Conversation }
        private AppMode _currentMode;

        private List<(string Sender, string Message, DateTime Timestamp)> _conversationHistory
            = new List<(string, string, DateTime)>();

        private readonly Random _random = new Random();
        private readonly string[] _generalTips =
        {
            "Always enable two-factor authentication on important accounts.",
            "Never share your password with anyone – not even a trusted friend.",
            "Keep your software and antivirus updated to block new threats.",
            "Use a password manager to create and store strong, unique passwords.",
            "Be cautious of unsolicited emails or messages asking for personal info."
        };

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                _messages = new ObservableCollection<ChatMessage>();
                ChatHistory.ItemsSource = _messages;

                _userMemory = new UserMemory();
                _responseManager = new ResponseManager();
                _sentimentAnalyzer = new SentimentAnalyzer();
                _currentMode = AppMode.AwaitingName;

                activityLog = new List<string>();

                _logAction = (msg) => AddSystemMessage($"[LOG] {msg}", Brushes.Gray);

                PlayVoiceGreeting();
                ShowAsciiArt();
                AddSystemMessage("Please enter your name:");
            }
            catch (Exception ex)
            {
                string innerMsg = ex.InnerException?.Message ?? "No inner exception";
                MessageBox.Show($"Startup error: {ex.Message}\nInner: {innerMsg}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

        private void ShowAsciiArt()
        {
            AddSystemMessage("╔══════════════════════════════════════════════════════════╗");
            AddSystemMessage("║               CYBERSECURITY AWARENESS BOT               ║");
            AddSystemMessage("║                  STAY SAFE ONLINE!                      ║");
            AddSystemMessage("╚══════════════════════════════════════════════════════════╝");
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                string voicePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "voice.wav");
                if (File.Exists(voicePath))
                {
                    using (SoundPlayer player = new SoundPlayer(voicePath))
                    {
                        player.Load();
                        player.Play();
                    }
                }
                else
                {
                    AddSystemMessage("[INFO] voice.wav not found – audio greeting skipped.", Brushes.Yellow);
                }
            }
            catch (Exception ex)
            {
                AddSystemMessage($"[WARNING] Could not play voice: {ex.Message}", Brushes.Orange);
            }
        }

        private void LogoImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            // Optional: display a message the first time the image fails.
        }

        private void AddUserMessage(string text)
        {
            _messages.Add(new ChatMessage
            {
                Text = $"{_userMemory.Name ?? "Guest"}: {text}",
                Color = Brushes.Green,
                IsUser = true
            });
            _conversationHistory.Add(($"{_userMemory.Name ?? "Guest"}", text, DateTime.Now));
            ScrollToEnd();
        }

        private void AddBotMessage(string text)
        {
            _messages.Add(new ChatMessage
            {
                Text = $"Bot: {text}",
                Color = Brushes.Blue,
                IsUser = false
            });
            _conversationHistory.Add(("Bot", text, DateTime.Now));
            ScrollToEnd();
        }

        private void AddSystemMessage(string text, Brush color = null)
        {
            _messages.Add(new ChatMessage
            {
                Text = text,
                Color = color ?? Brushes.DarkGray,
                IsUser = false
            });
            _conversationHistory.Add(("System", text, DateTime.Now));
            ScrollToEnd();
        }

        private void ScrollToEnd()
        {
            if (ChatHistory.Items.Count > 0)
                ChatHistory.ScrollIntoView(ChatHistory.Items[ChatHistory.Items.Count - 1]);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) => ProcessInput();

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessInput();
                e.Handled = true;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            AddBotMessage($"Goodbye, {_userMemory.Name ?? "user"}. Stay safe online!");
            Close();
        }

        private void ShowChatHistory()
        {
            if (_conversationHistory.Count == 0)
            {
                AddBotMessage("No conversation history yet. Start chatting and I'll remember!");
                return;
            }

            AddBotMessage("Conversation History (last 10 messages)");
            int start = Math.Max(0, _conversationHistory.Count - 10);
            for (int i = start; i < _conversationHistory.Count; i++)
            {
                var entry = _conversationHistory[i];
                AddSystemMessage($"[{entry.Timestamp:HH:mm:ss}] {entry.Sender}: {entry.Message}", Brushes.DarkGoldenrod);
            }
            AddDivider();
        }

        private void LogActivity(string action)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string entry = $"[{timestamp}] {action}";
            activityLog.Add(entry);

            if (activityLog.Count > MaxLogEntries)
                activityLog.RemoveAt(0);
        }

        private void ShowActivityLog()
        {
            if (activityLog.Count == 0)
            {
                AddBotMessage("No activities have been recorded yet.");
                return;
            }

            AddBotMessage("Recent Activity Log:");
            int start = Math.Max(0, activityLog.Count - 10);

            for (int i = start; i < activityLog.Count; i++)
            {
                AddSystemMessage(activityLog[i], Brushes.DarkGoldenrod);
            }
            AddDivider();
        }

        private void OpenTaskManager()
        {
            var taskWindow = new TaskWindow(LogActivity);
            taskWindow.ShowDialog();
            AddBotMessage("Task manager closed. Continue with your cybersecurity questions.");
        }

        private void OpenQuiz()
        {
            var quizWindow = new QuizWindow(LogActivity);
            quizWindow.ShowDialog();
            AddBotMessage("Quiz session ended. Keep learning about cybersecurity.");
        }

        private void TaskButton_Click(object sender, RoutedEventArgs e)
        {
            LogActivity("Task manager opened");
            OpenTaskManager();
        }

        private void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            LogActivity("Quiz started");
            OpenQuiz();
        }

        private void ProcessInput()
        {
            string input = InputBox.Text?.Trim();
            InputBox.Clear();

            if (string.IsNullOrEmpty(input))
            {
                AddBotMessage("I didn't quite understand that. Could you rephrase?");
                return;
            }

            if (_currentMode == AppMode.AwaitingName)
            {
                _userMemory.Name = input;
                AddSystemMessage($"\nHello {_userMemory.Name}! I'm your Cybersecurity Awareness Assistant.", Brushes.Green);
                ShowMainMenu();
                _currentMode = AppMode.MainMenu;
                return;
            }

            AddUserMessage(input);
            LogActivity($"User input: {input}");

            if (_currentMode == AppMode.MainMenu)
            {
                if (input == "1")
                    EnterConversationMode();
                else if (input == "2")
                {
                    AddBotMessage($"Goodbye, {_userMemory.Name}. Stay safe online!");
                    Close();
                }
                else if (input == "3")
                {
                    ShowChatHistory();
                    ShowMainMenu();
                }
                else
                {
                    AddBotMessage("Invalid choice. Enter 1, 2, or 3.");
                    ShowMainMenu();
                }
                return;
            }

            if (_currentMode == AppMode.Conversation)
            {
                string lowerInput = input.ToLower();
                if (lowerInput == "done")
                {
                    AddBotMessage($"Thank you for chatting, {_userMemory.Name}. Stay safe!");
                    Close();
                }
                else if (lowerInput == "menu")
                {
                    AddBotMessage("Returning to main menu.");
                    ShowMainMenu();
                    _currentMode = AppMode.MainMenu;
                }
                else
                {
                    HandleConversationInput(input);
                }
            }
        }

        private void HandleConversationInput(string input)
        {
            string lowerInput = input.ToLower();

            string intent = _responseManager.ExtractIntent(lowerInput);

            switch (intent)
            {
                case "SHOW_LOG":
                    ShowActivityLog();
                    return;
                case "START_QUIZ":
                    OpenQuiz();
                    return;
                case "VIEW_TASKS":
                case "ADD_TASK":
                    OpenTaskManager();
                    return;
            }

            string response = "";
            bool negativeSentiment = false;
            bool keywordFound = false;

            string sentimentPrefix = _sentimentAnalyzer.DetectSentiment(lowerInput, out bool isNegative);
            if (isNegative)
            {
                negativeSentiment = true;
                response = sentimentPrefix;
            }
            else if (!string.IsNullOrEmpty(sentimentPrefix))
            {
                response = sentimentPrefix;
            }

            if (_responseManager.HasFollowUpIntent(lowerInput) && !string.IsNullOrEmpty(_lastTopic))
            {
                string followUp = _responseManager.GetRandomResponse(_lastTopic);
                if (followUp != null)
                {
                    response += $"Here's another tip about {_lastTopic}: {followUp}";
                    AddBotMessage(response);
                    AddDivider();
                    return;
                }
            }

            foreach (var topic in new[] { "password", "phishing", "scam", "privacy", "safe browsing" })
            {
                if (lowerInput.Contains("interested in " + topic) || lowerInput.Contains("i like " + topic))
                {
                    _userMemory.AddInterest(topic);
                    response = $"Great! I'll remember that you're interested in {topic}. ";
                    _logAction?.Invoke($"User interest stored: {topic}");
                    break;
                }
            }

            string detectedKeyword = _responseManager.DetectKeyword(lowerInput);
            if (detectedKeyword != null)
            {
                keywordFound = true;
                _lastTopic = detectedKeyword;
                string keywordResponse = _responseManager.GetRandomResponse(detectedKeyword);
                if (!string.IsNullOrEmpty(keywordResponse))
                {
                    response += keywordResponse;

                    string interestRecall = _userMemory.RecallInterests();
                    if (interestRecall != null && !lowerInput.Contains("interested in"))
                    {
                        response = interestRecall + " " + response;
                    }
                }
            }
            else if (lowerInput.Contains("how are you"))
            {
                response += $"I'm great, {_userMemory.Name}! Ready to help you stay safe online.";
            }
            else if (lowerInput.Contains("purpose"))
            {
                response += "My purpose is to educate you about cybersecurity – passwords, phishing, scams, privacy, and safe browsing.";
            }
            else if (lowerInput.Contains("what can i ask"))
            {
                response += "You can ask me about: password safety, phishing, scams, privacy, or safe browsing. Try 'another tip' after I answer!";
            }
            else
            {
                response += "I didn't quite understand that. Could you rephrase?";
            }

            if (negativeSentiment && !keywordFound && !response.Contains("tip"))
            {
                string generalTip = _generalTips[_random.Next(_generalTips.Length)];
                response += $" Let me share a helpful tip: {generalTip}";
            }

            AddBotMessage(response);
            AddDivider();
        }

        private void ShowMainMenu()
        {
            AddSystemMessage("\n╔════════════════════════════════════════╗");
            AddSystemMessage("║              MAIN MENU                 ║");
            AddSystemMessage("╠════════════════════════════════════════╣");
            AddSystemMessage("║  1. Ask a cybersecurity question       ║");
            AddSystemMessage("║  2. Exit                               ║");
            AddSystemMessage("║  3. View chat history                  ║");
            AddSystemMessage("╚════════════════════════════════════════╝");
            AddSystemMessage("\nEnter your choice (1, 2, or 3):", Brushes.Cyan);
        }

        private void EnterConversationMode()
        {
            AddBotMessage("You can ask me about passwords, phishing, scams, privacy, or safe browsing.\n" +
                         "Say 'another tip' after an answer to learn more.\n" +
                         "Tell me 'I'm interested in [topic]' and I'll remember!\n" +
                         "Type 'menu' to go back or 'done' to exit.");
            AddDivider();
            _currentMode = AppMode.Conversation;
        }

        private void AddDivider()
        {
            AddSystemMessage(new string('─', 60));
        }
    }
}