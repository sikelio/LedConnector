using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using LedConnector.Models.Database;
using System.ComponentModel;

namespace LedConnector.Components
{
    public class ShapeBtn
    {
        private readonly int _width = 44;
        private readonly int _height = 11;

        private Message _message;
        public Message Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
                UpdateShapes();
            }
        }

        public ObservableCollection<Shape> LedShapes { get; set; }

        public ICommand EditCmd { get; set; }
        public ICommand DeleteCmd { get; set; }

        public ShapeBtn(Message message, ICommand editCmd, ICommand deleteCmd)
        {
            _message = message;
            LedShapes = new ObservableCollection<Shape>();
            EditCmd = editCmd;
            DeleteCmd = deleteCmd;

            UpdateShapes();
        }

        private void UpdateShapes()
        {
            LedShapes.Clear();
            int maxLength = _width * _height;
            string binaryMessage = _message.BinaryMessage.PadRight(maxLength, '0');

            for (int i = 0; i < maxLength; i++)
            {
                Rectangle rectangle = new()
                {
                    Width = 10,
                    Height = 10,
                    Fill = binaryMessage[i] == '1' ? Brushes.Green : Brushes.LightGray
                };

                LedShapes.Add(rectangle);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

