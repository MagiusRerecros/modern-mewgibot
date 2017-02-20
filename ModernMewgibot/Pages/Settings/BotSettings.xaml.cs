using ModernMewgibot.ViewModels;
using System.Windows.Controls;

namespace ModernMewgibot.Pages.Settings
{
    /// <summary>
    /// Interaction logic for BotSettings.xaml
    /// </summary>
    public partial class BotSettings : UserControl
    {
        public BotSettings()
        {
            InitializeComponent();

            // Set Data Context to View Model
            this.DataContext = new SettingsViewModel();
        }
    }
}
