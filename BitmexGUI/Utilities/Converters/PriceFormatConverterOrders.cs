using BitmexGUI.BaseClasses;
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
    class PriceFormatConverterOrders : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if(value is decimal vald)
            {
                var val = (Math.Round(OHLCandlestickChart.MapToScale(double.Parse((vald * 100000000).ToString())), 2));
                return val;
            }
            
            else if(value is double val)
            {
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
