using System;
using System.Collections.Generic;
using CybersecurityChatbotWPF.Models;

namespace CybersecurityChatbotWPF.Services
{
    public class ChatbotService
    {
        private Dictionary<string, List<string>> _keywordResponses;
        private Dictionary<string, List<string>> _randomResponses;
        private Random _random;

        public ChatbotService()
        {
            _random = new Random();
            InitializeKeywordResponses();
            InitializeRandomResponses();
        }

        private void InitializeKeywordResponses()
        {
            _keywordResponses = new Dictionary<string, List<string>>()
            {
                ["password"] = new List<string>
                {
                    "🔐 Use strong passwords with at least 12 characters including uppercase, lowercase, numbers, and symbols!",
                    "🔐 Never reuse passwords across different accounts - each account needs its own unique password!",
                    "🔐 Enable Two-Factor Authentication (2FA) whenever possible for an extra layer of security!",
                    "🔐 Consider using a password manager like Bitwarden or LastPass to generate and store complex passwords!"
                },
                ["scam"] = new List<string>
                {
                    "🎣 Never click on links in suspicious emails! Always hover over links to see the real URL first.",
                    "🎣 Legitimate companies never ask for your password via email. When in doubt, contact them directly!",
                    "🎣 Check for spelling errors, urgent language like 'Act Now!', and suspicious sender addresses!",
                    "🎣 If an offer seems too good to be true, it probably is! Don't fall for prize scams or lottery wins."
                },
                ["privacy"] = new List<string>
                {
                    "🔒 Review your privacy settings on social media regularly. Limit what personal information is public.",
                    "🔒 Be careful what you share online - once something is posted, it's difficult to remove completely.",
                    "🔒 Use a VPN when using public Wi-Fi to encrypt your internet traffic and protect your privacy!",
                    "🔒 Regularly check which apps have access to your data and remove ones you don't use anymore."
                },
                ["phishing"] = new List<string>
                {
                    "🎣 Phishing emails often create a sense of urgency. Take a moment to verify before clicking anything!",
                    "🎣 Look for poor grammar, generic greetings like 'Dear Customer', and mismatched email addresses.",
                    "🎣 When in doubt about an email, contact the organization directly using their official website, not links in the email.",
                    "🎣 Never enter personal information on a site you reached from an email link. Type the URL directly instead."
                },
                ["help"] = new List<string>
                {
                    "I can help you with:\n• Password safety\n• Recognizing scams and phishing\n• Privacy protection\n• Safe browsing habits\n\nJust ask me about any of these topics!",
                    "Try asking me:\n- 'Tell me about password safety'\n- 'How to spot a scam?'\n- 'Privacy tips'\n- 'What is phishing?'\n- 'How are you?'"
                },
                ["feeling"] = new List<string>
                {
                    "I'm doing great, thank you for asking! 😊 How can I help you stay safe online today?",
                    "I'm functioning perfectly! Thanks for checking in. What cybersecurity topic interests you?",
                    "All systems operational! I'm ready to help you learn about online safety. How are you feeling today?",
                    "I'm excellent! A bit of code and coffee keeps me going. What would you like to know about cybersecurity?"
                },
                ["ask_feeling"] = new List<string>
                {
                    "How are you feeling today? I'm here to listen and help with any cybersecurity concerns you might have.",
                    "Before we dive into cybersecurity topics, how are you doing today?",
                    "How's your day going? Remember, staying safe online starts with a mindful mindset!",
                    "How are you feeling? I can tailor my cybersecurity tips to match your mood."
                },
                ["empathetic"] = new List<string>
                {
                    "I understand how you feel. Cybersecurity can be overwhelming, but you're taking the right step by learning about it!",
                    "It's completely normal to feel that way. Many people share your concerns about online safety.",
                    "Thank you for sharing that. Remember, you're not alone in this - millions of people face similar challenges online.",
                    "I appreciate you being open about your feelings. Let me help make cybersecurity less intimidating for you."
                }
            };
        }

        private void InitializeRandomResponses()
        {
            _randomResponses = new Dictionary<string, List<string>>()
            {
                ["greeting"] = new List<string>
                {
                    "Hi there! How can I help you stay safe online today?",
                    "Hello! Ready to learn about cybersecurity?",
                    "Greetings! I'm your cybersecurity assistant. What would you like to know?"
                },
                ["thanks"] = new List<string>
                {
                    "You're welcome! 😊 Stay safe online!",
                    "My pleasure! Cybersecurity is everyone's responsibility.",
                    "Glad I could help! Remember to always think before you click!"
                }
            };
        }

        public ChatbotResponse GetResponse(string userInput, string sentiment, string currentTopic)
        {
            string lowerInput = userInput.ToLower();
            string responseText = "";
            string category = "unknown";
            string newTopic = null;

            // Handle how are you / feeling questions
            if (ContainsKeyword(lowerInput, new[] { "how are you", "how are you doing", "how's it going", "how do you feel" }))
            {
                category = "feeling";
                responseText = GetRandomResponse(_keywordResponses["feeling"]);
            }
            // Handle user sharing their feelings
            else if (ContainsKeyword(lowerInput, new[] { "i feel", "i'm feeling", "feeling", "i am feeling" }))
            {
                category = "empathetic";
                responseText = GetRandomResponse(_keywordResponses["empathetic"]);
                responseText += " Would you like to learn about a specific cybersecurity topic to feel more confident?";
            }
            // Handle positive responses
            else if (ContainsKeyword(lowerInput, new[] { "good", "great", "wonderful", "excellent", "happy" }))
            {
                category = "positive";
                responseText = "That's wonderful to hear! 😊 A positive mindset is great for learning. What cybersecurity topic interests you today?";
            }
            // Handle negative responses
            else if (ContainsKeyword(lowerInput, new[] { "bad", "sad", "tired", "stressed", "anxious", "worried", "not good" }))
            {
                category = "negative";
                responseText = "I'm sorry you're feeling that way. 💙 Cybersecurity can feel overwhelming, but I'm here to help make it simple. Would you like some easy tips to feel more secure online?";
            }
            // Handle follow-up questions
            else if (IsFollowUp(lowerInput) && !string.IsNullOrEmpty(currentTopic))
            {
                category = currentTopic;
                responseText = GetResponseForTopic(currentTopic);
                newTopic = currentTopic;
            }
            else if (ContainsKeyword(lowerInput, new[] { "thank", "thanks", "appreciate" }))
            {
                category = "thanks";
                responseText = GetRandomResponse(_randomResponses["thanks"]);
            }
            else if (ContainsKeyword(lowerInput, new[] { "password", "passphrase", "login", "credentials" }))
            {
                category = "password";
                responseText = GetRandomResponse(_keywordResponses["password"]);
                newTopic = "password";
            }
            else if (ContainsKeyword(lowerInput, new[] { "scam", "fraud", "fake" }))
            {
                category = "scam";
                responseText = GetRandomResponse(_keywordResponses["scam"]);
                newTopic = "scam";
            }
            else if (ContainsKeyword(lowerInput, new[] { "privacy", "private", "personal information" }))
            {
                category = "privacy";
                responseText = GetRandomResponse(_keywordResponses["privacy"]);
                newTopic = "privacy";
            }
            else if (ContainsKeyword(lowerInput, new[] { "phish", "phishing" }))
            {
                category = "phishing";
                responseText = GetRandomResponse(_keywordResponses["phishing"]);
                newTopic = "phishing";
            }
            else if (ContainsKeyword(lowerInput, new[] { "help", "what can you do", "options" }))
            {
                category = "help";
                responseText = GetRandomResponse(_keywordResponses["help"]);
            }
            else if (ContainsKeyword(lowerInput, new[] { "hello", "hi", "hey" }))
            {
                category = "greeting";
                responseText = GetRandomResponse(_randomResponses["greeting"]);
                // Also ask how they're feeling
                responseText += " " + GetRandomResponse(_keywordResponses["ask_feeling"]);
            }
            else
            {
                responseText = GetDefaultResponse();
            }

            // Adjust response based on sentiment
            responseText = AdjustResponseForSentiment(responseText, sentiment);

            return new ChatbotResponse(userInput, responseText, category) { NewTopic = newTopic };
        }

        private bool IsFollowUp(string input)
        {
            string[] keywords = { "more", "another", "tell me more", "explain more", "continue", "elaborate" };
            return ContainsKeyword(input, keywords);
        }

        private string GetResponseForTopic(string topic)
        {
            if (_keywordResponses.ContainsKey(topic))
                return GetRandomResponse(_keywordResponses[topic]);
            return GetDefaultResponse();
        }

        private bool ContainsKeyword(string input, string[] keywords)
        {
            foreach (string keyword in keywords)
                if (input.Contains(keyword)) return true;
            return false;
        }

        private string GetRandomResponse(List<string> responses)
        {
            return responses[_random.Next(responses.Count)];
        }

        private string AdjustResponseForSentiment(string response, string sentiment)
        {
            if (sentiment == "worried")
                return "I understand your concern. " + response + " Remember, being aware is the first step to staying safe! 💙";
            if (sentiment == "frustrated")
                return "I hear you. Let me help simplify this. " + response;
            if (sentiment == "curious")
                return "That's a great question to ask! " + response;
            return response;
        }

        private string GetDefaultResponse()
        {
            string[] defaults = {
                "I'm not sure I understand. Try asking about passwords, scams, privacy, or phishing!",
                "Hmm, I didn't catch that. You can ask me about password safety, spotting scams, or privacy protection!",
                "Try asking: 'Tell me about password safety', 'How to spot a scam?', 'Privacy tips', or 'How are you?'"
            };
            return defaults[_random.Next(defaults.Length)];
        }
    }
}