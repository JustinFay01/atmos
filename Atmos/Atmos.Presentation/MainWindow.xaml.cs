using System.Windows;
using Atmos.Presentation.Features.Dashboard.Pages;

namespace Atmos.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(new DashboardPage());
        }
    }
}
