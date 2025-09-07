using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Interfaces
{
    interface IPosition
    {
        public int AccountID { get; set; }
        public string Symbol { get; set; }
        public string Currency { get; set; } 
        public string QuoteCurrency { get; set; }
        public float? Commission { get; set; } 
        public float? Leverage { get; set; } 
        public float? CurrentQty { get; set; }  
        public float? RealisedCost { get; set; }
        public float? UnrealisedCost { get; set; } 
        public bool IsOpen { get; set; } 
        public float? RealisedPnl { get; set; }
        public float? UnrealisedPnl { get; set; }  
        public float? AvgEntryPrice { get; set; }
        public float? BreakEvenPrice { get; set; }
        public float? MarginCallPrice { get; set; }
        public float? LiquidationPrice { get; set; }  
        public DateTime Timestamp { get; set; }
    }
}
