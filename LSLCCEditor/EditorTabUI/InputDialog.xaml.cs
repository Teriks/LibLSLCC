using System.Windows;

namespace LSLCCEditor
{
    /// <summary>
    ///     Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public InputDialog()
        {
            InitializeComponent();
        }



        public bool Accepted { get; set; }



        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Accepted = true;
            Close();
        }



        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}