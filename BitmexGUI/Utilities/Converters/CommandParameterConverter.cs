using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BitmexGUI.ViewModels.Utilities
{

    public class CommandParameterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Example: Concatenate or create a custom object
            if (values[0] != null && values[1] != null)
            {
                string param1 = values[0].ToString();
                string param2 = values[1].ToString();

                // Combine or modify the value as needed
                return Tuple.Create(param1, param2); ; // Or return a custom object
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
