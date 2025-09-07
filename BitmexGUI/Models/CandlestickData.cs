using BitmexGUI.Interfaces;
using System.ComponentModel;

namespace BitmexGUI.Models
{
    public class CandlestickData : ICandlestick
    {
        public string Symbol { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public DateTime Timestamp { get; set; } 
        public double Posx { get; set; } 
        public double Height => Math.Abs(Open - Close) + 1; 
        public double Width { get; set; }
        public double CenterY => Math.Abs(High - Low) / 2; 
        public double CenterX => Width / 2;

    }

}



