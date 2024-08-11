using BitmexGUI.Models;
using BitmexGUI.Services.Implementations;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly string ID = "";
        private readonly string APIKEY = "";
        private readonly string BinanceEndpointRest = "https://www.bitmex.com/api/v1/instrument?symbol=XBTUSDT&timeframe=nearest&count=1&reverse=false";
        private readonly string BinanceEndpointWss = "wss://stream.binance.com:443/ws/btcusdt@kline_1m";

        private ObservableCollection<PriceData> _priceData;
       
        public ObservableCollection<PriceData> PriceData
        {
            get => _priceData;
            set
            {
                _priceData = value;
                OnPropertyChanged(nameof(PriceData));
            }
        }

        private readonly BinanceAPIPrice _binanceApi;

        public MainViewModel()
        {
            _priceData = new ObservableCollection<PriceData>();
            _binanceApi = new BinanceAPIPrice(ID, APIKEY, BinanceEndpointRest, BinanceEndpointWss);
            _binanceApi.PriceUpdated += OnPriceUpdated;
        }

        private void OnPriceUpdated(PriceData priceData)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                PriceData.Add(priceData);
            });
        }

        public void StartPriceFeed()
        {
            _binanceApi.GetPriceWSS();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
