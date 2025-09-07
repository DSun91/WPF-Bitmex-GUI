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
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CandleStickViewModel data)
            {
                // Return Red if Open < Close, Green otherwise
                return data.Open < data.Close ? Brushes.Red : Brushes.Green;
            }
            return Brushes.Transparent; // Default if something goes wrong
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
