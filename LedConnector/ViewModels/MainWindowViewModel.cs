﻿using LedConnector.Components;
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

        public ICommand EditMsgCmd { get; set; }
        public ICommand DeleteMsgCmd { get; set; }

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
            EditMsgCmd = new RelayCommand(EditMessage, CanEditMessage);
            DeleteMsgCmd = new RelayCommand(DeleteMessage, CanDeleteMessage);
            
            MsgButtons = new ObservableCollection<ShapeBtn>();
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

            MsgButtons.Add(new ShapeBtn(message, EditMsgCmd, DeleteMsgCmd));
            Messages.Add(message);

            MessageBox.Show("Message saved!");
        }

        private bool CanSaveMessage(object parameter)
        {
            return true;
        }

        private async void EditMessage(object parameter)
        {
        }

        private bool CanEditMessage(object parameter)
        {
            return true;
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

                if (success == false)
                {
                    MessageBox.Show("Error during the delete");
                    return;
                }

                Messages.Remove(shapeBtn.Message);
                MsgButtons.Remove(shapeBtn);

                MessageBox.Show("The message have been removed");
            }
        }

        private bool CanDeleteMessage(object parameter)
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

            foreach (Message message in Messages)
            {
                MsgButtons.Add(new ShapeBtn(message, EditMsgCmd, DeleteMsgCmd));
            }

            OnPropertyChanged("MsgButtons");
        }
    }
}
