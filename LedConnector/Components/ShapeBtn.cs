using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;

namespace LedConnector.Components
{
    public class ShapeBtn
    {
        private readonly int _width = 44;
        private readonly int _heigth = 11;

        public ObservableCollection<Shape> LedShapes { get; set; }
        
        public ShapeBtn(string binaryMsg)
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
        }
    }
}
