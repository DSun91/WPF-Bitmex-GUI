using BitmexGUI.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BitmexGUI.ViewModels.Utilities
{
    public class PriceConverterMapLabel : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           
            if (value is decimal decimalValue)
            {
                var val = (Math.Round(CandlestickChart.MapToScale(double.Parse(decimalValue.ToString())), 2) - 12.5);
                return val;
            }
            else if (value is double doubleValue)
            {
                if (doubleValue > 10e6)
                {
                    var val = (Math.Round(CandlestickChart.MapToScale(doubleValue), 2) - 12.5);
                    return val;
                }
                else
                {
                    var corrected = doubleValue * 100000000;
                    var val = (Math.Round(CandlestickChart.MapToScale(corrected), 2) - 12.5);
                    return val;
                }
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
