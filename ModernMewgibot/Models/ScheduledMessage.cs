using ModernMewgibot.Events;
using System;

namespace ModernMewgibot.Models
{
    class ScheduledMessage
    {
        //public int Interval { get; set; }
        //public string Message { get; set; }

        public IntervalMessage intMessage;

        public event EventHandler<ScheduledMessageEventArgs> OnScheduledMessage;

        internal System.Timers.Timer timer;

        public ScheduledMessage()
        {
            intMessage = new IntervalMessage()
            {
                Interval = 10,
                Message = "Chat Message"
            };

            timer = new System.Timers.Timer(intMessage.Interval * 60 * 1000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
        }

        public ScheduledMessage(int interval, string message)
        {
            intMessage = new IntervalMessage()
            {
                Interval = interval,
                Message = message
            };

            timer = new System.Timers.Timer(intMessage.Interval * 60 * 1000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
        }

        public ScheduledMessage(IntervalMessage message)
        {
            intMessage = message;

            timer = new System.Timers.Timer(intMessage.Interval * 60 * 1000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            OnScheduledMessage(this, new ScheduledMessageEventArgs { Message = intMessage.Message });
        }
    }

    static class ScheduledMessageExtensions
    {
        public static void StartTimer(this ScheduledMessage message)
        {
            message.timer.Stop();
            message.timer.Interval = message.intMessage.Interval * 60 * 1000;
            message.timer.Start();
        }
    }
}
