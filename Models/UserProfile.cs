using System.Collections.Generic;

namespace CybersecurityChatbotWPF.Models
{
    public class UserProfile
    {
        public string UserName { get; set; }
        public string FavoriteTopic { get; set; }
        public List<string> Interests { get; set; }
        public int ConversationCount { get; set; }
        public List<string> ConversationHistory { get; set; }

        public UserProfile()
        {
            Interests = new List<string>();
            ConversationHistory = new List<string>();
            ConversationCount = 0;
            FavoriteTopic = "";
        }
    }
}