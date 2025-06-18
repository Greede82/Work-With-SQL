using System.Windows;

namespace WpfApp1.Views
{
    public partial class UserGuideWindow : Window
    {
        public UserGuideWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
