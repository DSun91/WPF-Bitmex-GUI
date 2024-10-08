﻿namespace BitmexGUI.Models
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

        public double Height => Math.Abs(Open - Close) + 1;

        public double Width { get; set; }
        public double CenterY => Math.Abs(High - Low) / 2;

        public double CenterX => Width / 2;

    }

    public class SettledPrice
    {
        public string Symbol { get; set; }
        public double SettledPriceValue { get; set; }

        public string Timestamp { get; set; }

    }

    public class CurrentClosePrice
    {
        public string Symbol { get; set; }
        public double PriceValue { get; set; }

        public string Timestamp { get; set; }

    }

    public class Pattern
    {
        public CandlestickData P0 { get; set; }
        public CandlestickData P1 { get; set; }
        public CandlestickData P2 { get; set; }
        public CandlestickData P3 { get; set; }
        public CandlestickData P4 { get; set; }
        public CandlestickData P5 { get; set; }
        public CandlestickData P6 { get; set; }
        public CandlestickData P7 { get; set; }

    }
}
