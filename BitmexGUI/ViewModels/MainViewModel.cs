using BitmexGUI.Models;
using BitmexGUI.Services.Implementations;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;
using System.Configuration;

namespace BitmexGUI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly string IdBinance = "";
        private readonly string ApiKeyBinance = "";
        private readonly string IdBitmex = "";
        private readonly string ApiKeyBitmex = "";

        private readonly string BinanceEndpointRest = "";
        private readonly string BinanceEndpointWss = "";
        private readonly string BitmexEndpointRest = "https://www.bitmex.com/api/v1";
        private readonly string BitmexEndpointWss = "wss://ws.bitmex.com/realtime";
        public event Action PriceDataUpdated;
        private ObservableCollection<PriceData> _priceData = new ObservableCollection<PriceData>();
        private Dictionary<string, PriceData> _priceDataDictionary = new Dictionary<string, PriceData>();
    
        private readonly int _maxCandlesLoading;
        private readonly BinanceAPIPrice _binanceApi;
        private readonly BitmexAPIPrice _bitmexApi;
        private string TimeFrame= ConfigurationManager.AppSettings["Timeframe"];
       

        public ObservableCollection<PriceData> PriceData
        {
            get => _priceData;
            set
            {
                _priceData = value;
                OnPropertyChanged(nameof(PriceData));
            }
        }
        
     
        
        public MainViewModel(int InitialCandlesNumber)
        {


            int.TryParse(ConfigurationManager.AppSettings["MaxCandles"],out _maxCandlesLoading);

            
            BinanceEndpointRest = $"https://api.binance.com/api/v3/klines?symbol=BTCUSDT&interval={TimeFrame}&limit={InitialCandlesNumber}";
            BinanceEndpointWss = $"wss://stream.binance.com:443/ws/btcusdt@kline_{TimeFrame}";
            _binanceApi = new BinanceAPIPrice(IdBinance, ApiKeyBinance, BinanceEndpointRest, BinanceEndpointWss);
            _bitmexApi = new BitmexAPIPrice(IdBitmex, ApiKeyBitmex, BitmexEndpointRest, BitmexEndpointWss);

            _binanceApi.GetPriceREST(PriceData,_priceDataDictionary);

            //MessageBox.Show("hrtr");

            _binanceApi.PriceUpdated += OnPriceUpdated;


        }

        

        public void UpdateInitialCandles(int newInitialCandlesNumber)
        {
            
            _binanceApi.UpdateRestEndpoint($"https://api.binance.com/api/v3/klines?symbol=BTCUSDT&interval={TimeFrame}&limit={newInitialCandlesNumber}");

            //MessageBox.Show(_binanceApi.UrlRest);

            PriceData = new ObservableCollection<PriceData>();

            _priceDataDictionary=new Dictionary<string,PriceData>();

             

            for (int i = _binanceApi.CachedPriceData.Count- newInitialCandlesNumber; i < _binanceApi.CachedPriceData.Count;i++)
            {
                PriceData.Add(_binanceApi.CachedPriceData[i]);
                _priceDataDictionary.TryAdd(_binanceApi.CachedPriceData[i].Timestamp.ToString(), _binanceApi.CachedPriceData[i]);
            }
            

            //_binanceApi.GetPriceREST(PriceData,_priceDataDictionary); 
        }

      
        
        private void OnPriceUpdated(PriceData priceData)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                

                var timestamp = priceData.Timestamp;

             
                
                if (_priceDataDictionary.ContainsKey(timestamp.ToString()) && priceData!=null)
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

                    while (PriceData.Count > _maxCandlesLoading)
                    {
                        PriceData.Remove(PriceData.First());
                    }
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
