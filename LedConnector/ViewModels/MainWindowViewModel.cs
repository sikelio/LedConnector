using LedConnector.Models.Database;
using LedConnector.Services;
using System.ComponentModel;

namespace LedConnector.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public List<Message> Messages { get; set; }

        public MainWindowViewModel()
        {

            SetMessages();
        }

        private async void SetMessages()
        {
            try
            {
                Messages = await Query.GetMessages();
                OnPropertyChanged("Messages");
            }
            catch (Exception ex)
            {
                Messages = new List<Message>();
            }
        }
    }
}
