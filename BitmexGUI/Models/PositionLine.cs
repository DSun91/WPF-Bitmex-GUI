namespace BitmexGUI.Models
{ 
    public class PositionLine
    {
        public int AccountID { get; set; }
        public string Symbol { get; set; }
        public decimal AvgEntryPrice { get; set; }

        public decimal BreakEvenPrice { get; set; }

        public decimal UnrealisedPnl { get; set; }

        public decimal LiquidationPrice { get; set; }

        public decimal DeltaFromBreakEven { get; set; }

    }
}
