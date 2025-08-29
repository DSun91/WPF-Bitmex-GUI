using BitmexGUI.Services.Interfaces;

namespace BitmexGUI.Models
{
    public class Account : IAccount
    {
        public string ID { get; set; }

        public double Balance { get; set; }

        public string CurrencyName { get; set; }

        public virtual void GetWallet()
        {

        }

    }

}
