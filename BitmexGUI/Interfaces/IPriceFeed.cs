using BitmexGUI.Models;
using BitmexGUI.ViewModels;
using System.Collections.ObjectModel;

namespace BitmexGUI.Interfaces
{
    internal interface IPriceFeed
    {
        public void GetPriceREST();
         
        public void GetPriceREST(ObservableCollection<CandleStickViewModel> PriceData, Dictionary<string, CandlestickData> priceDataDictionary);

        public void GetPriceWSS(string name);
    }
}
