using System;
using System.Collections.Generic;
using System.IO;
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

namespace LSLCCEditor
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public static readonly DependencyProperty NameAndVersionProperty = DependencyProperty.Register("NameAndVersion", typeof(string), typeof(About), new PropertyMetadata(default(string)));

        public string NameAndVersion
        {
            get { return (string)GetValue(NameAndVersionProperty); }
            set { SetValue(NameAndVersionProperty, value); }
        }

        public About()
        {

            InitializeComponent();
            NameAndVersion = "LSLCCEditor v" + Assembly.GetCallingAssembly().GetName().Version;

            foreach (var assy in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = assy.GetName();

                if(name.Name == "Microsoft.GeneratedCode") continue;

                LoadedAssembliesBox.Items.Add(name.Name + " v"+name.Version);
            }

            using (var reader = File.OpenRead("license.rtf"))
            {
                LicenseText.SelectAll();
                LicenseText.Selection.Load(reader, DataFormats.Rtf);
            }
        }
    }
}
