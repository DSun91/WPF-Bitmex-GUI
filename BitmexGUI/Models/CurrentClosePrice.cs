using BitmexGUI.Interfaces;

namespace BitmexGUI.Models
{
     
    public class CurrentClosePrice: IPriceTicker
    {
        public string Symbol { get; set; }
        public double PriceValue { get; set; } 
        public string Timestamp { get; set; }

    }
     
}
