using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using LSLCCEditor.Utility.Validation;

namespace LSLCCEditor.Utility.Converters
{
    public class ElementValidConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DependencyObject obj = (DependencyObject)parameter;
            if (obj != null)
            {
                return obj.IsValid();
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
