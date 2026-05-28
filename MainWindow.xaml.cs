using CybersecurityChatbotWPF.Services;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace CybersecurityChatbotWPF
{
    public partial class MainWindow : Window
    {
        private ChatbotService _chatbotService;
        private SentimentService _sentimentService;
        private MemoryService _memoryService;
        private AudioService _audioService;
        private string _currentTopic = "";
        private bool _voiceEnabled = true;  // Voice ON by default

        public MainWindow()
        {
            InitializeComponent();
            InitializeServices();
            DisplayAsciiArt();
            PlayWelcomeGreeting();
            AddBotMessage("Hello! 👋 I'm your Cybersecurity Awareness Assistant.", "#00FFAA");
            AddBotMessage("I can help you with passwords, scams, privacy, and phishing.", "#00FFAA");

            // Ask how the user is feeling (proactive)
            AddBotMessage("Before we start, how are you feeling today? 😊", "#88FF88");
        }

        private void InitializeServices()
        {
            _chatbotService = new ChatbotService();
            _sentimentService = new SentimentService();
            _memoryService = new MemoryService();
            _audioService = new AudioService();
        }

        private void DisplayAsciiArt()
        {
            string ascii = """

                ╔══════════════════════════════════════════════════════════════════════════════════════════╗
                ║                                                                                          ║
                ║     ██████╗██╗   ██╗██████╗ ███████╗██████╗ ███████╗ ██████╗██╗   ██╗██████╗ ██╗████████╗║
                ║    ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗██╔════╝██╔════╝██║   ██║██╔══██╗██║╚══██╔══╝║
                ║    ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝███████╗██║     ██║   ██║██████╔╝██║   ██║   ║
                ║    ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗╚════██║██║     ██║   ██║██╔══██╗██║   ██║   ║
                ║    ╚██████╗   ██║   ██████╔╝███████╗██║  ██║███████║╚██████╗████████║██║  ██║██║   ██║   ║
                ║     ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝╚══════╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝╚═╝   ╚═╝   ║
                ║                                                                                          ║
                ║           🛡️  CYBERSECURITY AWARENESS CHATBOT: PROTECTING SOUTH AFRICAN CITIZENS  🛡️    ║
                ║                              🇿🇦  Stay Safe Online - Think Before You Click!  🇿🇦           ║
                ║                                                                                          ║
                ╚══════════════════════════════════════════════════════════════════════════════════════════╝
                """;

            AsciiArt.Text = ascii;
        }

        private void PlayWelcomeGreeting()
        {
            if (_voiceEnabled)
            {
                string welcomeMessage = "Hello! Welcome to the Cybersecurity Awareness Bot. I'm here to help you stay safe online.";
                _audioService.SpeakAsync(welcomeMessage);
            }
        }

        private void AddBotMessage(string message, string colorHex = "#00FFAA")
        {
            try
            {
                Run run = new Run(message);
                run.Foreground = (Brush)new BrushConverter().ConvertFromString(colorHex);

                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run("🤖 Bot: ") { Foreground = Brushes.LightGreen, FontWeight = FontWeights.Bold });
                paragraph.Inlines.Add(run);

                rtbChatDisplay.Document.Blocks.Add(paragraph);
                ScrollToBottom();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding bot message: {ex.Message}");
            }
        }

        private void AddUserMessage(string message)
        {
            try
            {
                string userName = _memoryService.GetUserName();
                string displayName = string.IsNullOrEmpty(userName) ? "You" : userName;

                Run run = new Run(message);
                run.Foreground = Brushes.Cyan;

                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run($"{displayName}: ") { Foreground = Brushes.Yellow, FontWeight = FontWeights.Bold });
                paragraph.Inlines.Add(run);

                rtbChatDisplay.Document.Blocks.Add(paragraph);
                ScrollToBottom();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding user message: {ex.Message}");
            }
        }

        private void ScrollToBottom()
        {
            try
            {
                scrollViewer.ScrollToBottom();
            }
            catch (Exception) { }
        }

        private void BtnSaveName_Click(object sender, RoutedEventArgs e)
        {
            string name = txtUserName.Text.Trim();
            if (!string.IsNullOrEmpty(name) && name.Length >= 2)
            {
                _memoryService.SaveUserName(name);

                // Show text message
                AddBotMessage($"Nice to meet you, {name}! 😊 I'll remember that. What cybersecurity topic interests you?", "#00FFAA");

                // Speak the greeting with the user's name (if voice is enabled)
                if (_voiceEnabled)
                {
                    string spokenGreeting = $"Nice to meet you, {name}! I'll remember that. What cybersecurity topic interests you?";
                    _audioService.SpeakAsync(spokenGreeting);
                }

                lblMemory.Text = $"💾 Remembering: {name}";
                txtUserName.Text = "";
            }
            else
            {
                AddBotMessage("Please enter a valid name (at least 2 characters).", "#FF6666");
                if (_voiceEnabled)
                {
                    _audioService.SpeakAsync("Please enter a valid name with at least 2 characters.");
                }
            }
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserInput();
        }

        private void TxtUserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessUserInput();
            }
        }

        private void BtnVoice_Click(object sender, RoutedEventArgs e)
        {
            if (!_voiceEnabled)
            {
                AddBotMessage("🔇 Voice is currently OFF. Turn it ON using the toggle button.", "#FFAA00");
                return;
            }

            string userName = _memoryService.GetUserName();
            string welcomeMessage;

            if (!string.IsNullOrEmpty(userName))
            {
                welcomeMessage = $"Hello {userName}! Welcome back to the Cybersecurity Awareness Bot. I'm here to help you stay safe online.";
            }
            else
            {
                welcomeMessage = "Hello! Welcome to the Cybersecurity Awareness Bot. I'm here to help you stay safe online.";
            }

            _audioService.SpeakAsync(welcomeMessage);
            AddBotMessage("🔊 Playing welcome message...", "#888888");
        }

        // Voice ON/OFF Toggle
        private void BtnVoiceOnOff_Checked(object sender, RoutedEventArgs e)
        {
            _voiceEnabled = true;
            btnVoiceOnOff.Content = "🔊 ON";
            btnVoiceOnOff.Background = (Brush)new BrushConverter().ConvertFromString("#00FFAA");
            AddBotMessage("🔊 Voice responses ENABLED. I will speak my answers.", "#00FFAA");
        }

        private void BtnVoiceOnOff_Unchecked(object sender, RoutedEventArgs e)
        {
            _voiceEnabled = false;
            btnVoiceOnOff.Content = "🔇 OFF";
            btnVoiceOnOff.Background = (Brush)new BrushConverter().ConvertFromString("#FF4444");
            _audioService.StopSpeaking();  // Stop any ongoing speech
            AddBotMessage("🔇 Voice responses DISABLED. I will only show text.", "#FFAA00");
        }

        // Stop current speech
        private void BtnStopVoice_Click(object sender, RoutedEventArgs e)
        {
            _audioService.StopSpeaking();
            AddBotMessage("⏹️ Speech stopped.", "#888888");
        }

        // Exit/Chat the chatbot
        private void BtnExitChat_Click(object sender, RoutedEventArgs e)
        {
            string userName = _memoryService.GetUserName();
            string farewellMessage = string.IsNullOrEmpty(userName)
                ? "Goodbye! Stay safe online! 🔒"
                : $"Goodbye {userName}! Stay safe online! 🔒";

            AddBotMessage(farewellMessage, "#00FFAA");

            if (_voiceEnabled)
            {
                _audioService.SpeakAsync(farewellMessage);
                // Wait a moment for speech to start before closing
                Task.Delay(1500).ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() => Application.Current.Shutdown());
                });
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private async void ProcessUserInput()
        {
            string userInput = txtUserInput.Text.Trim();

            // Check for exit command
            if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                userInput.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
                userInput.Equals("close", StringComparison.OrdinalIgnoreCase))
            {
                BtnExitChat_Click(null, null);
                return;
            }

            if (string.IsNullOrEmpty(userInput))
            {
                AddBotMessage("Please type a message inside. I'm here to help you stay safe online!", "#FFAA00");
                if (_voiceEnabled)
                {
                    _audioService.SpeakAsync("Please type a message. I'm here to help you stay safe online.");
                }
                return;
            }

            AddUserMessage(userInput);
            txtUserInput.Text = "";

            lblStatus.Text = "🤔 Bot is thinking...";
            lblStatus.Foreground = Brushes.Yellow;

            string sentiment = _sentimentService.DetectSentiment(userInput);
            UpdateSentimentDisplay(sentiment);

            _memoryService.StoreInformation(userInput, _currentTopic);
            UpdateMemoryDisplay();

            var response = _chatbotService.GetResponse(userInput, sentiment, _currentTopic);

            if (!string.IsNullOrEmpty(response.NewTopic))
            {
                _currentTopic = response.NewTopic;
                UpdateTopicDisplay(_currentTopic);
            }

            await Task.Delay(500);

            string responseColor = response.Category == "unknown" ? "#FFAA00" : "#00FFAA";
            AddBotMessage(response.BotAnswer, responseColor);

            // Speak the response ONLY if voice is enabled
            if (_voiceEnabled)
            {
                _audioService.SpeakResponse(response.BotAnswer);
            }

            lblStatus.Text = "✅ Ready";
            lblStatus.Foreground = Brushes.LightGreen;
        }

        private void UpdateSentimentDisplay(string sentiment)
        {
            string sentimentIcon = sentiment switch
            {
                "worried" => "😟",
                "curious" => "🤔",
                "frustrated" => "😤",
                _ => "😊"
            };
            string displaySentiment = char.ToUpper(sentiment[0]) + sentiment.Substring(1);
            lblSentiment.Text = $"{sentimentIcon} Sentiment: {displaySentiment}";
        }

        private void UpdateMemoryDisplay()
        {
            lblMemoryStatus.Text = _memoryService.GetMemorySummary();
        }

        private void UpdateTopicDisplay(string topic)
        {
            if (!string.IsNullOrEmpty(topic))
            {
                lblTopic.Text = $"📚 Current topic: {topic}";
                lblTopic.Visibility = Visibility.Visible;
            }
            else
            {
                lblTopic.Visibility = Visibility.Collapsed;
            }
        }
    }
}