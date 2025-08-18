using BitmexGUI.Models;
using BitmexGUI.Services.Implementations;
using BitmexGUI.ViewModels.Utilities;
using BitmexGUI.Views;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Reflection;
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
        public event Action PriceDataUpdated;
        public event Action SettledPriceDataUpdated;  
        public event Action PositionsdatsUpdated;
        public event Action OpenordersInfoUpdated;
        public event Action HistoricOrderdataUpdated;
        private Dictionary<string, CandlestickData> _priceDataDictionary = new Dictionary<string, CandlestickData>();

        private BinanceAPI BinanceApi;
        private BitmexAPI BitmexApi;
        private string TimeFrame = ConfigurationManager.AppSettings["Timeframe"];
        public Action OrderLineUpdated; 


        

        public EntryViewModel entryViewModel { get; set; } 

        public SymbolSelectionViewModel symbolSelectionViewModel { get; set; }

        public Dictionary<string, string> ExchangeTickersMap = new Dictionary<string, string>
        {
            { "BTCUSDT","XBTUSDT" },
            { "BTCUSD","XBTUSD" },
            { "ETHUSDT","ETHUSDT" }
        };

        public MainViewModel()
        {


            symbolSelectionViewModel= new SymbolSelectionViewModel();
            
          
            var BinanceInstrument = symbolSelectionViewModel.SelectedTicker.ToString().Equals("BTCUSD") ? "BTCUSDT" : symbolSelectionViewModel.SelectedTicker.ToString();
            var TimeFrame = symbolSelectionViewModel.SelectedTimeFrame.ToString();
            var BitmexInstrument = ExchangeTickersMap[symbolSelectionViewModel.SelectedTicker.ToString()];

            SetEndpoints(CandlestickChart.CachedCandles, BinanceInstrument, TimeFrame, BitmexInstrument);

            //MessageBox.Show(BinanceEndpointRest);
            BinanceApi = new BinanceAPI(IdBinance, ApiKeyBinance, BinanceEndpointRest, BinanceEndpointWss);

            

            BinanceApi.GetPriceREST(PriceData, _priceDataDictionary);

            BinanceApi.PriceUpdated += OnPriceUpdatedBinance;



            BitmexApi = new BitmexAPI(IdBitmex, ApiKeyBitmex, BitmexEndpointRest, BitmexEndpointWss); 

            entryViewModel = new EntryViewModel(BitmexApi);

            BitmexApi.SettledPriceUpdated += OnPriceUpdatedBitmex;

            
            BitmexApi.PositionUpdated += OnPositionUpdate;
            BitmexApi.OrderUpdated += OnOrderReceived;
            
            symbolSelectionViewModel.SymbolSelected+= RefreshDataContext;
            //StartPriceFeed();
            //BitmexApi.SetLeverage("XBTUSDT",5.4);

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
            
            BinanceApi.GetPriceWSS();
            BitmexApi.GetPositionWSS();
            BitmexApi.GetPriceWSS();
            BitmexApi.GetOrdersWSS();
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
                        PatternCreator();
                    });
                }
                return _createPatternRecognitionWindow;
            }
        }

        private void PatternCreator()
        {
            //CustomPTR secondWindow = new CustomPTR(25,PriceData);
            //secondWindow.Show();
        }


      


        #region ORDERS_SECTION
        private ICommand _createNewOrderCommand;
        private void ExecuteCreateNewOrder(object param)
        {
            
            string side = param.ToString();
            CreateNewOrder(side);
        }
        public ICommand CreateNewOrderCommand
        {
            get
            {
                if (_createNewOrderCommand == null)
                {
                    _createNewOrderCommand = new RelayCommand(ExecuteCreateNewOrder);
                }
                return _createNewOrderCommand;
            }
        }
        public void CreateNewOrder(string Side)
        {
            
            string orderside = Side.ToLower().Replace(" ", "");

            //MessageBox.Show(MainWindow.ExchangeTickersMap[Symbol]+" "+ Quantity * 1000000);

            if (orderside.Contains("buylimit"))
            {

                BitmexApi.CreateOrder(ExchangeTickersMap[symbolSelectionViewModel.SelectedTicker],
                                      entryViewModel.Quantity * 1000000,
                                       Math.Round(entryViewModel.EntryPrice, 0),
                                       "Limit",
                                       "GoodTillCancel",
                                       "Buy",
                                       entryViewModel.SliderLeverage);
            }

            //else if (orderside.Contains("selllimit"))
            //{

            //    BitmexApi.CreateOrder(ExchangeTickersMap[Symbol],
            //                           Quantity * 1000000,
            //                           Math.Round(EntryPrice, 0),
            //                           "Limit",
            //                           "GoodTillCancel",
            //                           "Sell",
            //                           SliderLeverage);
            //}

            //else if (orderside.Contains("buymarket"))
            //{
            //    BitmexApi.CreateOrder(ExchangeTickersMap[Symbol],
            //                           Quantity * 1000000,
            //                           Math.Round(EntryPrice, 0),
            //                           "Market",
            //                           "ImmediateOrCancel",
            //                           "Buy",
            //                           SliderLeverage);
            //}

            //else if (orderside.Contains("sellmarket"))
            //{
            //    BitmexApi.CreateOrder(ExchangeTickersMap[Symbol],
            //                           -Quantity * 1000000,
            //                           Math.Round(EntryPrice, 0),
            //                           "Market",
            //                           "ImmediateOrCancel",
            //                           "Sell",
            //                           SliderLeverage);
            //}
             
        }
        public void CancelOrder(string OrderID)
        {

            BitmexApi.CancelOrder(OrderID);
        }
        private ObservableCollection<OrderLine> _ordersLinessa=new ObservableCollection<OrderLine>();

        public ObservableCollection<OrderLine> OrdersLinessa
        {
            get => _ordersLinessa;
            set
            {
                _ordersLinessa = value;
                OnPropertyChanged(nameof(OrdersLinessa));
            }
        }
        private void UpdateorderLines(Order newOrderData)
        {


            OrderLine neworderLine = new OrderLine
            {
                OrderID = newOrderData.OrderID,
                Price = (decimal)CandlestickChart.MapToScale(double.Parse(newOrderData.Price.ToString())/10000000),
                Symbol = newOrderData.Symbol,
                Side = newOrderData.Side

            };
            var existingOrderLine = OrdersLines.FirstOrDefault(p => p.OrderID.Equals(newOrderData.OrderID));
            if (existingOrderLine != null)
            {
                OrdersLines.Remove(existingOrderLine);
            }
            OrdersLines.Add(neworderLine); 
            OrderLineUpdated?.Invoke();


        }

        private void RemoveorderLines(Order newOrderData)
        {

            var existingOrderLine = OrdersLines.FirstOrDefault(p => p.OrderID.Equals(newOrderData.OrderID));
            if (existingOrderLine != null)
            {
                OrdersLines.Remove(existingOrderLine);
            } 
        }
        private void OnOrderReceived(Order newOrderData)
        {
            if (newOrderData != null)
            {

                //MessageBox.Show(newOrderData.Price.ToString());
                // Check if OrdStatus is present

                var ordStatusProperty = newOrderData.GetType().GetTypeInfo().GetDeclaredProperty("OrdStatus");

                if (ordStatusProperty != null)
                {
                    // OrdStatus exists and is not null or empty
                    if (!string.IsNullOrEmpty(newOrderData.OrdStatus))
                    {

                        string orderStatus = newOrderData.OrdStatus.ToLower();

                        if (orderStatus.Contains("new") || orderStatus.Contains("partiallyfilled"))
                        {
                            var existingOrder = OrdersInfo.FirstOrDefault(p => p.OrderID.Equals(newOrderData.OrderID));

                            if (existingOrder != null)
                            {
                                OrdersInfo.Remove(existingOrder);
                            }


                            UpdateorderLines(newOrderData);
                            OrdersInfo.Add(newOrderData);
                        }
                        else
                        {
                            var existingOrder = OrdersInfo.FirstOrDefault(p => p.OrderID.Equals(newOrderData.OrderID));
                            if (existingOrder != null)
                            {
                                RemoveorderLines(existingOrder);
                                OrdersInfo.Remove(existingOrder);
                            }
                        }
                    }
                    else
                    {

                        // OrdStatus does not exist, so update the existing order
                        var existingOrder = OrdersInfo.FirstOrDefault(p => p.OrderID.Equals(newOrderData.OrderID));

                        if (existingOrder != null)
                        {

                            Order TempOrder = new Order();

                            int index = OrdersInfo.IndexOf(existingOrder);
                            // Iterate over all properties of newOrderData and update existingOrder with differing values
                            foreach (var prop in typeof(Order).GetProperties())
                            {
                                var newValue = prop.GetValue(newOrderData);
                                var existingValue = prop.GetValue(existingOrder);

                                // Only update the property if the new value is different and not null
                                if (newValue != null && !newValue.Equals(existingValue))
                                {
                                    prop.SetValue(TempOrder, newValue);
                                }
                                else
                                {
                                    prop.SetValue(TempOrder, existingValue);
                                }

                            }
                            OrdersInfo[index] = TempOrder;
                            UpdateorderLines(TempOrder);
                        }
                        else
                        {
                            // If no existing order is found, add the new order to OrdersInfo
                            OrdersInfo.Add(newOrderData);
                            UpdateorderLines(newOrderData);
                        }
                    }
                }
                 
                // Always update historic orders
                HistoricOrdersInfo.Add(newOrderData);
            }
        }

        private void UpdateOrderLinesRescale(ObservableCollection<Order> Orderinfos)
        {
            OrdersLines.Clear();

            for (int j = 0; j < Orderinfos.Count; j++)
            {
                var Price = CandlestickChart.MapToScale((double)Orderinfos[j].Price);

                OrderLine tempOrdlIne = new OrderLine
                {
                    OrderID = Orderinfos[j].OrderID,
                    Price = (decimal)Price,
                    Side = Orderinfos[j].Side,
                    Symbol = Orderinfos[j].Symbol
                };
                OrdersLines.Add(tempOrdlIne);

            }
        }


        private CandlestickData ScaleCandle(CandlestickData priceData)
        {
            CandlestickData temp = new CandlestickData();
            try
            {
                int maxCandlesInView = (int)Math.Ceiling(700 / (CandlestickChart.CandlesInterspace - CandlestickChart.candleWidth));
                var allValues = PriceData.Skip(CandlestickChart.CachedCandles - CandlestickChart.CandlesToView).Take(CandlestickChart.CachedCandles).SelectMany(data => new[] { data.Open, data.High, data.Low, data.Close });
                var minVal = allValues.Min();
                var maxVal = allValues.Max();


                CandlestickChart.minOriginal = minVal;
                CandlestickChart.maxOriginal = maxVal;
                double padding = (CandlestickChart.maxOriginal - CandlestickChart.minOriginal) * CandlestickChart.ScaleFactor;
                CandlestickChart.minOriginal -= padding;
                CandlestickChart.maxOriginal += padding;






                temp.Open = CandlestickChart.MapToScale(priceData.Open) + CandlestickChart.VericalOffset;
                temp.High = CandlestickChart.MapToScale(priceData.High) + CandlestickChart.VericalOffset;
                temp.Low = CandlestickChart.MapToScale(priceData.Low) + CandlestickChart.VericalOffset;
                temp.Close = CandlestickChart.MapToScale(priceData.Close) + CandlestickChart.VericalOffset;
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
        public void HandleOrderLineUpdate(string AmendingOrderID)
        {


            var existingOrder = OrdersInfo.FirstOrDefault(p => p.OrderID.Equals(AmendingOrderID));
            var updatedorderPrice = OrdersLines.FirstOrDefault(p => p.OrderID.Equals(AmendingOrderID));
            if (existingOrder != null)
            {

                double NewPrice = (double)CandlestickChart.InvMapToScale(double.Parse(updatedorderPrice.Price.ToString()));

                var diff = NewPrice - (double)existingOrder.Price;

                if (Math.Abs(diff) > 0)
                {


                    existingOrder.Price = (decimal)Math.Round(NewPrice, 2);

                    BitmexApi.AmmendOrder(existingOrder);
                }

            }

        }

        #endregion  

        #region POSITIONS_SECTION

        private void UpdatepositionLines(Position newPositionData)
        {

            PositionLine newpositionLine = new PositionLine
            {
                AccountID = newPositionData.AccountID,
                AvgEntryPrice = (decimal)CandlestickChart.MapToScale(double.Parse(newPositionData.AvgEntryPrice.ToString())),
                Symbol = newPositionData.Symbol,
                BreakEvenPrice = (decimal)CandlestickChart.MapToScale(double.Parse(newPositionData.BreakEvenPrice.ToString())),
                UnrealisedPnl = (int)newPositionData.UnrealisedPnl,
                LiquidationPrice = (decimal)newPositionData.LiquidationPrice,
                DeltaFromBreakEven = (decimal)newPositionData.DeltaFromBreakEven,

            };

            var existingPositionLine = PositionsLines.FirstOrDefault(p => p.AccountID.Equals(newPositionData.AccountID) && p.Symbol.Equals(newPositionData.Symbol));
            if (existingPositionLine != null)
            {
                PositionsLines.Remove(existingPositionLine);
            }

            PositionsLines.Add(newpositionLine);
             


        }
        private void OnPositionUpdate(Position newPositionData)
        {
            var existingPosition = PositionsInfo.FirstOrDefault(p => p.AccountID == newPositionData.AccountID && p.Symbol == newPositionData.Symbol);


            if (existingPosition != null)
            {

                //MessageBox.Show("existing pos has commision " + existingPosition.PosComm.HasValue.ToString());
                // Create a new Position object with updated data
                var updatedPosition = new Position
                {
                    AccountID = existingPosition.AccountID,
                    Symbol = existingPosition.Symbol,
                    AvgEntryPrice = newPositionData.AvgEntryPrice.HasValue ? newPositionData.AvgEntryPrice.Value : existingPosition.AvgEntryPrice,
                    MarkPrice = newPositionData.MarkPrice.HasValue ? newPositionData.MarkPrice.Value : existingPosition.MarkPrice,
                    BreakEvenPrice = newPositionData.BreakEvenPrice.HasValue ? newPositionData.BreakEvenPrice.Value : existingPosition.BreakEvenPrice,
                    LiquidationPrice = newPositionData.LiquidationPrice.HasValue ? newPositionData.LiquidationPrice.Value : existingPosition.LiquidationPrice,
                    RealisedPnl = newPositionData.RealisedPnl.HasValue ? newPositionData.RealisedPnl.Value : existingPosition.RealisedPnl,
                    UnrealisedPnl = newPositionData.UnrealisedPnl.HasValue ? newPositionData.UnrealisedPnl.Value : existingPosition.UnrealisedPnl,
                    Commission = newPositionData.Commission.HasValue ? newPositionData.Commission.Value : existingPosition.Commission,
                    Leverage = newPositionData.Leverage.HasValue ? newPositionData.Leverage.Value : existingPosition.Leverage,
                    CurrentQty = newPositionData.CurrentQty.HasValue ? newPositionData.CurrentQty.Value : existingPosition.CurrentQty,
                    CurrentCost = newPositionData.CurrentCost.HasValue ? newPositionData.CurrentCost.Value : existingPosition.CurrentCost,
                    RealisedCost = newPositionData.RealisedCost.HasValue ? newPositionData.RealisedCost.Value : existingPosition.RealisedCost,
                    PosComm = newPositionData.PosComm.HasValue ? newPositionData.PosComm.Value : existingPosition.PosComm,
                    MarkValue = newPositionData.MarkValue.HasValue ? newPositionData.MarkValue.Value : existingPosition.MarkValue,
                    RebalancedPnl = newPositionData.RebalancedPnl.HasValue ? newPositionData.RebalancedPnl.Value : existingPosition.RebalancedPnl,
                    HomeNotional = newPositionData.HomeNotional.HasValue ? newPositionData.HomeNotional.Value : existingPosition.HomeNotional,
                    ForeignNotional = newPositionData.ForeignNotional.HasValue ? newPositionData.ForeignNotional.Value : existingPosition.ForeignNotional,
                };

                int index = PositionsInfo.IndexOf(existingPosition);
                PositionsInfo[index] = updatedPosition;
                UpdatepositionLines(updatedPosition);

                if (updatedPosition.MarkValue == 0 && updatedPosition.RebalancedPnl == 0)
                {
                    PositionsLines.Clear();
                    PositionsInfo.RemoveAt(index);
                    PositionsdatsUpdated?.Invoke();
                }



            }
            else if (Math.Abs((decimal)newPositionData.CurrentQty) > 0)
            {

                PositionsInfo.Add(newPositionData);
                UpdatepositionLines(newPositionData);
                PositionsdatsUpdated?.Invoke();
            }


        }

        private ICommand _closePositionCommand;
        public ICommand ClosePositionCommand
        {
            get
            {
                if (_closePositionCommand == null)
                {
                    _closePositionCommand = new RelayCommand(param =>
                    {
                        var parameters = (Tuple<string, string>)param;
                        string symbol = parameters.Item1;
                        string Type = parameters.Item2;
                        ClosePosition(symbol, Type);
                    });
                }
                return _closePositionCommand;
            }
        }

        public void ClosePosition(string Symbol, string Type)
        {

            var existingPosition = PositionsInfo.FirstOrDefault(p => p.Symbol == Symbol);
            if (existingPosition != null)
            {
                //MessageBox.Show(existingPosition.Symbol);
                BitmexApi.ClosePosition(Type, existingPosition);
            }

        }




        #endregion 


        
 
 
      

        // this takes the function CreateNewOrder


 

        private ObservableCollection<CurrentClosePrice> _currentClose = new ObservableCollection<CurrentClosePrice>();


        public ObservableCollection<CurrentClosePrice> CurrentClose
        {
            get => _currentClose;
            set
            {
                _currentClose = value;
                OnPropertyChanged(nameof(CurrentClose));
            }
        }



        
         

        // PRICES LIVE STREAM SECTION


        public Action ScaledPriceDataUpdated;


        public void RefreshScaledPriceData()
        {
            ScaledPriceData.Clear();

            int ToSkip = CandlestickChart.CachedCandles - CandlestickChart.CandlesToView;
            for (int i = ToSkip; i < PriceData.Count; i++)
            {
                CandlestickData scaledCandle = ScaleCandle(PriceData[i]);
                scaledCandle.Posx = (i - ToSkip) * CandlestickChart.CandlesInterspace;
                scaledCandle.Width = CandlestickChart.candleWidth;

                ScaledPriceData.Add(scaledCandle);

                if (!MainWindow.isDraggingOrderLine)
                {
                    UpdateOrderLinesRescale(OrdersInfo);
                }

                if (ScaledPriceData.Count >= CandlestickChart.CandlesToView)
                {


                }
            }
        }

        private void AppendPriceData(DateTime timestamp, CandlestickData priceData)
        {
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
                int index = PriceData.IndexOf(existingData);

                if (index >= 0)
                {

                    PriceData[index] = existingData; // Update the item in the ObservableCollection


                }
            }
        }
        private void AppendScaledPriceData(DateTime timestamp, CandlestickData priceData)
        {
            var ScaledPriceExisting = ScaledPriceData.FirstOrDefault(data => data.Timestamp.Equals(timestamp));
            int indexScaledPrice = ScaledPriceData.IndexOf(ScaledPriceExisting);
            if (indexScaledPrice > 0)
            {
                ScaledPriceData[indexScaledPrice] = ScaleCandle(priceData);
                ScaledPriceData[indexScaledPrice].Posx = ScaledPriceExisting.Posx;
            }

        }

        private void AddNewPriceData(DateTime timestamp, CandlestickData priceData)
        {
            _priceDataDictionary[timestamp.ToString()] = priceData;
            priceData.Posx = CandlestickChart.CandlesInterspace * PriceData.Count;

            PriceData.Add(priceData);

            if (PriceData.Count > CandlestickChart.CachedCandles)
            {
                PriceData.Remove(PriceData.First());

                for (int i = 0; i < PriceData.Count; i++)
                {
                    PriceData[i].Posx = CandlestickChart.CandlesInterspace * i;


                }
            }
        }

        private void AddNewScaledPriceData(DateTime timestamp, CandlestickData priceData)
        {
            _priceDataDictionary[timestamp.ToString()] = priceData;
            priceData.Posx = CandlestickChart.CandlesInterspace * PriceData.Count;

            ScaledPriceData.Add(ScaleCandle(priceData));

            if (ScaledPriceData.Count > CandlestickChart.CandlesToView)
            {
                ScaledPriceData.Remove(ScaledPriceData.First());

                for (int i = 0; i < ScaledPriceData.Count; i++)
                {
                    ScaledPriceData[i].Posx = CandlestickChart.CandlesInterspace * i;
                }
            }
        }
        private void OnPriceUpdatedBinance(CandlestickData priceData)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {

                RefreshScaledPriceData();

                var timestamp = priceData.Timestamp;

                CurrentClosePrice crt = new CurrentClosePrice
                {
                    PriceValue = priceData.Close,
                    Symbol = priceData.Symbol,

                };

                CurrentClose.Clear();
                CurrentClose.Add(crt);
                //MessageBox.Show(CurrentClose[0].ToString());

                if (_priceDataDictionary.ContainsKey(timestamp.ToString()) && priceData != null)
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
        private void OnPriceUpdatedBitmex(SettledPrice setpriceData)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {

                var timestamp = setpriceData.Timestamp;
                SettledPriceData.Clear();

                SettledPriceData.Add(setpriceData);

                SettledPriceDataUpdated?.Invoke();


            });
        }

        public async void RefreshDataContext()
        {
            try
            {
                var BinanceInstrument = symbolSelectionViewModel.SelectedTicker.ToString().Equals("BTCUSD") ? "BTCUSDT" : symbolSelectionViewModel.SelectedTicker.ToString();
                var TimeFrame = symbolSelectionViewModel.SelectedTimeFrame.ToString();
                var BitmexInstrument = ExchangeTickersMap[symbolSelectionViewModel.SelectedTicker.ToString()];
                if (TimeFrame != null && BitmexInstrument != null)
                {
                    await WebSocketManager.Instance.CloseAllWebSocketsAsync(CancellationToken.None);
                    PriceData.Clear();
                    ScaledPriceData.Clear();
                    SettledPriceData.Clear();
                    

                    SetEndpoints(CandlestickChart.CachedCandles, BinanceInstrument, TimeFrame, BitmexInstrument);


                    BinanceApi = new BinanceAPI(IdBinance, ApiKeyBinance, BinanceEndpointRest, BinanceEndpointWss);
                    BinanceApi.GetPriceREST(PriceData, _priceDataDictionary);
                    BinanceApi.PriceUpdated += OnPriceUpdatedBinance;


                    BitmexApi = new BitmexAPI(IdBitmex, ApiKeyBitmex, BitmexEndpointRest, BitmexEndpointWss);
                    BitmexApi.SettledPriceUpdated += OnPriceUpdatedBitmex;

                    entryViewModel.RefreshWalletInfo();

                    BitmexApi.PositionUpdated += OnPositionUpdate;
                    BitmexApi.OrderUpdated += OnOrderReceived;

                    StartPriceFeed();
                }
                  
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing view model: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }




    }
}
