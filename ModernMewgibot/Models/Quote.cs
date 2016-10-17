using System;

namespace ModernMewgibot.Models
{
    class Quote
    {
        public int QuoteID { get; set; }
        public string Message { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
