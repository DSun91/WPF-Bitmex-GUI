using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Models
{
    public class Order
    {
        public string? OrderID { get; set; }        // Represents "orderID"
        public int? Account { get; set; }           // Represents "account"
        public string? Symbol { get; set; }         // Represents "symbol"
        public string? Side { get; set; }           // Represents "side"
        public int? OrderQty { get; set; }          // Represents "orderQty"
        public decimal? Price { get; set; }        // Represents "price"
        public int? DisplayQty { get; set; }       // Represents "displayQty"
        public decimal? StopPx { get; set; }       // Represents "stopPx"
        public decimal? PegOffsetValue { get; set; } // Represents "pegOffsetValue"
        public string? Currency { get; set; }       // Represents "currency"
        public string? SettlCurrency { get; set; }  // Represents "settlCurrency"
        public string? OrdType { get; set; }        // Represents "ordType"
        public string? TimeInForce { get; set; }    // Represents "timeInForce"
        public string? OrdStatus { get; set; }      // Represents "ordStatus"
        public bool? WorkingIndicator { get; set; } // Represents "workingIndicator"
        public int? LeavesQty { get; set; }         // Represents "leavesQty"
        public int? CumQty { get; set; }            // Represents "cumQty"
        public decimal? AvgPx { get; set; }         // Represents "avgPx"
        public string? Text { get; set; }           // Represents "text"
        public DateTime? TransactTime { get; set; } // Represents "transactTime"
        public DateTime? Timestamp { get; set; }    // Represents "timestamp"
    }
}
