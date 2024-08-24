namespace BitmexGUI.Models
{
    public class OrderLine
    {

        public string OrderID { get; set; }
        public decimal Price { get; set; }
        public string Side { get; set; }
        public string Symbol { get; set; }
    }
    public class PositionLine
    {
        public int AccountID { get; set; }
        public string Symbol { get; set; }
        public decimal AvgEntryPrice { get; set; }

        public decimal BreakEvenPrice { get; set; }

        public int UnrealisedPnl { get; set; }

        public decimal LiquidationPrice { get; set; }

        public decimal DeltaFromBreakEven { get; set; }

    }
}
