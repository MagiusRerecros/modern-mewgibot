using System;

namespace ModernMewgibot.Models
{
    class BotCommand
    {
        public string Trigger { get; set; }
        public string Response { get; set; }
        public ChatLevel AccessLevel { get; set; }
        public enum ChatLevel
        {
            None = 0,
            Moderator = 1,
            Broadcaster =2
        }
    }
}
