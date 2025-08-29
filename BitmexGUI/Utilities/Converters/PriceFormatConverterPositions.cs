using BitmexGUI.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace BitmexGUI.ViewModels.Utilities
{
    class PriceFormatConverterPositions : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if(value is decimal vald)
            {
                var val = (Math.Round(CandlestickChart.MapToScale(double.Parse((Math.Round(vald) * 100000000).ToString())), 2));
                return val;
            }

            else if (value is float valf)
            {
                var val = (Math.Round(CandlestickChart.MapToScale(double.Parse((valf * 100000000).ToString())), 2));
                return val;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
 }
