using BitmexGUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Services.Interfaces
{
    internal interface IPrice
    {
        public  void GetPriceREST();

        
        public void GetPriceREST(ObservableCollection<CandlestickData> PriceData, Dictionary<string, CandlestickData> priceDataDictionary);

        public  void GetPriceWSS();
    }
}
