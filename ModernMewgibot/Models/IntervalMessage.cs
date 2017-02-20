using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernMewgibot.Models
{
    class IntervalMessage
    {
        public double Interval { get; set; }
        public string Message { get; set; }

        public IntervalMessage()
        {
            Interval = 10;
            Message = "Chat Message";
        }
    }
}
