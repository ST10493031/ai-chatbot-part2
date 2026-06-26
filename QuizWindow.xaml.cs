using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace chatbot_ai
{
    public partial class QuizWindow : Window
    {
        private List<QuizQuestion> questions;
        private int currentIndex;
        private int score;
        private Action<string> logCallback;

        public QuizWindow(Action<string> logCallback)
        {
            InitializeComponent();
            this.logCallback = logCallback;
            InitializeQuestions();
            currentIndex = 0;
            score = 0;
            DisplayQuestion();
            logCallback?.Invoke("Quiz session started");
        }

        private void InitializeQuestions()
        {
            questions = new List<QuizQuestion>
            {
                new QuizQuestion(
                    "What should you do if you receive an email requesting your password?",
                    new List<string>
                    {
                        "Reply with your password",
                        "Delete the email",
                        "Report the email as phishing",
                        "Forward it to a colleague"
                    },
                    2,
                    "Phishing emails should be reported, not responded to. Legitimate organisations never ask for passwords via email."
                ),
                new QuizQuestion(
                    "Which of the following is considered a strong password?",
                    new List<string>
                    {
                        "Password123",
                        "Your birthdate",
                        "Correct-Horse-Battery-Staple",
                        "Same password for all accounts"
                    },
                    2,
                    "A strong password uses a mix of characters and is easy to remember but hard to guess."
                ),
                new QuizQuestion(
                    "What is the primary purpose of phishing attacks?",
                    new List<string>
                    {
                        "To steal personal information",
                        "To spread computer viruses",
                        "To test network security",
                        "To send spam emails"
                    },
                    0,
                    "Phishing attacks aim to deceive users into providing sensitive information."
                ),
                new QuizQuestion(
                    "What does two-factor authentication provide?",
                    new List<string>
                    {
                        "A second password",
                        "An additional layer of security",
                        "Faster login times",
                        "Access to multiple accounts"
                    },
                    1,
                    "Two-factor authentication requires two forms of verification, significantly reducing the risk of unauthorised access."
                ),
                new QuizQuestion(
                    "Which of these indicates a website is secure for entering personal information?",
                    new List<string>
                    {
                        "The website has a professional design",
                        "The URL begins with 'https://'",
                        "The website loads quickly",
                        "The website has a search function"
                    },
                    1,
                    "The 'https://' protocol and padlock icon indicate the connection is encrypted."
                ),
                new QuizQuestion(
                    "What is social engineering in cybersecurity?",
                    new List<string>
                    {
                        "A type of software testing",
                        "Manipulating people to divulge confidential information",
                        "A networking protocol",
                        "An encryption method"
                    },
                    1,
                    "Social engineering exploits human psychology rather than technical vulnerabilities."
                ),
                new QuizQuestion(
                    "What is the safest action when you receive a suspicious link?",
                    new List<string>
                    {
                        "Click it to check what it is",
                        "Forward it to friends",
                        "Avoid clicking and report it",
                        "Copy and paste it into your browser"
                    },
                    2,
                    "Never click suspicious links. Report them to the appropriate authorities."
                ),
                new QuizQuestion(
                    "Why should you avoid using public Wi-Fi for sensitive transactions?",
                    new List<string>
                    {
                        "It is too slow",
                        "It uses too much data",
                        "It is vulnerable to eavesdropping",
                        "It requires a password"
                    },
                    2,
                    "Public Wi-Fi networks are often unencrypted, making it easy for attackers to intercept your data."
                ),
                new QuizQuestion(
                    "What is the recommended frequency for password changes?",
                    new List<string>
                    {
                        "Never change them",
                        "Every 3-6 months or after a breach",
                        "Only when you forget them",
                        "Every week"
                    },
                    1,
                    "Regular password changes, and immediately after any security breach, is considered good practice."
                ),
                new QuizQuestion(
                    "What is a common sign of a scam email?",
                    new List<string>
                    {
                        "Professional layout",
                        "Urgent language requesting immediate action",
                        "Correct grammar and spelling",
                        "Sender address you recognise"
                    },
                    1,
                    "Scammers often use urgent language and threats to pressure recipients into acting without thinking."
                ),
                new QuizQuestion(
                    "What should you do if your account has been compromised?",
                    new List<string>
                    {
                        "Wait to see what happens",
                        "Change your password immediately and enable two-factor authentication",
                        "Post about it on social media",
                        "Ignore it and continue using the account"
                    },
                    1,
                    "Immediate action is critical. Change passwords, enable two-factor authentication, and contact the service provider."
                ),
                new QuizQuestion(
                    "Which of the following is a secure browsing practice?",
                    new List<string>
                    {
                        "Clicking all pop-up ads",
                        "Using the same password everywhere",
                        "Checking for HTTPS before entering data",
                        "Ignoring security certificate warnings"
                    },
                    2,
                    "Always verify the connection is secure before entering sensitive information, and never ignore security warnings."
                )
            };
        }

        private void DisplayQuestion()
        {
            if (currentIndex >= questions.Count)
            {
                EndQuiz();
                return;
            }

            QuizQuestion q = questions[currentIndex];
            QuestionDisplay.Text = q.Question;
            ProgressDisplay.Text = $"Question {currentIndex + 1} of {questions.Count}";
            ScoreDisplay.Text = $"Score: {score} / {currentIndex}";

            OptionsPanel.Children.Clear();

            for (int i = 0; i < q.Options.Count; i++)
            {
                RadioButton option = new RadioButton
                {
                    Content = q.Options[i],
                    Tag = i,
                    FontSize = 14,
                    Margin = new Thickness(5, 3, 0, 3),
                    GroupName = "QuestionOptions"
                };
                OptionsPanel.Children.Add(option);
            }

            SubmitButton.IsEnabled = true;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex >= questions.Count)
                return;

            int selectedIndex = -1;

            foreach (RadioButton rb in OptionsPanel.Children)
            {
                if (rb.IsChecked == true)
                {
                    selectedIndex = (int)rb.Tag;
                    break;
                }
            }

            if (selectedIndex == -1)
            {
                MessageBox.Show("Please select an answer before submitting.", "Selection Required",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            QuizQuestion currentQuestion = questions[currentIndex];
            bool isCorrect = selectedIndex == currentQuestion.CorrectIndex;

            if (isCorrect)
            {
                score++;
                MessageBox.Show($"Correct. {currentQuestion.Feedback}", "Correct Answer",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string correctOption = currentQuestion.Options[currentQuestion.CorrectIndex];
                MessageBox.Show($"Incorrect. The correct answer was: {correctOption}\n\n{currentQuestion.Feedback}",
                    "Incorrect Answer", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            currentIndex++;
            DisplayQuestion();

            if (currentIndex >= questions.Count)
            {
                EndQuiz();
            }
        }

        private void EndQuiz()
        {
            SubmitButton.IsEnabled = false;
            QuestionDisplay.Text = "Quiz Complete!";
            OptionsPanel.Children.Clear();

            string feedback;
            if (score >= 10)
                feedback = "Excellent work. You demonstrate strong cybersecurity awareness.";
            else if (score >= 7)
                feedback = "Good effort. Continue learning to strengthen your cybersecurity knowledge.";
            else if (score >= 5)
                feedback = "Review the cybersecurity topics covered and try again for better results.";
            else
                feedback = "Consider revisiting basic cybersecurity concepts. Practice is key to improvement.";

            MessageBox.Show($"Quiz Complete. Final Score: {score} out of {questions.Count}\n\n{feedback}",
                "Quiz Results", MessageBoxButton.OK, MessageBoxImage.Information);

            logCallback?.Invoke($"Quiz completed. Score: {score}/{questions.Count}");
            Close();
        }
    }

    public class QuizQuestion
    {
        public string Question { get; }
        public List<string> Options { get; }
        public int CorrectIndex { get; }
        public string Feedback { get; }

        public QuizQuestion(string question, List<string> options, int correctIndex, string feedback)
        {
            Question = question;
            Options = options;
            CorrectIndex = correctIndex;
            Feedback = feedback;
        }
    }
}