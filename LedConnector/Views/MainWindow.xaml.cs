using LedConnector.ViewModels;
using LedConnector.Views;
using System.Windows;

namespace LedConnector
{
    public partial class MainWindow : Window
    {
        private Splashscreen splashScreen;
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new();
            DataContext = viewModel;

            Hide();
            splashScreen = new();
            splashScreen.Show();

            viewModel.ScanCompleted += OnScanCompleted;
        }

        private void OnScanCompleted(object sender, EventArgs e)
        {
            splashScreen.Close();
            Show();
        }
    }
}