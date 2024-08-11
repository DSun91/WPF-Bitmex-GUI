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
        public event Action PriceDataUpdated;
        private ObservableCollection<PriceData> _priceData;
        private Dictionary<string, PriceData> _priceDataDictionary;
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
            _priceDataDictionary = new Dictionary<string, PriceData>();
            _binanceApi = new BinanceAPIPrice(ID, APIKEY, BinanceEndpointRest, BinanceEndpointWss);
            _binanceApi.PriceUpdated += OnPriceUpdated;
        }

        private void OnPriceUpdated(PriceData priceData)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var timestamp = priceData.Timestamp;

                if (_priceDataDictionary.ContainsKey(timestamp.ToString()))
                {
                    // Update existing entry
                    var existingData = _priceDataDictionary[timestamp.ToString()];
                    // Assuming PriceData has properties like Open, High, Low, Close
                    existingData.Open = priceData.Open;
                    existingData.High = priceData.High;
                    existingData.Low = priceData.Low;
                    existingData.Close = priceData.Close;

                    // Notify collection that an item has been updated
                    var index = PriceData.IndexOf(existingData);
                    if (index >= 0)
                    {
                        PriceData[index] = existingData; // Update the item in the ObservableCollection
                    }
                }
                else
                {
                    // Add new entry
                    _priceDataDictionary[timestamp.ToString()] = priceData;
                    PriceData.Add(priceData);
                }

                PriceDataUpdated?.Invoke(); // Trigger the event
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
