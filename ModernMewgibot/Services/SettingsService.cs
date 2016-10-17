namespace ModernMewgibot.Services
{
    public static class SettingsService
    {
        private static Bot _settings
        {
            get
            {
                return Bot.Default;
            }
        }

        private readonly static string _username = _settings.Username;
        public static string Username
        {
            get
            {
                return _username;
            }
        }

        private readonly static string _oauth = _settings.OAuth;
        public static string OAuth
        {
            get
            {
                return _oauth;
            }
        }

        private readonly static string _clientID = _settings.ClientID;
        public static string ClientID
        {
            get
            {
                return _clientID;
            }
        }

        private static string _channel = _settings.Channel;
        public static string Channel
        {
            get
            {
                return _channel;
            }
            set
            {
                _channel = value;
                _settings.Channel = value;
                _settings.Save();
            }
        }

        private static bool _linkModEnabled = _settings.LinkModEnabled;
        public static bool LinkModEnabled
        {
            get
            {
                return _linkModEnabled;
            }
            set
            {
                _linkModEnabled = value;
                _settings.LinkModEnabled = value;
                _settings.Save();
            }
        }

        private static bool _purgeEnabled = _settings.PurgeEnabled;
        public static bool PurgeEnabled
        {
            get
            {
                return _purgeEnabled;
            }
            set
            {
                _purgeEnabled = value;
                _settings.PurgeEnabled = value;
                _settings.Save();
            }
        }

        private static bool _songEnabled = _settings.SongEnabled;
        public static bool SongEnabled
        {
            get
            {
                return _songEnabled;
            }
            set
            {
                _songEnabled = value;
                _settings.SongEnabled = value;
                _settings.Save();
            }
        }

        private static string _songFile = _settings.SongFile;
        public static string SongFile
        {
            get
            {
                return _songFile;
            }
            set
            {
                _songFile = value;
                _settings.SongFile = value;
                _settings.Save();
            }
        }
    }
}
