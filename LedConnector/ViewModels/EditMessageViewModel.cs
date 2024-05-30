using LedConnector.Models.Database;
using LedConnector.Services;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LedConnector.ViewModels
{
    public class EditMessageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private Message _message;
        public Message Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
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

        public ICommand EditCmd { get; set; }
        public ICommand CancelCmd { get; set; }

        public EditMessageViewModel()
        {
            EditCmd = new RelayCommand(Edit, CanEdit);
            CancelCmd = new RelayCommand(Cancel, CanCancel);
        }

        private void Edit(object parameter)
        {
            if (parameter is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        private void Cancel(object parameter)
        {
            if (parameter is Window window)
            {
                window.DialogResult = false;
                window.Close();
            }
        }



        private bool CanEdit(object parameter)
        {
            if (string.IsNullOrEmpty(_message.RawMessage))
            {
                return false;
            }

            return true;
        }

        private bool CanCancel(object parameter)
        {
            return true;
        }
    }
}
