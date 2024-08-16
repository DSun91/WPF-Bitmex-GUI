using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Models
{
    public class CandlestickData
    {
        public string Symbol { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public DateTime Timestamp { get; set; }

        public double Posx { get; set; }

        public double Height => Math.Abs(Open - Close);

        public double Width { get; set; } = 10;

    }

    public class SettledPrice
    {
        public string Symbol { get; set; }
        public double SettledPriceValue { get; set; }

        public string Timestamp { get; set; }

    }


}
