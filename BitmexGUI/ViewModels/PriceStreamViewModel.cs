using BitmexGUI.BaseClasses;
using BitmexGUI.Models;
using BitmexGUI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BitmexGUI.ViewModels
{
    public class PriceStreamViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<CandleStickViewModel> _priceData = new ObservableCollection<CandleStickViewModel>(); 

        private ObservableCollection<CandleStickViewModel> _scaledpriceData = new ObservableCollection<CandleStickViewModel>();

        private ObservableCollection<SettledPrice> _settledPriceData = new ObservableCollection<SettledPrice>();

        public List<CandleStickViewModel> CachedScaledCandlesticks = new List<CandleStickViewModel>();

        private ObservableCollection<CurrentClosePrice> _currentClose = new ObservableCollection<CurrentClosePrice>();

        public  Action ScaledPriceUpdated;

        public PriceStreamViewModel()
        {
            CachedScaledCandlesticks.Clear();
            PriceData.Clear();
            ScaledPriceData.Clear();
            InitializedScaledData();
            InitializedCachedCandlesticks();
        }   

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        

        #region Properties

        public ObservableCollection<CurrentClosePrice> CurrentClose
        {
            get => _currentClose;
            set
            {
                _currentClose = value;
                OnPropertyChanged(nameof(CurrentClose));
            }
        }


        public ObservableCollection<CandleStickViewModel> ScaledPriceData
        {
            get => _scaledpriceData;
            set
            {
                _scaledpriceData = value;
                OnPropertyChanged(nameof(ScaledPriceData));
            }
        }
        public ObservableCollection<CandleStickViewModel> PriceData
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

        
        #endregion
        private void InitializedCachedCandlesticks()
        {
             
            for (int i = 0; i < OHLCandlestickChart.TotalCandlesCount; i++)
            {
                CachedScaledCandlesticks.Add(new CandleStickViewModel(new CandlestickData()));

            }
        }
        private void InitializedScaledData()
        {
       
            for (int i = 0; i < OHLCandlestickChart.CandlesToView; i++)
            {
                ScaledPriceData.Add(new CandleStickViewModel( new CandlestickData()));

            }

        }

        private void AppendPriceData(DateTime timestamp, CandlestickData priceData)
        {
            if (MainViewModel._priceDataDictionary.ContainsKey(timestamp.ToString()))
            {

                // Update existing entry
                var existingData = MainViewModel._priceDataDictionary[timestamp.ToString()];
                // Assuming PriceData has properties like Open, High, Low, Close
                existingData.Open = priceData.Open;
                existingData.High = priceData.High;
                existingData.Low = priceData.Low;
                existingData.Close = priceData.Close;

                
                int index = PriceData.ToList().FindIndex(x=>x.Candlestick.Timestamp.Equals(existingData.Timestamp));

                if (index >= 0)
                { 
                    PriceData[index] =new CandleStickViewModel(existingData);  
                }
            }
        }
        private void AppendScaledPriceData(DateTime timestamp, CandlestickData priceData)
        {
            var ScaledPriceExisting = ScaledPriceData.FirstOrDefault(data => data.Timestamp.Equals(timestamp));
            int indexScaledPrice = ScaledPriceData.IndexOf(ScaledPriceExisting);
            if (indexScaledPrice > 0)
            {
                ScaledPriceData[indexScaledPrice] =new CandleStickViewModel( ScaleCandle(priceData));
                ScaledPriceData[indexScaledPrice].Posx = ScaledPriceExisting.Posx;
            }

        }

        // This Method is executed to add new Candle to Copy of Original
        // after last on close so get executed once each timeframe
        private void AddNewPriceData(DateTime timestamp, CandlestickData priceData)
        {
            MainViewModel._priceDataDictionary[timestamp.ToString()] = priceData;
            priceData.Posx = OHLCandlestickChart.CandlesInterspace * PriceData.Count;

            PriceData.Add(new CandleStickViewModel(priceData));

            if (PriceData.Count > OHLCandlestickChart.TotalCandlesCount)
            {
                PriceData.Remove(PriceData.First());

                for (int i = 0; i < PriceData.Count; i++)
                {
                    PriceData[i].Posx = OHLCandlestickChart.CandlesInterspace * i;
                     
                }
            }
            
        }


        // This Method is executed to add new Candle to scaled prices
        // after last on close so get executed once each timeframe
        private void AddNewScaledPriceData(DateTime timestamp, CandlestickData priceData)
        {
            MainViewModel._priceDataDictionary[timestamp.ToString()] = priceData;
            priceData.Posx = OHLCandlestickChart.CandlesInterspace * PriceData.Count;

            ScaledPriceData.Add(new CandleStickViewModel(ScaleCandle(priceData)));

            if (ScaledPriceData.Count > OHLCandlestickChart.CandlesToView)
            {
                ScaledPriceData.Remove(ScaledPriceData.First());

                for (int i = 0; i < ScaledPriceData.Count; i++)
                {
                    ScaledPriceData[i].Posx = OHLCandlestickChart.CandlesInterspace * i;
                }
            }
            CachedScaledCandlesticks = ScaledPriceData.ToList();
        }

        // Gets alle each time the live price stream is updated from the websocket
        public void OnPriceUpdatedBinance(CandlestickData priceData)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                RefreshScaledPriceData("FromPriceFeed");

                if (ScaledPriceData.Count ==0)
                {
                    RefreshScaledPriceData("InitializedNewChart");
                }
                var timestamp = priceData.Timestamp;

                CurrentClosePrice crt = new CurrentClosePrice
                {
                    PriceValue = priceData.Close,
                    Symbol = priceData.Symbol,

                };

                CurrentClose.Clear();// Used to update live market price ticker
                CurrentClose.Add(crt);// Used to update live market price ticker
                //MessageBox.Show(CurrentClose[0].ToString());

                if (MainViewModel._priceDataDictionary.ContainsKey(timestamp.ToString()) && priceData != null)
                {

                    AppendPriceData(timestamp, priceData);
                    AppendScaledPriceData(timestamp, priceData);

                }
                else
                {
                    // Add new entry
                    AddNewPriceData(timestamp, priceData);
                    AddNewScaledPriceData(timestamp, priceData);

                }
               

            });
        }



        // Ticker for the settled price from bitmex
        public void OnPriceUpdatedBitmex(SettledPrice setpriceData)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {

                var timestamp = setpriceData.Timestamp;
                SettledPriceData.Clear();

                SettledPriceData.Add(setpriceData);
                 
            });
        }

         
        

        // this is called each time the live price stream is updated from the websocket
        public void RefreshScaledPriceData(string FromEvenFeed)
        { 
            ScalePriceData(FromEvenFeed);

            ScaledPriceUpdated?.Invoke();
            
        }
       
        public void ScalePriceData(string FromEvenFeed)
        {
            int CandlesToSkip = OHLCandlestickChart.TotalCandlesCount - OHLCandlestickChart.CandlesToView;

            if (FromEvenFeed == "FromPriceFeed" )
            {

                // this is called each time the live price stream is updated from the websocket after last price is added
                // it compares the cached scaled candles with the current ones and updates only those that changed
                for (int i = 0; i < ScaledPriceData.Count; i++)
                {
                    if(CachedScaledCandlesticks[i].Close != 0)
                    {
                        if (CachedScaledCandlesticks[i].Close != ScaledPriceData[i].Close)
                        {

                            CandlestickData scaledCandle = ScaleCandle(PriceData[i + CandlesToSkip].Candlestick);

                            ScaledPriceData[i].Open = scaledCandle.Open;
                            ScaledPriceData[i].Close = scaledCandle.Close;
                            ScaledPriceData[i].High = scaledCandle.High;
                            ScaledPriceData[i].Low = scaledCandle.Low;
                            ScaledPriceData[i].Posx = i * OHLCandlestickChart.CandlesInterspace;
                            ScaledPriceData[i].Width = OHLCandlestickChart.candleWidth;
                            ScaledPriceData[i].Symbol = scaledCandle.Symbol;
                            ScaledPriceData[i].Timestamp = scaledCandle.Timestamp;
                        }
                    }
                    else
                    {   // Initial start of symbol all Close are zero so reloading whole scaled price data
                        ReloadAllScaledPriceScaled(CandlesToSkip);
                    }
                    

                }
                
                CachedScaledCandlesticks = ScaledPriceData.ToList();
            }
            else
            {
                // this is called when mouse event fires
                ReloadAllScaledPriceScaled(CandlesToSkip);
            }
            
        }


        public void ReloadAllScaledPriceScaled(int ToSkip)
        {
            // Actual Loop that Clears and Scales all the priceStreamViewModel.ScaledPriceData list of candles in case events like
            // dragging and mousewheel are triggered from LivePriceChart
            // in this case there is priceStreamViewModel.ScaledPriceData.Clear() so it reload ScaledPriceData completely
            // after scaling and resizing

            ScaledPriceData.Clear();

            for (int i = ToSkip; i < PriceData.Count; i++)
            {

                CandlestickData scaledCandle = ScaleCandle(PriceData[i].Candlestick);
                scaledCandle.Posx = (i - ToSkip) * OHLCandlestickChart.CandlesInterspace;
                scaledCandle.Width = OHLCandlestickChart.candleWidth;

                ScaledPriceData.Add(new CandleStickViewModel( scaledCandle));

            }
            CachedScaledCandlesticks = ScaledPriceData.ToList();
        }

         
        public CandlestickData ScaleCandle(CandlestickData priceData)
        {
            CandlestickData temp = new CandlestickData();
            try
            {
                int maxCandlesInView = (int)Math.Ceiling(700 / (OHLCandlestickChart.CandlesInterspace - OHLCandlestickChart.candleWidth));
                var allValues = PriceData.Skip(OHLCandlestickChart.TotalCandlesCount - OHLCandlestickChart.CandlesToView).Take(OHLCandlestickChart.TotalCandlesCount).SelectMany(data => new[] { data.Open, data.High, data.Low, data.Close });
                var minVal = allValues.Min();
                var maxVal = allValues.Max();


                OHLCandlestickChart.minOriginal = minVal;
                OHLCandlestickChart.maxOriginal = maxVal;
                double padding = (OHLCandlestickChart.maxOriginal - OHLCandlestickChart.minOriginal) * OHLCandlestickChart.ScaleFactor;
                OHLCandlestickChart.minOriginal -= padding;
                OHLCandlestickChart.maxOriginal += padding;


                temp.Open = OHLCandlestickChart.MapToScale(priceData.Open) + OHLCandlestickChart.VerticalOffset;
                temp.High = OHLCandlestickChart.MapToScale(priceData.High) + OHLCandlestickChart.VerticalOffset;
                temp.Low = OHLCandlestickChart.MapToScale(priceData.Low) + OHLCandlestickChart.VerticalOffset;
                temp.Close = OHLCandlestickChart.MapToScale(priceData.Close) + OHLCandlestickChart.VerticalOffset;
                temp.Timestamp = priceData.Timestamp;
                temp.Width = priceData.Width;
                temp.Posx = priceData.Posx;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex + " " + ex.StackTrace);
            }
            return temp;
        }



    }
}
