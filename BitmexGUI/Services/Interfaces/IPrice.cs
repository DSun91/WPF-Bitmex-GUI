using BitmexGUI.Models;
using System.Collections.ObjectModel;

namespace BitmexGUI.Services.Interfaces
{
    internal interface IPrice
    {
        public void GetPriceREST();


        public void GetPriceREST(ObservableCollection<CandlestickData> PriceData, Dictionary<string, CandlestickData> priceDataDictionary);

        public void GetPriceWSS();
    }
}
