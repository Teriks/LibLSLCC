using System.Reflection;
using System.Windows;

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
            Properties.Items.Clear();
            var members = lslAutoCompleteParser.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var member in members)
            {
                var val = member.GetValue(lslAutoCompleteParser);
                if (val == null) val = "NULL";

                Properties.Items.Add(member.Name + " = "+val+";");
            }
        }
    }
}
