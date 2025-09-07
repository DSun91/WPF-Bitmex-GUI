namespace BitmexGUI.Interfaces
{
    internal interface IPriceTicker
    {
        public string Symbol { get; set; }
        public double PriceValue { get; set; }
        public string Timestamp { get; set; }

    }
}
