using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LSLCCEditor.Settings
{
    public class EditorControlSettingsDictionaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dict = value as IDictionary<string, EditorControlSettingsNode>;
            if (dict != null)
            {
                return dict[AppSettings.Settings.CurrentEditorControlConfiguration];
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
