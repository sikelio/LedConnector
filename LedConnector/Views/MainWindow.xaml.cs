using LedConnector.Components;
using LedConnector.Models.Database;
using LedConnector.Services;
using LedConnector.ViewModels;
using System.IO;
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
        }

        private void SendBtnClick(object sender, RoutedEventArgs e)
        {
            string message = byteLetters.TranslateToBytes(RawMessage.Text);

            SendMessage(message);
        }
    
        private async void SendMessage(string binaryMessage)
        {
            bool isConnected = await ConnectToInstance();

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

        private async Task<bool> ConnectToInstance()
        {
            string address = IP.Text;
            int port;

            try
            {
                port = int.Parse(Port.Text);
            }
            catch
            {
                port = 1234;
            }

            connector = new(address, port);

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
                SendMessage(shapeBtn.Message.BinaryMessage);
            }
        }
    }
}