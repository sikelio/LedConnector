using LedConnector.Services;
using LedConnector.ViewModels;
using System.IO;
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

        public MainWindow()
        {
            InitializeComponent();
            byteLetters = new();
            DataContext = new MainWindowViewModel();
        }

        private async void SendBtnClick(object sender, RoutedEventArgs e)
        {
            string message = CreateBinaryMessage();

            bool isConnected = await ConnectToInstance();

            if (isConnected == false)
            {
                MessageBox.Show("Invalid credentials");
                return;
            }

            SendMessage(message);
        }

        private string CreateBinaryMessage()
        {
            string rawMessage = RawMessage.Text;

            if (rawMessage.Length == 0)
            {
                return string.Empty;
            }

            int count = 1;
            int start = 0;
            string binaryMessage = "";

            for (int i = 1; i < 11; i++)
            {
                foreach (char letter in rawMessage)
                {
                    string byteLetter = byteLetters.GetByteLetter(letter.ToString());
                    binaryMessage += byteLetter.Substring(start, 5);
                }

                while (binaryMessage.Length < 44 * count)
                {
                    binaryMessage += "0";
                }

                count++;
                start += 5;
            }

            return binaryMessage;
        }
    
        private async void SendMessage(string binaryMessage)
        {
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
    }
}