using LedConnector.Components;
using LedConnector.Services;
using LedConnector.ViewModels;
using LedConnector.Views;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace LedConnector
{
    public partial class MainWindow : Window
    {
        private ByteLetters byteLetters;

        private Connector? connector;
        private TcpClient? client;

        public MainWindow()
        {
            InitializeComponent();
            byteLetters = new();
            DataContext = new MainWindowViewModel();
            _ = ScanPort();
        }

        private void SendBtnClick(object sender, RoutedEventArgs e)
        {
            List<int> ports = ServerList.SelectedItems.Cast<int>().ToList();

            foreach (int port in ports)
            {
                string message = byteLetters.TranslateToBytes(RawMessage.Text);

                SendMessage(message, port);
            }
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

        private void SendSavedMessage(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ShapeBtn shapeBtn)
            {
                List<int> ports = ServerList.SelectedItems.Cast<int>().ToList();

                foreach (int port in ports)
                {
                    SendMessage(shapeBtn.Message.BinaryMessage, port);
                }
            }
        }

        private async Task ScanPort()
        {
            Splashscreen splashScreen = new();
            splashScreen.Show();

            List<int> ports = new();
            int startPort = 1234, endport = 1244;

            for (int port = startPort; port <= endport; port++)
            {
                string scanMessage = byteLetters.TranslateToBytes(" ");
                byte[] buffer = Encoding.UTF8.GetBytes(scanMessage);

                try
                {
                    TcpClient connection = new("127.0.0.1", port);
                    NetworkStream netStream = connection.GetStream();

                    await netStream.WriteAsync(buffer);
                    await netStream.FlushAsync();
                    netStream.Close();

                    ports.Add(port);

                    Trace.WriteLine($"Alive connection on port {port}");
                }
                catch
                {
                    continue;
                }
            }

            foreach (int port in ports)
            {
                ServerList.Items.Add(port);
            }

            splashScreen.Close();
        }

        private async void RefreshBtnClick(object sender, RoutedEventArgs e)
        {
            ServerList.Items.Clear();
            await ScanPort();
        }
    }
}