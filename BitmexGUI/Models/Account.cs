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



}
