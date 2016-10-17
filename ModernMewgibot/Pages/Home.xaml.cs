using System.Windows.Controls;

namespace ModernMewgibot.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();

            // Set Data Context to ViewModel
            //BotViewModel vm = new BotViewModel();
            //this.DataContext = vm;
            //rtb.DataContext = vm;
        }
    }
}
