using ModernMewgibot.Models;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ModernMewgibot.Services
{
    public static class SettingsService
    {
#pragma warning disable IDE1006 // Naming Styles
        private static Bot _settings
#pragma warning restore IDE1006 // Naming Styles
        {
            get
            {
                return Bot.Default;
            }
        }

#pragma warning disable IDE1006 // Naming Styles
        private static UserInterface _uiSettings
#pragma warning restore IDE1006 // Naming Styles
        {
            get
            {
                return UserInterface.Default;
            }
        }

        internal static void Export()
        {
            BotSettings botSettings = new BotSettings
            {
                Username = _settings.Username,
                OAuth = _settings.OAuth,
                Channel = _settings.Channel,
                ClientID = _settings.ClientID,
                ChannelAccessToken = _settings.ChannelAccessToken,
                LinkModEnabled = _settings.LinkModEnabled,
                PurgeEnabled = _settings.PurgeEnabled,
                SongEnabled = _settings.SongEnabled,
                SongFile = _settings.SongFile,
                FollowGreetingEnabled = _settings.FollowGreetingEnabled,
                SubGreetingEnabled = _settings.SubGreetingEnabled,
                FollowGreeting = _settings.FollowGreeting,
                SubGreeting = _settings.SubGreeting,
                HostAutoThank = _settings.HostAutoThank,
                SubsCanLink = _settings.SubsCanLink,
                RegularsCanLink = _settings.RegularsCanLink,
                QuotesEnabled = _settings.QuotesEnabled
            };

            UISettings uiSettings = new UISettings
            {
                SelectedAccentColor = _uiSettings.SelectedAccentColor,
                SelectedThemeSource = _uiSettings.SelectedThemeSource,
                SelectedThemeDisplayName = _uiSettings.SelectedThemeDisplayName,
                SelectedFontSize = _uiSettings.SelectedFontSize
            };

            Task.Factory.StartNew(async () => await JsonFileService.SaveToFileAsync(botSettings, @".\Settings\botSettings.json"));
            //Task.Factory.StartNew(async () => await JsonFileService.SaveToFileAsync(uiSettings, @".\Settings\uiSettings.json"));
        }

        internal async static Task Import()
        {
            BotSettings botSettings = await JsonFileService.LoadFromFileAsync<BotSettings>(@".\Settings\botSettings.json");
            //UISettings uiSettings = await JsonFileService.LoadFromFileAsync<UISettings>(@".\Settings\uiSettings.json");

            //_uiSettings.SelectedAccentColor = uiSettings.SelectedAccentColor;
            //_uiSettings.SelectedThemeSource = uiSettings.SelectedThemeSource;
            //_uiSettings.SelectedThemeDisplayName = uiSettings.SelectedThemeDisplayName;
            //_uiSettings.SelectedFontSize = uiSettings.SelectedFontSize;
            //_uiSettings.Save();
            //_uiSettings.Reload();

            _settings.Username = botSettings.Username;
            _settings.OAuth = botSettings.OAuth;
            _settings.ChannelAccessToken = botSettings.ChannelAccessToken;
            _settings.Channel = botSettings.Channel;
            _settings.ClientID = botSettings.ClientID;
            _settings.LinkModEnabled = botSettings.LinkModEnabled;
            _settings.PurgeEnabled = botSettings.PurgeEnabled;
            _settings.SongEnabled = botSettings.SongEnabled;
            _settings.SongFile = botSettings.SongFile;
            _settings.FollowGreetingEnabled = botSettings.FollowGreetingEnabled;
            _settings.SubGreetingEnabled = botSettings.SubGreetingEnabled;
            _settings.FollowGreeting = botSettings.FollowGreeting;
            _settings.SubGreeting = botSettings.SubGreeting;
            _settings.HostAutoThank = botSettings.HostAutoThank;
            _settings.SubsCanLink = botSettings.SubsCanLink;
            _settings.RegularsCanLink = botSettings.RegularsCanLink;
            _settings.QuotesEnabled = botSettings.QuotesEnabled;
            _settings.Save();
            _settings.Reload();

            _username = _settings.Username;
            _oauth = _settings.OAuth;
            _channelAccessToken = _settings.ChannelAccessToken;
            _channel = _settings.Channel;
            _linkModEnabled = _settings.LinkModEnabled;
            _purgeEnabled = _settings.PurgeEnabled;
            _songEnabled = _settings.SongEnabled;
            _songFile = _settings.SongFile;
            _followGreetingEnabled = _settings.FollowGreetingEnabled;
            _subGreetingEnabled = _settings.SubGreetingEnabled;
            _followGreeting = _settings.FollowGreeting;
            _subGreeting = _settings.SubGreeting;
            _hostAutoThank = _settings.HostAutoThank;
            _subsCanLink = _settings.SubsCanLink;
            _regularsCanLink = _settings.RegularsCanLink;
            _quotesEnabled = _settings.QuotesEnabled;
        }

        //internal static void Import(string filename)
        //{
        //    string baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MewgiBot\\Settings\\";

        //    if (!File.Exists(baseDirectory + filename))
        //        return;

        //    try
        //    {
        //        var import = XDocument.Load(baseDirectory + filename);
        //        var settings = import.XPathSelectElements("//setting");
        //        foreach (var setting in settings)
        //        {
        //            string name = setting.Attribute("name").Value;
        //            string value = setting.XPathSelectElement("value").FirstNode.ToString();

        //            try
        //            {
        //                _settings[name] = value;
        //            }
        //            catch (SettingsPropertyNotFoundException ex)
        //            {
        //                Console.Error.WriteLine($"An imported setting ({name}) did not match an existing setting. {ex.Message}");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Error.WriteLine($"Could not import settings: {ex.Message}");
        //        _settings.Reload();
        //    }
        //}

        private static string _username = _settings.Username;
        public static string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                _settings.Username = _username;
                _settings.Save();
            }
        }

        private static string _oauth = _settings.OAuth;
        public static string OAuth
        {
            get
            {
                return EncryptionService.ToInsecureString(EncryptionService.DecryptString(_oauth));
            }
            set
            {
                _oauth = EncryptionService.EncryptString(EncryptionService.ToSecureString(value));
                _settings.OAuth = _oauth;
                _settings.Save();
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

        private static string _channelAccessToken = _settings.ChannelAccessToken;
        public static string ChannelAccessToken
        {
            get { return EncryptionService.ToInsecureString(EncryptionService.DecryptString(_channelAccessToken)); }
            set
            {
                _channelAccessToken = EncryptionService.EncryptString(EncryptionService.ToSecureString(value));
                _settings.ChannelAccessToken = _channelAccessToken;
                _settings.Save();
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

        private static bool _hostAutoThank = _settings.HostAutoThank;
        public static bool HostAutoThank
        {
            get { return _hostAutoThank; }
            set
            {
                _hostAutoThank = value;
                _settings.HostAutoThank = value;
                _settings.Save();
            }
        }

        private static bool _linkModEnabled = _settings.LinkModEnabled;
        public static bool LinkModEnabled
        {
            get { return _linkModEnabled; }
            set
            {
                _linkModEnabled = value;
                _settings.LinkModEnabled = value;
                _settings.Save();
            }
        }

        private static bool _subsCanLink = _settings.SubsCanLink;
        public static bool SubsCanLink
        {
            get { return _subsCanLink; }
            set
            {
                _subsCanLink = value;
                _settings.SubsCanLink = value;
                _settings.Save();
            }
        }

        private static bool _regularsCanLink = _settings.RegularsCanLink;
        public static bool RegularsCanLink
        {
            get { return _regularsCanLink; }
            set
            {
                _regularsCanLink = value;
                _settings.RegularsCanLink = value;
                _settings.Save();
            }
        }

        private static bool _followGreetingEnabled = _settings.FollowGreetingEnabled;
        public static bool FollowGreetingEnabled
        {
            get { return _followGreetingEnabled; }
            set
            {
                _followGreetingEnabled = value;
                _settings.FollowGreetingEnabled = value;
                _settings.Save();
            }
        }

        private static bool _subGreetingEnabled = _settings.SubGreetingEnabled;
        public static bool SubGreetingEnabled
        {
            get { return _subGreetingEnabled; }
            set
            {
                _subGreetingEnabled = value;
                _settings.SubGreetingEnabled = value;
                _settings.Save();
            }
        }

        private static bool _quotesEnabled = _settings.QuotesEnabled;
        public static bool QuotesEnabled
        {
            get { return _quotesEnabled; }
            set
            {
                _quotesEnabled = value;
                _settings.QuotesEnabled = value;
                _settings.Save();
            }
        }

        private static bool _purgeEnabled = _settings.PurgeEnabled;
        public static bool PurgeEnabled
        {
            get { return _purgeEnabled; }
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
            get { return _songEnabled; }
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
            get { return _songFile; }
            set
            {
                _songFile = value;
                _settings.SongFile = value;
                _settings.Save();
            }
        }

        private static string _followGreeting = _settings.FollowGreeting;
        public static string FollowGreeting
        {
            get { return _followGreeting; }
            set
            {
                _followGreeting = value;
                _settings.FollowGreeting = value;
                _settings.Save();
            }
        }

        private static string _subGreeting = _settings.SubGreeting;
        public static string SubGreeting
        {
            get { return _subGreeting; }
            set
            {
                _subGreeting = value;
                _settings.SubGreeting = value;
                _settings.Save();
            }
        }
    }
}