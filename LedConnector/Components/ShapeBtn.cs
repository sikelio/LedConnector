using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Input;

namespace LedConnector.Components
{
    public class ShapeBtn
    {
        private readonly int _width = 44;
        private readonly int _heigth = 11;

        public ObservableCollection<Shape> LedShapes { get; set; }

        public ICommand EditCmd { get; set; }
        public ICommand DeleteCmd { get; set; }
        
        public ShapeBtn(string binaryMsg, ICommand editCmd, ICommand deleteCmd)
        {
            LedShapes = new ObservableCollection<Shape>();

            int maxLength = _width * _heigth;
            binaryMsg = binaryMsg.PadRight(maxLength, '0');

            for (int i = 0; i < maxLength; i++)
            {
                Rectangle rectangle = new()
                {
                    Width = 10,
                    Height = 10,
                    Fill = binaryMsg[i] == '1' ? Brushes.Green : Brushes.LightGray
                };

                LedShapes.Add(rectangle);
            }

            EditCmd = editCmd;
            DeleteCmd = deleteCmd;
        }
    }
}
