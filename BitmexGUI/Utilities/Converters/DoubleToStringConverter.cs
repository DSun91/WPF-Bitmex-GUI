using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BitmexGUI.ViewModels.Utilities
{
    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                return decimalValue.ToString();
            }
            if (value is double doubleValue)
            {
                return doubleValue.ToString();
            }
            return ""; // Default value or handle as needed
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string doubleValue)
            {
                return double.Parse(doubleValue);
            }
            return string.Empty; // Default value or handle as needed
        }
    }
}
