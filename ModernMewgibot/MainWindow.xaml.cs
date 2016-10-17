using FirstFloor.ModernUI.Windows.Controls;
using ModernMewgibot.Services;
using ModernMewgibot.ViewModels;

namespace ModernMewgibot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        private BotViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();

            JsonFileService.InitializeAllFiles();

            viewModel = new BotViewModel();
            this.DataContext = viewModel;

            Pages.Settings.AppearanceViewModel settings = new Pages.Settings.AppearanceViewModel();
            settings.SetThemeAndColor(UserInterface.Default.SelectedThemeDisplayName,
                UserInterface.Default.SelectedThemeSource,
                UserInterface.Default.SelectedAccentColor,
                UserInterface.Default.SelectedFontSize);
        }
    }
}
