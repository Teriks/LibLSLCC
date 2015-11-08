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
    /// Interaction logic for CompilerPane.xaml
    /// </summary>
    public partial class CompilerPane : UserControl, ISettingsPane
    {
        public CompilerPane()
        {
            InitializeComponent();
        }

        public string Title { get { return "Compiler"; } }
        public void Init(SettingsWindow window)
        {
            
        }

    }
}
