using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernMewgibot.Models
{
    class RaffleUser
    {
        public string Username { get; set; }
        public string Raffle { get; set; }
        public DateTimeOffset LastWin { get; set; }

        public override string ToString()
        {
            return $"{ Username } - { Raffle } - { LastWin }";
        }
    }
}
