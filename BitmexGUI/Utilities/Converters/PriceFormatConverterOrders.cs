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
    class PriceFormatConverterOrders : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if(value is decimal vald)
            return vald / 100;
            else if(value is double val) 
                    return val / 100;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
 }
