using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LibLSLCC.AutoCompleteParser;

namespace LSLCCEditor
{
    /// <summary>
    /// Interaction logic for DebugObjectView.xaml
    /// </summary>
    public partial class DebugObjectView : Window
    {
        public DebugObjectView()
        {
            InitializeComponent();
        }

        public void ViewObject(string empty, object lslAutoCompleteParser)
        {
            this.Properties.Items.Clear();
            var members = lslAutoCompleteParser.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var member in members)
            {
                var val = member.GetValue(lslAutoCompleteParser);
                if (val == null) val = "NULL";

                this.Properties.Items.Add(member.Name + " = "+val+";");
            }
        }
    }
}
