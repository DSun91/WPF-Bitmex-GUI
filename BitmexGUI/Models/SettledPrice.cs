using BitmexGUI.Interfaces;

namespace BitmexGUI.Models
{
     
    public class SettledPrice: ISettledPrice
    {
        public string Symbol { get; set; }
        public double SettledPriceValue { get; set; } 
        public string Timestamp { get; set; }

    }
 
}
