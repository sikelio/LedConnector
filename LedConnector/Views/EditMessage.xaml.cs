using System.Windows;

namespace LedConnector.Views
{
    public partial class EditMessage : Window
    {
        public EditMessage()
        {
            InitializeComponent();
        }

        private void EditBtnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelBtnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
