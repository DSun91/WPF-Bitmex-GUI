using BitmexGUI.Interfaces;

namespace BitmexGUI.Models
{
    public class Instrument : IInstrument
    {
        public string CurrencyName { get; set; }

        public double MakerFee { get; set; }

        public double TakerFee { get; set; }
    }



}
