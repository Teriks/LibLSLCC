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
        private readonly string _startingName;
        private readonly bool _generateNumericSuffix;
        private readonly HashSet<string> _takenNames;


        public static readonly DependencyProperty ChosenNameProperty = DependencyProperty.Register(
            "ChosenName", typeof (string), typeof (UniqueNamerWindow), new PropertyMetadata(default(string), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            UniqueNamerWindow window = (UniqueNamerWindow) dependencyObject;
            string val = (string)dependencyPropertyChangedEventArgs.NewValue;
            window.OkButton.IsEnabled = true;
            if (string.IsNullOrWhiteSpace(val))
            {
                window.OkButton.IsEnabled = false;
                throw new Exception("Must provide a name.");
            }

            if (window._takenNames.Contains(val) && !(window._generateNumericSuffix==false && window._startingName == val))
            {
                window.OkButton.IsEnabled = false;
                throw new Exception("That name is taken already.");
            }
        }

        public string ChosenName
        {
            get { return (string) GetValue(ChosenNameProperty); }
            set { SetValue(ChosenNameProperty, value); }
        }
        public UniqueNamerWindow(IEnumerable<string> takenNames, string startingName, bool generateNumericSuffix = true)
        {
            _startingName = startingName;
            _generateNumericSuffix = generateNumericSuffix;
            _takenNames = new HashSet<string>(takenNames);
            InitializeComponent();
            Canceled = true;

            if (_generateNumericSuffix)
            {

                var name = _startingName;
                int num = 1;

                while (_takenNames.Contains(name))
                {
                    name = _startingName + " " + num;
                    num++;
                }

                ChosenName = name;
            }
            else
            {
                ChosenName = _startingName;
            }
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
