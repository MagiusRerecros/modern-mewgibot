using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernMewgibot.Models
{
    class BotSettings
    {
        public string Username { get; set; }
        public string OAuth { get; set; }
        public string Channel { get; set; }
        public string ClientID { get; set; }
        public string ChannelAccessToken { get; set; }
        public bool LinkModEnabled { get; set; }
        public bool PurgeEnabled { get; set; }
        public bool SongEnabled { get; set; }
        public string SongFile { get; set; }
        public bool FollowGreetingEnabled { get; set; }
        public bool SubGreetingEnabled { get; set; }
        public string FollowGreeting { get; set; }
        public string SubGreeting { get; set; }
        public bool HostAutoThank { get; set; }
        public bool SubsCanLink { get; set; }
        public bool RegularsCanLink { get; set; }
        public bool QuotesEnabled { get; set; }
    }
}
