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

        private readonly string BinanceEndpointRest = ConfigurationManager.AppSettings["BaseRESTBinance"];
        private readonly string BinanceEndpointWss = ConfigurationManager.AppSettings["BaseWSSBinance"];
        private readonly string BitmexEndpointRest = ConfigurationManager.AppSettings["BaseRESTBitmex"];
        private readonly string BitmexEndpointWss = ConfigurationManager.AppSettings["BaseWSSBitmex"];
        private readonly string Instrument = ConfigurationManager.AppSettings["Instrument"];
        public event Action PriceDataUpdated;
        public event Action SettledPriceDataUpdated;
        private ObservableCollection<CandlestickData> _priceData = new ObservableCollection<CandlestickData>();
        private ObservableCollection<SettledPrice> _settledPriceData = new ObservableCollection<SettledPrice>();
        private Dictionary<string, CandlestickData> _priceDataDictionary = new Dictionary<string, CandlestickData>();
    
        private readonly int _maxCandlesLoading;
        private readonly BinanceAPIPrice BinanceApi;
        private readonly BitmexAPIPrice BitmexApi;
        private string TimeFrame= ConfigurationManager.AppSettings["Timeframe"];
       

        public ObservableCollection<CandlestickData> PriceData
        {
            get => _priceData;
            set
            {
                _priceData = value;
                OnPropertyChanged(nameof(PriceData));
            }
        }
        public ObservableCollection<SettledPrice> SettledPriceData
        {
            get => _settledPriceData;
            set
            {
                _settledPriceData = value;
                OnPropertyChanged(nameof(SettledPriceData));
            }
        }




        public MainViewModel(int InitialCandlesNumber)
        {


            int.TryParse(ConfigurationManager.AppSettings["MaxCandles"],out _maxCandlesLoading);

            
            BinanceEndpointRest += $"/klines?symbol={Instrument}&interval={TimeFrame}&limit={InitialCandlesNumber}";
            BinanceEndpointWss += $"{Instrument.ToLower()}@kline_{TimeFrame}";
            BitmexEndpointWss += $"?subscribe=instrument:XBTUSDT";
            //MessageBox.Show(BinanceEndpointRest);
            BinanceApi = new BinanceAPIPrice(IdBinance, ApiKeyBinance, BinanceEndpointRest, BinanceEndpointWss);
            BinanceApi.GetPriceREST(PriceData,_priceDataDictionary);
            BinanceApi.PriceUpdated += OnPriceUpdatedBinance;

            BitmexApi = new BitmexAPIPrice(IdBitmex, ApiKeyBitmex, BitmexEndpointRest, BitmexEndpointWss);

            BitmexApi.SettledPriceUpdated += OnPriceUpdatedBitmex;

        }

        public void StartPriceFeed()
        {
            BinanceApi.GetPriceWSS();
            BitmexApi.GetPriceWSS();
        }
        private void OnPriceUpdatedBinance(CandlestickData priceData)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {


                var timestamp = priceData.Timestamp;



                if (_priceDataDictionary.ContainsKey(timestamp.ToString()) && priceData != null)
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
        private void OnPriceUpdatedBitmex(SettledPrice setpriceData)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {


                var timestamp = setpriceData.Timestamp;
                 
                SettledPriceData.Add(setpriceData);

                SettledPriceDataUpdated?.Invoke();  
            });
        }

        public void UpdateInitialCandles(int newInitialCandlesNumber)
        {

            BinanceApi.UpdateRestEndpoint($"https://api.binance.com/api/v3/klines?symbol=BTCUSDT&interval={TimeFrame}&limit={newInitialCandlesNumber}");

            //MessageBox.Show(_binanceApi.UrlRest);

            PriceData = new ObservableCollection<CandlestickData>();

            _priceDataDictionary=new Dictionary<string,CandlestickData>();

             

            for (int i = BinanceApi.CachedPriceData.Count- newInitialCandlesNumber; i < BinanceApi.CachedPriceData.Count;i++)
            {
                PriceData.Add(BinanceApi.CachedPriceData[i]);
                _priceDataDictionary.TryAdd(BinanceApi.CachedPriceData[i].Timestamp.ToString(), BinanceApi.CachedPriceData[i]);
            }
            

            //_binanceApi.GetPriceREST(PriceData,_priceDataDictionary); 
        }

      
        
       

        
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
