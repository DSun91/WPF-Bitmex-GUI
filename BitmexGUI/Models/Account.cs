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
        public float Commission { get; set; }
        public float InitMarginReq { get; set; }
        public float MaintMarginReq { get; set; }
        public long RiskLimit { get; set; }
        public float Leverage { get; set; }
        public bool CrossMargin { get; set; }
        public float DeleveragePercentile { get; set; }
        public long RebalancedPnl { get; set; }
        public long PrevRealisedPnl { get; set; }
        public long PrevUnrealisedPnl { get; set; }
        public long OpeningQty { get; set; }
        public long OpenOrderBuyQty { get; set; }
        public long OpenOrderBuyCost { get; set; }
        public long OpenOrderBuyPremium { get; set; }
        public long OpenOrderSellQty { get; set; }
        public long OpenOrderSellCost { get; set; }
        public long OpenOrderSellPremium { get; set; }
        public long CurrentQty { get; set; }
        public long CurrentCost { get; set; }
        public long CurrentComm { get; set; }
        public long RealisedCost { get; set; }
        public long UnrealisedCost { get; set; }
        public long GrossOpenPremium { get; set; }
        public bool IsOpen { get; set; }
        public float MarkPrice { get; set; }
        public long MarkValue { get; set; }
        public long RiskValue { get; set; }
        public float HomeNotional { get; set; }
        public float ForeignNotional { get; set; }
        public string PosState { get; set; }
        public long PosCost { get; set; }
        public long PosCross { get; set; }
        public long PosComm { get; set; }
        public long PosLoss { get; set; }
        public long PosMargin { get; set; }
        public long PosMaint { get; set; }
        public long PosInit { get; set; }
        public long InitMargin { get; set; }
        public long MaintMargin { get; set; }
        public long RealisedPnl { get; set; }
        public long UnrealisedPnl { get; set; }
        public float UnrealisedPnlPcnt { get; set; }
        public float UnrealisedRoePcnt { get; set; }
        public float AvgCostPrice { get; set; }
        public float AvgEntryPrice { get; set; }
        public float BreakEvenPrice { get; set; }
        public float MarginCallPrice { get; set; }
        public float LiquidationPrice { get; set; }
        public float BankruptPrice { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
