namespace BitmexGUI.Interfaces
{
    internal interface IInstrument
    {
        public string CurrencyName { get; set; }

        public double MakerFee { get; set; }

        public double TakerFee { get; set; }

    }
}
