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
    /// Interaction logic for CompletionPane.xaml
    /// </summary>
    public partial class CompletionPane : UserControl,  ISettingsPane
    {
        public CompletionPane()
        {
            InitializeComponent();
            Title = "Completion Settings";
        }

        public int Priority { get { return 2; } }
        public string Title { get; private set; }

        public void Init(SettingsWindow window)
        {
            
        }
    }
}
