using LedConnector.Services;
using LedConnector.ViewModels;
using LedConnector.Views;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace LedConnector
{
    public partial class MainWindow : Window
    {
        private ByteLetters byteLetters;

        private Connector? connector;
        private TcpClient? client;

        private bool hasLoadedOnce = false;

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
            
            byteLetters = new();
        }

        private void OnScanCompleted(object sender, EventArgs e)
        {
            splashScreen.Close();
            Show();
        }

        private void SendBtnClick(object sender, RoutedEventArgs e)
        {
            /*List<int> ports = ServerList.SelectedItems.Cast<int>().ToList();

            foreach (int port in ports)
            {
                string message = byteLetters.TranslateToBytes(RawMessage.Text);

                SendMessage(message, port);
            }*/
        }

        private async void SendMessage(string binaryMessage, int port)
        {
            bool isConnected = await ConnectToInstance(port);

            if (isConnected == false)
            {
                MessageBox.Show("Invalid credentials");
                return;
            }

            if (client == null || connector == null)
            {
                MessageBox.Show($"Client or Connector is null");
                return;
            }

            if (!client.Connected)
            {
                MessageBox.Show($"Client is not connected");
                return;
            }

            NetworkStream networkStream = client.GetStream();
            byte[] bytes = Encoding.UTF8.GetBytes(binaryMessage);

            try
            {
                await networkStream.WriteAsync(bytes);
                await networkStream.FlushAsync();
                networkStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send message: {ex.Message}");
                return;
            }
        }

        private async Task<bool> ConnectToInstance(int port)
        {
            connector = new("127.0.0.1", port);

            try
            {
                client = await connector.Connect();

                return true;
            }
            catch (Exception ex)
            {
                client = null;
                MessageBox.Show($"An error occured: {ex.Message}");

                return false;
            }
        }
    }
}