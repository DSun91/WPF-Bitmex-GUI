using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Models
{
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
        public float? RebalancedPnl { get; set; }
        public long? PrevRealisedPnl { get; set; }
        public long? PrevUnrealisedPnl { get; set; }
        public long? OpeningQty { get; set; }
        public long? OpenOrderBuyQty { get; set; }
        public long? OpenOrderBuyCost { get; set; }
        public long? OpenOrderBuyPremium { get; set; }
        public long? OpenOrderSellQty { get; set; }
        public long? OpenOrderSellCost { get; set; }
        public long? OpenOrderSellPremium { get; set; }
        public float? CurrentQty { get; set; }
        public float? CurrentCost { get; set; }
        public long CurrentComm { get; set; }
        public float? RealisedCost { get; set; }
        public float? UnrealisedCost { get; set; }
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
        public float? RealisedPnl { get; set; }
        public float? UnrealisedPnl { get; set; }
        public float? UnrealisedPnlPcnt { get; set; }
        public float? UnrealisedRoePcnt { get; set; }
        public float? AvgCostPrice { get; set; }
        public float? AvgEntryPrice { get; set; }
        public float? BreakEvenPrice { get; set; }
        public float? MarginCallPrice { get; set; }
        public float? LiquidationPrice { get; set; }
        public float? BankruptPrice { get; set; }

        public float? DeltaEntryCurrent => (float)Math.Round((float)AvgEntryPrice - (float)MarkPrice);
        public DateTime Timestamp { get; set; }
    }

}
