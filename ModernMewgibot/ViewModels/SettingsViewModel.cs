using ModernMewgibot.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ModernMewgibot.Utils;

namespace ModernMewgibot.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
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
                songEnabled = value;
                OnPropertyChanged();
                SettingsService.LinkModEnabled = value;
            }
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
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.Filter = "Text Files (*.txt)|*.txt";
            fileDialog.Multiselect = false;
            fileDialog.RestoreDirectory = true;

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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
