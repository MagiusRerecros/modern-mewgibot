using System;

namespace ModernMewgibot.Models
{
    public enum ChatLevel
    {
        None = 0,
        Moderator = 1,
        Broadcaster = 2
    }

    class BotCommand
    {
        private string _trigger;
        public string Trigger
        {
            get { return _trigger; }
            set
            {
                if (value.StartsWith("!"))
                    _trigger = value.Substring(1);
                else
                    _trigger = value;
            }
        }

        public bool Enabled { get; set; }
        public string Response { get; set; }
        public ChatLevel AccessLevel { get; set; }
        public DateTimeOffset LastUsed { get; set; }
        public int Interval { get; set; }
    }
}
