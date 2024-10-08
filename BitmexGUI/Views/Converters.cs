﻿using BitmexGUI.Models;
using BitmexGUI.Services.Implementations;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace BitmexGUI.Views
{
   
       
    public class PriceConverterMapSettledPrice : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double data)
            {
                return Math.Round(CandlestickChart.MapToScale(double.Parse(data.ToString())), 2).ToString();
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

   


    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && double.TryParse(stringValue, out double result))
            {
                return result;
            }
            return 0.0; // Default value or handle as needed
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                return doubleValue.ToString(culture);
            }
            return string.Empty; // Default value or handle as needed
        }
    }
    public class PositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CandlestickData data)
            {

                return data.Close > data.Open ? data.Open : data.Close;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class PositionConverterCandlestickX : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CandlestickData data)
            {

                return (data.Posx - data.Width/2);
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class PositionConverterOrderlineY : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderLine data)
            {

                return data.Price-20;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class PriceConverterMapOrderline : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            { 
                return (Math.Round(CandlestickChart.MapToScale(double.Parse(decimalValue.ToString())),2) ) ;
            }
            else if (value is double doubleValue)
            {
                return (Math.Round(CandlestickChart.MapToScale(doubleValue), 2) );
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class PriceConverterMapLabel : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                return (Math.Round(CandlestickChart.MapToScale(double.Parse(decimalValue.ToString())), 2) - 12.5);
            }
            else if (value is double doubleValue)
            {
                return (Math.Round(CandlestickChart.MapToScale(doubleValue), 2) - 12.5);
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PriceConverterInvMapOrderline : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            


            if (value is decimal data)
            {
                 
                return Math.Round(CandlestickChart.InvMapToScale(double.Parse(data.ToString())),2).ToString();

            }
            else
            {
                return 0;
            }
          
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CandlestickData data)
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


    public class ColorConverterOrderTag : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderLine data)
            {
                // Return Red if Open < Close, Green otherwise
                if(data.Side.ToLower().Contains("sell"))
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
