using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BitmexGUI.ViewModels.Utilities
{
    public class PriceFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                var val= Math.Round(decimalValue / 100000000, 2);
                return val;
            }
            else if (value is double doubleValue)
            {
                var val = Math.Round(doubleValue / 100000000, 2);
                return val;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

}
