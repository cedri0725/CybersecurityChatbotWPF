using System;

namespace CybersecurityChatbotWPF.Models
{
    public class ChatbotResponse
    {
        public string UserQuestion { get; set; }
        public string BotAnswer { get; set; }
        public string Category { get; set; }
        public string Sentiment { get; set; }
        public DateTime Timestamp { get; set; }
        public string NewTopic { get; set; }

        public ChatbotResponse()
        {
            Timestamp = DateTime.Now;
        }

        public ChatbotResponse(string userQuestion, string botAnswer, string category)
        {
            UserQuestion = userQuestion;
            BotAnswer = botAnswer;
            Category = category;
            Timestamp = DateTime.Now;
            NewTopic = null;
        }
    }
}