using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LSLCCEditor.SettingsUI
{
    /// <summary>
    /// Interaction logic for EditorPane.xaml
    /// </summary>
    public partial class EditorPane : UserControl, ISettingsPane
    {
        public EditorPane()
        {
            InitializeComponent();

            Title = "Editor Settings";
        }

        public string Title { get; private set; }
        public SettingsWindow Owner { get; set; }

    }
}
