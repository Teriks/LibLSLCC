using System;
using System.Collections.Generic;
using System.Windows;

namespace LSLCCEditor.SettingsUI
{
    /// <summary>
    /// Interaction logic for UniqueNamerWindow.xaml
    /// </summary>
    public partial class UniqueNamerWindow : Window
    {
        private readonly HashSet<string> _takenNames;


        public static readonly DependencyProperty ChosenNameProperty = DependencyProperty.Register(
            "ChosenName", typeof (string), typeof (UniqueNamerWindow), new PropertyMetadata(default(string), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            UniqueNamerWindow window = (UniqueNamerWindow) dependencyObject;
            string val = (string)dependencyPropertyChangedEventArgs.NewValue;

            if (string.IsNullOrWhiteSpace(val))
            {
                throw new Exception("Must provide a name.");
            }

            if (val != null && window._takenNames.Contains(val))
            {
                throw new Exception("That name is taken already.");
            }
        }

        public string ChosenName
        {
            get { return (string) GetValue(ChosenNameProperty); }
            set { SetValue(ChosenNameProperty, value); }
        }
        public UniqueNamerWindow(IEnumerable<string> takenNames, string startingName)
        {
            _takenNames = new HashSet<string>(takenNames);
            InitializeComponent();


            var name = startingName;
            int num = 1;

            while (_takenNames.Contains(name))
            {
                name = startingName + " " + num;
                num++;
            }

            ChosenName = name;
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            Canceled = false;
            Close();
        }

        public bool Canceled { get; private set; }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            Canceled = true;
            Close();
        }
    }
}
