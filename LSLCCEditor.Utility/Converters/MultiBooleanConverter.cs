#region Imports

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

#endregion

namespace LSLCCEditor.Utility.Converters
{
    public class MultiBooleanConverter : IMultiValueConverter
    {
        public bool And { get; set; }
        public bool True { get; set; }
        public bool False { get; set; }


        public MultiBooleanConverter()
        {
            True = true;
            False = false;
        }


        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return false;

            try
            {
                if (And)
                {
                    var value = values.Select(System.Convert.ToBoolean).Aggregate(true, (current, v) => current && v);
                    return value ? True : False;
                }
                else
                {
                    var value = values.Select(System.Convert.ToBoolean).Aggregate(false, (current, v) => current || v);
                    return value ? True : False;
                }
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}