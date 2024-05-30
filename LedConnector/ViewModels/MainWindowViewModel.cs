using LedConnector.Components;
using LedConnector.Models.Database;
using LedConnector.Models.Dto;
using LedConnector.Services;
using LedConnector.Views;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;

namespace LedConnector.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ByteLetters byteLetters = new ByteLetters();

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ScanCompleted;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ICommand SendMsgCmd { get; set; }
        private ICommand _saveMsgCmd;
        public ICommand SaveMsgCmd
        {
            get { return _saveMsgCmd; }
            set { _saveMsgCmd = value; }
        }
        public ICommand EditMsgCmd { get; set; }
        public ICommand DeleteMsgCmd { get; set; }
        public ICommand SendSavedCmd { get; set; }
        public ICommand RefreshPortsCmd { get; set; }
        public ICommand ExportMessagesCmd { get; set; }
        public ICommand ImportMessagesCmd { get; set; }

        private string _rawMessage;
        public string RawMessage
        {
            get { return _rawMessage; }
            set
            {
                _rawMessage = value;
                OnPropertyChanged(nameof(RawMessage));
            }
        }

        private string _tags;
        public string Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                OnPropertyChanged(nameof(Tags));
            }
        }

        private string _filterText;
        public string FilterText
        {
            get { return _filterText; }
            set
            {
                _filterText = value;
                OnPropertyChanged(nameof(FilterText));
                ApplyFilter();
            }
        }

        private bool _isScanning;
        public bool IsScanning
        {
            get { return _isScanning; }
            set
            {
                _isScanning = value;
                OnPropertyChanged(nameof(IsScanning));
            }
        }

        public ObservableCollection<int> ServerList { get; set; }
        public ObservableCollection<int> SelectedServers { get; set; }
        public List<Message> Messages { get; set; }
        public ObservableCollection<ShapeBtn> MsgButtons { get; set; }
        public ICollectionView FilteredMsgButtons { get; set; }

        public MainWindowViewModel()
        {
            ServerList = [];
            SelectedServers = [];

            SendMsgCmd = new RelayCommand(SendMessage, CanSendMessage);
            SaveMsgCmd = new RelayCommand(SaveMessage, CanSaveMessage);
            EditMsgCmd = new RelayCommand(EditMessage, CanEditMessage);
            DeleteMsgCmd = new RelayCommand(DeleteMessage, CanDeleteMessage);
            SendSavedCmd = new RelayCommand(SendSavedMessage, CanSendSavedMessage);
            RefreshPortsCmd = new RelayCommand(async _ => await ScanPorts(), CanRefreshServerList);
            ExportMessagesCmd = new RelayCommand(ExportMessages, CanExport);
            ImportMessagesCmd = new RelayCommand(ImportMessages, CanImport);

            _ = ScanPorts();

            MsgButtons = [];
            FilteredMsgButtons = CollectionViewSource.GetDefaultView(MsgButtons);
            CreateButtons();
        }

        private async void SendMessage(object parameter)
        {
            List<int> ports = new(SelectedServers);
            string binaryMessage = byteLetters.TranslateToBytes(RawMessage);

            foreach (int port in ports)
            {
                await SendMessage(port, binaryMessage);
            }
        }

        private async void SaveMessage(object parameter)
        {
            if (string.IsNullOrEmpty(_rawMessage))
            {
                return;
            }

            Message message = new()
            {
                RawMessage = _rawMessage,
                BinaryMessage = byteLetters.TranslateToBytes(_rawMessage)
            };

            Message msg = await Query.AddMessage(message);
            await SaveTags(msg.Id);

            MsgButtons.Add(new ShapeBtn(message, EditMsgCmd, DeleteMsgCmd, SendSavedCmd));
            Messages.Add(message);

            MessageBox.Show("Message saved!");
        }

        private async Task SaveTags(int messageId)
        {
            if (string.IsNullOrEmpty(_tags))
            {
                return;
            }

            string[] tags = _tags.Split(',');

            foreach (string tag in tags)
            {
                Tag? potentialTag = await Query.FindTagByName(tag.Trim());

                if (potentialTag == null)
                {
                    Tag newTag = await Query.AddTag(new() { Name = tag.Trim() });
                    await Query.LinkTagToMessage(messageId, newTag.Id);
                }
                else
                {
                    await Query.LinkTagToMessage(messageId, potentialTag.Id);
                }
            }
        }

        private async void EditMessage(object parameter)
        {
            if (parameter is ShapeBtn shapeBtn)
            {
                Message message = shapeBtn.Message;
                string initialTags = string.Join(",", message.Tags.Select(t => t.Name));

                var editDialog = new EditMessage
                {
                    DataContext = new EditMessageViewModel
                    {
                        Message = message,
                        Tags = initialTags
                    }
                };

                if (editDialog.ShowDialog() == true)
                {
                    message.BinaryMessage = byteLetters.TranslateToBytes(message.RawMessage);

                    bool success = await Query.EditMessage(message);

                    if (!success)
                    {
                        MessageBox.Show("Error while editing the message");
                        return;
                    }

                    List<string>? newTags = ((EditMessageViewModel)editDialog.DataContext).Tags
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(tag => tag.Trim())
                        .ToList();

                    success = await Query.UpdateMessageTag(message.Id, newTags);

                    if (!success)
                    {
                        MessageBox.Show("Error while editing the message");
                        return;
                    }

                    Message? newMsg = await Query.FindMessageById(message.Id);

                    if (newMsg == null)
                    {
                        return;
                    }

                    shapeBtn.Message = newMsg;
                    OnPropertyChanged(nameof(MsgButtons));
                    FilteredMsgButtons.Refresh();
                }
            }
        }

        private async void DeleteMessage(object parameter)
        {
            if (parameter is ShapeBtn shapeBtn)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this message", "Delete confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                {
                    return;
                }

                bool success = await Query.DeleteMessage(shapeBtn.Message);

                if (!success)
                {
                    MessageBox.Show("Error during the delete");
                    return;
                }

                Messages.Remove(shapeBtn.Message);
                MsgButtons.Remove(shapeBtn);

                MessageBox.Show("The message have been removed");
            }
        }

        private async void CreateButtons()
        {
            try
            {
                Messages = await Query.GetMessages();
                OnPropertyChanged(nameof(Messages));
            }
            catch
            {
                Messages = new List<Message>();
            }

            foreach (Message message in Messages)
            {
                MsgButtons.Add(new ShapeBtn(message, EditMsgCmd, DeleteMsgCmd, SendSavedCmd));
            }

            OnPropertyChanged(nameof(MsgButtons));
        }

        private void ApplyFilter()
        {
            if (FilteredMsgButtons != null)
            {
                FilteredMsgButtons.Filter = item =>
                {
                    if (item is ShapeBtn shapeBtn)
                    {
                        return string.IsNullOrEmpty(FilterText) || shapeBtn.Message.RawMessage.Contains(FilterText, StringComparison.OrdinalIgnoreCase);
                    }

                    return true;
                };

                FilteredMsgButtons.Refresh();
            }
        }

        private async void SendSavedMessage(object parameter)
        {
            if (parameter is ShapeBtn shapeBtn)
            {
                List<int> ports = new(SelectedServers);

                foreach (int port in ports)
                {
                    await SendMessage(port, shapeBtn.Message.BinaryMessage);
                }
            }
        }

        private async Task SendMessage(int port, string binaryMsg)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(binaryMsg);

            try
            {
                Connector connector = new("127.0.0.1", port);
                TcpClient client = await connector.Connect();
                NetworkStream stream = client.GetStream();

                await stream.WriteAsync(buffer);
                await stream.FlushAsync();
                stream.Close();
            }
            catch
            {
                return;
            }
        }

        private async Task ScanPorts()
        {
            IsScanning = true;

            List<int> ports = [];
            int startPort = 1234, endport = 1244;

            for (int port = startPort; port <= endport; port++)
            {
                string scanMessage = byteLetters.TranslateToBytes(" ");
                byte[] buffer = Encoding.UTF8.GetBytes(scanMessage);

                try
                {
                    await Task.Run(async () =>
                    {
                        TcpClient connection = new("127.0.0.1", port);
                        NetworkStream netStream = connection.GetStream();

                        await netStream.WriteAsync(buffer);
                        await netStream.FlushAsync();
                        netStream.Close();

                        ports.Add(port);
                        Trace.WriteLine($"Alive connection on port {port}");
                    });
                }
                catch
                {
                    continue;
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                ServerList.Clear();

                foreach (var port in ports)
                {
                    ServerList.Add(port);
                }
            });

            IsScanning = false;
            ScanCompleted?.Invoke(this, EventArgs.Empty);
        }

        private async void ExportMessages(object parameter)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "JSON Files (*.json)|*.json",
                DefaultExt = "json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                List<MessageDto> messages = await Query.FindAllMessages();

                JsonSerializerOptions options = new()
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(messages, options);
                await File.WriteAllTextAsync(saveFileDialog.FileName, json);

                MessageBox.Show("Messages exported successfully!");
            }
        }

        private async void ImportMessages(object parameter)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "JSON Files (*.json)|*.json",
                DefaultExt = "json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string json = await File.ReadAllTextAsync(openFileDialog.FileName);
                List<MessageDto>? importedMessages = JsonSerializer.Deserialize<List<MessageDto>>(json);

                if (importedMessages == null)
                {
                    MessageBox.Show("Json is invalid or null");
                    return;
                }

                foreach (MessageDto message in importedMessages)
                {
                    Message newMsg = new()
                    {
                        RawMessage = message.RawMessage,
                        BinaryMessage = message.BinaryMessage,
                        Tags = new List<Tag>()
                    };

                    Message addedMsg = await Query.AddMessage(newMsg);

                    foreach (TagDto tag in message.Tags)
                    {
                        Tag? dbTag = await Query.FindTagByName(tag.Name);

                        Tag newTag = new()
                        {
                            Name = tag.Name
                        };

                        if (dbTag == null)
                        {
                            Tag addedTag = await Query.AddTag(newTag);
                            await Query.LinkTagToMessage(addedMsg.Id, addedTag.Id);
                        }
                        else
                        {
                            await Query.LinkTagToMessage(addedMsg.Id, dbTag.Id);
                        }
                    }
                }

                CreateButtons();
                MessageBox.Show("Messages imported successfully!");
            }
        }

        private bool CanSendMessage(object parameter)
        {
            return true;
        }

        private bool CanSaveMessage(object parameter)
        {
            return true;
        }

        private bool CanEditMessage(object parameter)
        {
            return true;
        }

        private bool CanDeleteMessage(object parameter)
        {
            return true;
        }

        private bool CanSendSavedMessage(object parameter)
        {
            return true;
        }

        private bool CanRefreshServerList(object parameter)
        {
            return true;
        }

        private bool CanExport(object parameter)
        {
            return true;
        }

        private bool CanImport(object parameter)
        {
            return true;
        }
    }
}
