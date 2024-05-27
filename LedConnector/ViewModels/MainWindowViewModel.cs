using LedConnector.Components;
using LedConnector.Models.Database;
using LedConnector.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LedConnector.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ByteLetters byteLetters = new ByteLetters();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ICommand _saveCmd;
        public ICommand SaveCmd
        {
            get { return _saveCmd; }
            set { _saveCmd = value; }
        }

        private string _rawMessage;
        public string RawMessage
        {
            get { return _rawMessage; }
            set
            {
                _rawMessage = value;
                OnPropertyChanged("RawMessage");
            }
        }

        public List<Message> Messages { get; set; }

        public ObservableCollection<ShapeBtn> MsgButtons { get; set; }

        public MainWindowViewModel()
        {
            SaveCmd = new RelayCommand(SaveMessage, CanSaveMessage);
            CreateButtons();
        }

        private async void SaveMessage(object parameter)
        {
            if (_rawMessage == "")
            {
                return;
            }

            Message message = new()
            {
                RawMessage = _rawMessage,
                BinaryMessage = byteLetters.TranslateToBytes(_rawMessage)
            };

            bool saveSuccess = await Query.AddMessage(message);

            if (saveSuccess == false)
            {
                MessageBox.Show("Error during message save!");
                return;
            }

            MessageBox.Show("Message saved!");
        }

        private bool CanSaveMessage(object parameter)
        {
            return true;
        }
        
        private async void CreateButtons()
        {
            try
            {
                Messages = await Query.GetMessages();
                OnPropertyChanged("Messages");
            }
            catch
            {
                Messages = [];
            }

            MsgButtons = new ObservableCollection<ShapeBtn>();

            foreach (Message message in Messages)
            {
                MsgButtons.Add(new ShapeBtn(message.BinaryMessage));
            }

            OnPropertyChanged("MsgButtons");
        }
    }
}
