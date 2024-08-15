using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Models
{
    public class Account
    {
        public string ID { get; set; }

        public double Balance { get; set; }

        public string CurrencyName { get; set; }
    }

    public class Instrument
    {
        public string CurrencyName { get; set; }

        public double makerFee { get; set; }

        public double takerFee { get; set; }
    }

    public class Position
    {
        public int AccountID { get; set; }
        public string Symbol { get; set; }
        public string Currency { get; set; }
        public string Underlying { get; set; }
        public string QuoteCurrency { get; set; }
        public float? Commission { get; set; }
        public float? InitMarginReq { get; set; }
        public float? MaintMarginReq { get; set; }
        public long? RiskLimit { get; set; }
        public float? Leverage { get; set; }
        public bool CrossMargin { get; set; }
        public float? DeleveragePercentile { get; set; }
        public long? RebalancedPnl { get; set; }
        public long? PrevRealisedPnl { get; set; }
        public long? PrevUnrealisedPnl { get; set; }
        public long? OpeningQty { get; set; }
        public long? OpenOrderBuyQty { get; set; }
        public long? OpenOrderBuyCost { get; set; }
        public long? OpenOrderBuyPremium { get; set; }
        public long? OpenOrderSellQty { get; set; }
        public long? OpenOrderSellCost { get; set; }
        public long? OpenOrderSellPremium { get; set; }
        public long? CurrentQty { get; set; }
        public long? CurrentCost { get; set; }
        public long CurrentComm { get; set; }
        public long? RealisedCost { get; set; }
        public long? UnrealisedCost { get; set; }
        public long? GrossOpenPremium { get; set; }
        public bool IsOpen { get; set; }
        public float? MarkPrice { get; set; }
        public long? MarkValue { get; set; }
        public long? RiskValue { get; set; }
        public float? HomeNotional { get; set; }
        public float? ForeignNotional { get; set; }
        public string PosState { get; set; }
        public long? PosCost { get; set; }
        public long? PosCross { get; set; }
        public long? PosComm { get; set; }
        public long? PosLoss { get; set; } 
        public long? PosMargin { get; set; }
        public long? PosMaint { get; set; }
        public long? PosInit { get; set; }
        public long? InitMargin { get; set; }
        public long? MaintMargin { get; set; }
        public long? RealisedPnl { get; set; }
        public long? UnrealisedPnl { get; set; }
        public float? UnrealisedPnlPcnt { get; set; }
        public float? UnrealisedRoePcnt { get; set; }
        public float? AvgCostPrice { get; set; }
        public float? AvgEntryPrice { get; set; }
        public float? BreakEvenPrice { get; set; }
        public float? MarginCallPrice { get; set; }
        public float? LiquidationPrice { get; set; }
        public float? BankruptPrice { get; set; }
        public DateTime Timestamp { get; set; }
    }

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
