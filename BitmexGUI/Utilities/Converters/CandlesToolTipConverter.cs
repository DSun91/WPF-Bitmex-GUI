using BitmexGUI.BaseClasses;
using BitmexGUI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BitmexGUI.ViewModels.Utilities
{
    public class CandlesToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if (value is CandleStickViewModel candle)
            {
                string timestamp = ((DateTime)candle.Timestamp).ToString("d/MM/y HH:mm:ss");
                string Open = Math.Round(OHLCandlestickChart.InvMapToScale(candle.Open)/100000000,2).ToString();
                string High = Math.Round(OHLCandlestickChart.InvMapToScale(candle.High) / 100000000, 2).ToString();
                string Low =  Math.Round(OHLCandlestickChart.InvMapToScale(candle.Low) / 100000000, 2).ToString();
                string Close =Math.Round(OHLCandlestickChart.InvMapToScale(candle.Close) / 100000000, 2).ToString();
               
                return $"{timestamp}\n" + $"Open: {Open}\n" + $"High: {High}\n" + $"Low: {Low}\n" + $"Close: {Close}";

            }
            return string.Empty;  
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
