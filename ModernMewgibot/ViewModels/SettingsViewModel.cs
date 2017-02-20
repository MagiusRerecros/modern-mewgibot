using ModernMewgibot.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ModernMewgibot.Utils;
using System;
using System.Xml;
using System.Reflection;
using System.Threading.Tasks;

namespace ModernMewgibot.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        readonly string version = GetPublishedVersion();
        public string Version
        {
            get { return version; }
        }

        string channel = SettingsService.Channel;
        public string Channel
        {
            get { return channel; }
            set
            {
                channel = value;
                OnPropertyChanged();
                SettingsService.Channel = value;
            }
        }

        string songFile = SettingsService.SongFile;
        public string SongFile
        {
            get { return songFile; }
            set
            {
                songFile = value;
                OnPropertyChanged();
                SettingsService.SongFile = value;
            }
        }

        bool songEnabled = SettingsService.SongEnabled;
        public bool SongEnabled
        {
            get { return songEnabled; }
            set
            {
                songEnabled = value;
                OnPropertyChanged();
                SettingsService.SongEnabled = value;
            }
        }

        bool quotesEnabled = SettingsService.QuotesEnabled;
        public bool QuotesEnabled
        {
            get { return quotesEnabled; }
            set
            {
                quotesEnabled = value;
                OnPropertyChanged();
                SettingsService.QuotesEnabled = value;
            }
        }

        bool hostAutoThank = SettingsService.HostAutoThank;
        public bool HostAutoThank
        {
            get { return hostAutoThank; }
            set
            {
                hostAutoThank = value;
                OnPropertyChanged();
                SettingsService.HostAutoThank = value;
            }
        }

        bool followGreetingEnabled = SettingsService.FollowGreetingEnabled;
        public bool FollowGreetingEnabled
        {
            get { return followGreetingEnabled; }
            set
            {
                followGreetingEnabled = value;
                OnPropertyChanged();
                SettingsService.FollowGreetingEnabled = value;
            }
        }

        bool subGreetingEnabled = SettingsService.SubGreetingEnabled;
        public bool SubGreetingEnabled
        {
            get { return subGreetingEnabled; }
            set
            {
                subGreetingEnabled = value;
                OnPropertyChanged();
                SettingsService.SubGreetingEnabled = value;
            }
        }

        bool purgeEnabled = SettingsService.PurgeEnabled;
        public bool PurgeEnabled
        {
            get { return purgeEnabled; }
            set
            {
                purgeEnabled = value;
                OnPropertyChanged();
                SettingsService.PurgeEnabled = value;
            }
        }

        bool linkModEnabled = SettingsService.LinkModEnabled;
        public bool LinkModEnabled
        {
            get { return linkModEnabled; }
            set
            {
                linkModEnabled = value;
                OnPropertyChanged();
                SettingsService.LinkModEnabled = value;
            }
        }

        bool subsCanLink = SettingsService.SubsCanLink;
        public bool SubsCanLink
        {
            get { return subsCanLink; }
            set
            {
                subsCanLink = value;
                OnPropertyChanged();
                SettingsService.SubsCanLink = value;
            }
        }

        bool regularsCanLink = SettingsService.RegularsCanLink;
        public bool RegularsCanLink
        {
            get { return regularsCanLink; }
            set
            {
                regularsCanLink = value;
                OnPropertyChanged();
                SettingsService.RegularsCanLink = value;
            }
        }

        string followGreeting = SettingsService.FollowGreeting;
        public string FollowGreeting
        {
            get { return followGreeting; }
            set
            {
                followGreeting = value;
                OnPropertyChanged();
                SettingsService.FollowGreeting = value;
            }
        }

        string subGreeting = SettingsService.SubGreeting;
        public string SubGreeting
        {
            get { return subGreeting; }
            set
            {
                subGreeting = value;
                OnPropertyChanged();
                SettingsService.SubGreeting = value;
            }
        }

        private static string GetPublishedVersion()
        {
            XmlDocument xmlDoc = new XmlDocument();
            Assembly asmCurrent = Assembly.GetExecutingAssembly();
            string executePath = new Uri(asmCurrent.GetName().CodeBase).LocalPath;

            xmlDoc.Load(executePath + ".manifest");
            string retval = string.Empty;
            if (xmlDoc.HasChildNodes)
            {
                retval = xmlDoc.ChildNodes[1].ChildNodes[0].Attributes.GetNamedItem("version").Value.ToString();
            }

            return retval;
        }

        ICommand connectChannel;
        public ICommand ConnectChannelCommand =>
            connectChannel ?? (connectChannel = new Command(() => ConnectToChannel()));
        private void ConnectToChannel()
        {
            BotViewModel.SwitchChannels(Channel);
        }

        ICommand openFile;
        public ICommand OpenFileCommand =>
            openFile ?? (openFile = new Command(() => OpenFile()));
        private void OpenFile()
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "Text Files (*.txt)|*.txt",
                Multiselect = false,
                RestoreDirectory = true
            };
            var result = fileDialog.ShowDialog();

            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    var file = fileDialog.FileName;
                    SongFile = file;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    SongFile = "Not Configured";
                    break;
            }
        }

        ICommand backupSettings;
        public ICommand BackupSettingsCommand =>
            backupSettings ?? (backupSettings = new Command(() => BackupSettings()));
        private void BackupSettings()
        {
            SettingsService.Export();
        }

        ICommand restoreSettings;
        public ICommand RestoreSettingsCommand =>
            restoreSettings ?? (restoreSettings = new Command(async () => await RestoreSettings()));
        private async Task RestoreSettings()
        {
            await SettingsService.Import();

            Channel = SettingsService.Channel;
            SongFile = SettingsService.SongFile;
            SongEnabled = SettingsService.SongEnabled;
            QuotesEnabled = SettingsService.QuotesEnabled;
            HostAutoThank = SettingsService.HostAutoThank;
            FollowGreetingEnabled = SettingsService.FollowGreetingEnabled;
            SubGreetingEnabled = SettingsService.SubGreetingEnabled;
            PurgeEnabled = SettingsService.PurgeEnabled;
            LinkModEnabled = SettingsService.LinkModEnabled;
            SubsCanLink = SettingsService.SubsCanLink;
            RegularsCanLink = SettingsService.RegularsCanLink;
            FollowGreeting = SettingsService.FollowGreeting;
            SubGreeting = SettingsService.SubGreeting;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
