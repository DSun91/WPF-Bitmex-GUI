using BitmexGUI.BaseClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BitmexGUI.ViewModels.Utilities
{
    public class PriceConverterMapOrderline : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                return (Math.Round(OHLCandlestickChart.MapToScale(double.Parse(decimalValue.ToString())), 2));
            }
            else if (value is double doubleValue)
            {
                return (Math.Round(OHLCandlestickChart.MapToScale(doubleValue), 2));
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
