using BitmexGUI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace BitmexGUI.ViewModels.Utilities
{
    public class ColorConverterOrderTag : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderLine data)
            {
                // Return Red if Open < Close, Green otherwise
                if (data.Side.ToLower().Contains("sell"))
                {
                    return Brushes.Red;
                }
                if (data.Side.ToLower().Contains("buy"))
                {
                    return Brushes.LightGreen;
                }
                else
                {
                    return "Error!";
                }

            }
            return Brushes.Transparent; // Default if something goes wrong
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
