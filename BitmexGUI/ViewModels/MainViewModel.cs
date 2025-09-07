using BitmexGUI.BaseClasses;
using BitmexGUI.Models;
using BitmexGUI.ViewModels.Indicators;
using BitmexGUI.ViewModels.Utilities;
using BitmexGUI.Views;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace BitmexGUI.ViewModels
{

    public class MainViewModel : MainViewModelBase
    {
        private string IdBinance = "";
        private string ApiKeyBinance = "";
        private string IdBitmex = "";
        private string ApiKeyBitmex = "";

        private string BinanceEndpointRest;
        private string BinanceEndpointWss;
        private string BitmexEndpointRest;
        private string BitmexEndpointWss;  
         
        public static Dictionary<string, CandlestickData> _priceDataDictionary = new Dictionary<string, CandlestickData>();

        private BinanceAPI _binanceApi;
        private BitmexAPI _bitmexApi; 
         
        public EntryViewModel entryViewModel { get; set; }  
        public SymbolSelectionViewModel symbolSelectionViewModel { get; set; }
        public OrdersViewModel ordersViewModel { get; set; } 
        public PriceStreamViewModel priceStreamViewModel { get; set; }
        public PositionsViewModel positionsViewModel { get; set; } 
        public IndicatorsViewModelManager indicatorsViewModelManager { get; set; }

        private Stopwatch _stopwatch = new Stopwatch();

        public MainViewModel()
        {
            
            symbolSelectionViewModel = new SymbolSelectionViewModel();
          
            var BinanceInstrument = symbolSelectionViewModel.SelectedTicker.ToString().Equals("BTCUSD") ? "BTCUSDT" : symbolSelectionViewModel.SelectedTicker.ToString();
            var TimeFrame = symbolSelectionViewModel.SelectedTimeFrame.ToString();
            var BitmexInstrument = SymbolSelectionViewModel.ExchangeTickersMap[symbolSelectionViewModel.SelectedTicker.ToString()];

            SetEndpoints(OHLCandlestickChart.TotalCandlesCount, BinanceInstrument, TimeFrame, BitmexInstrument);


            _binanceApi = new BinanceAPI(IdBinance, ApiKeyBinance, BinanceEndpointRest, BinanceEndpointWss);

            _bitmexApi = new BitmexAPI(IdBitmex, ApiKeyBitmex, BitmexEndpointRest, BitmexEndpointWss);

            entryViewModel = new EntryViewModel(_bitmexApi);

            priceStreamViewModel = new PriceStreamViewModel();

            indicatorsViewModelManager= new IndicatorsViewModelManager(priceStreamViewModel);

            ordersViewModel = new OrdersViewModel(_bitmexApi, symbolSelectionViewModel, entryViewModel, priceStreamViewModel); 

            positionsViewModel = new PositionsViewModel(_bitmexApi);

             
            symbolSelectionViewModel.SymbolOrTimeFrameSelected+= RefreshDataContext;
            
            
            StartPriceFeed();
             
            StartMaintainOrdersPositionsWSS();

        }
        


        private void SetEndpoints(int InitialCandlesNumber, string BinanceInstrument, string TimeFrame, string BitmexInstrument)
        {
            BinanceEndpointRest = ConfigurationManager.AppSettings["BaseRESTBinance"] + $"/klines?symbol={BinanceInstrument}&interval={TimeFrame}&limit={InitialCandlesNumber}";
            BinanceEndpointWss = ConfigurationManager.AppSettings["BaseWSSBinance"] + $"{BinanceInstrument.ToLower()}@kline_{TimeFrame}";

            BitmexEndpointRest = ConfigurationManager.AppSettings["BaseBitmexUrl"] + ConfigurationManager.AppSettings["BaseRESTBitmex"];
            BitmexEndpointWss = ConfigurationManager.AppSettings["BaseWSSBitmex"] + $"?subscribe=instrument:{BitmexInstrument}";
        }
        public void StartPriceFeed()
        {
            _bitmexApi.SettledPriceUpdated += priceStreamViewModel.OnPriceUpdatedBitmex;
            _binanceApi.PriceUpdated += priceStreamViewModel.OnPriceUpdatedBinance;
            _stopwatch.Start();
            _binanceApi.GetPriceREST(priceStreamViewModel.PriceData, _priceDataDictionary);
            _stopwatch.Stop();
            //MessageBox.Show($"Binance REST API call completed in {_stopwatch.ElapsedMilliseconds} ms."); 
            _binanceApi.GetPriceWSS("BinancePriceFeed"); 
            _bitmexApi.GetPriceWSS("BitmexPriceFeed"); 
        }
        public void StartMaintainOrdersPositionsWSS()
        {
            _bitmexApi.GetOrdersWSS(); 
            _bitmexApi.GetPositionWSS(); 
        }
         
         
        public async void RefreshDataContext()
        {
            try
            {
                var BinanceInstrument = symbolSelectionViewModel.SelectedTicker.ToString().Equals("BTCUSD") ? "BTCUSDT" : symbolSelectionViewModel.SelectedTicker.ToString();
                var TimeFrame = symbolSelectionViewModel.SelectedTimeFrame.ToString();
                var BitmexInstrument = SymbolSelectionViewModel.ExchangeTickersMap[symbolSelectionViewModel.SelectedTicker.ToString()];

                if (TimeFrame != null && BitmexInstrument != null)
                {
                    priceStreamViewModel.PriceData.Clear();
                    priceStreamViewModel.ScaledPriceData.Clear();
                    priceStreamViewModel.SettledPriceData.Clear();
                    priceStreamViewModel.CachedScaledCandlesticks.Clear();  



                    await WebSocketManager.Instance.CloseAllWebSocketsAsync();

                    SetEndpoints(OHLCandlestickChart.TotalCandlesCount, BinanceInstrument, TimeFrame, BitmexInstrument);


                    _binanceApi = new BinanceAPI(IdBinance, ApiKeyBinance, BinanceEndpointRest, BinanceEndpointWss);
                    

                    _bitmexApi = new BitmexAPI(IdBitmex, ApiKeyBitmex, BitmexEndpointRest, BitmexEndpointWss);
                 

                    entryViewModel.RefreshWalletInfo();

                    positionsViewModel = new PositionsViewModel(_bitmexApi);

                    
                    StartPriceFeed();

                    StartMaintainOrdersPositionsWSS();

                }

            }
            catch (Exception ex)
            {
                Debugger.Break();
                //MessageBox.Show("Error initializing view model: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


         
        private ICommand _createPatternRecognitionWindow;
        public ICommand CreatePatternRecognitionWindow
        {
            get
            {
                if (_createPatternRecognitionWindow == null)
                {
                    _createPatternRecognitionWindow = new RelayCommand(param =>
                    {
                       
                    });
                }
                return _createPatternRecognitionWindow;
            }
        }

    }
}
