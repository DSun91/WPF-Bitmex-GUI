using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Interfaces
{
    interface IOrder
    {
        public string? OrderID { get; set; }        // Represents "orderID"
        public int? Account { get; set; }           // Represents "account"
        public string? Symbol { get; set; }         // Represents "symbol"
        public string? Side { get; set; }           // Represents "side"
        public int? OrderQty { get; set; }          // Represents "orderQty"
        public decimal? Price { get; set; }        // Represents "price" 
        public string? Currency { get; set; }       // Represents "currency" 
        public string? OrdType { get; set; }        // Represents "ordType"
        public string? TimeInForce { get; set; }    // Represents "timeInForce"
        public string? OrdStatus { get; set; }      // Represents "ordStatus" 
        public decimal? AvgPx { get; set; }         // Represents "avgPx" 
        public DateTime? TransactTime { get; set; } // Represents "transactTime"
        public DateTime? Timestamp { get; set; }    // Represents "timestamp"
    }
}
