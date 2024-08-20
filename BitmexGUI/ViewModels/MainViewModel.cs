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
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.VisualBasic;
using System;
using System.Reflection;
using System.Windows.Controls;
using BitmexGUI.Views;

namespace BitmexGUI.ViewModels
{
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
    public class MainViewModel : mainViewProperties
    {
        private readonly string IdBinance = "";
        private readonly string ApiKeyBinance = "";
        private readonly string IdBitmex = "";
        private readonly string ApiKeyBitmex = "";

        private readonly string BinanceEndpointRest = ConfigurationManager.AppSettings["BaseRESTBinance"];
        private readonly string BinanceEndpointWss = ConfigurationManager.AppSettings["BaseWSSBinance"];
        private readonly string BitmexEndpointRest = ConfigurationManager.AppSettings["BaseBitmexUrl"] + ConfigurationManager.AppSettings["BaseRESTBitmex"];
        private readonly string BitmexEndpointWss = ConfigurationManager.AppSettings["BaseWSSBitmex"];
        private readonly string BinanceInstrument = ConfigurationManager.AppSettings["Instrument"];
        private readonly string BitmexInstrument = ConfigurationManager.AppSettings["BitMexSymbol"];
        public event Action PriceDataUpdated;
        public event Action SettledPriceDataUpdated;
        public event Action BalanceUpdated;
        public event Action NewPricedataAdded;
        public event Action PositionsdatsUpdated;
        public event Action OpenordersInfoUpdated;
        public event Action HistoricOrderdataUpdated;
        private Dictionary<string, CandlestickData> _priceDataDictionary = new Dictionary<string, CandlestickData>();

        private readonly BinanceAPI BinanceApi;
        private readonly BitmexAPI BitmexApi;
        private string TimeFrame = ConfigurationManager.AppSettings["Timeframe"];





        private double _entryAmount;
        private double _sliderLeverage;
        private double _positionValue;
        private double _entryPrice;




        public MainViewModel(int InitialCandlesNumber)
        {





            BinanceEndpointRest += $"/klines?symbol={BinanceInstrument}&interval={TimeFrame}&limit={InitialCandlesNumber}";
            BinanceEndpointWss += $"{BinanceInstrument.ToLower()}@kline_{TimeFrame}";
            BitmexEndpointWss += $"?subscribe=instrument:{BitmexInstrument}";
            //MessageBox.Show(BinanceEndpointRest);
            BinanceApi = new BinanceAPI(IdBinance, ApiKeyBinance, BinanceEndpointRest, BinanceEndpointWss);
            BinanceApi.GetPriceREST(PriceData, _priceDataDictionary);
            BinanceApi.PriceUpdated += OnPriceUpdatedBinance;

            BitmexApi = new BitmexAPI(IdBitmex, ApiKeyBitmex, BitmexEndpointRest, BitmexEndpointWss);
            BitmexApi.SettledPriceUpdated += OnPriceUpdatedBitmex;

            BitmexApi.AccountInfo += OnWalletInfoReceived;
            BitmexApi.PositionUpdated += OnPositionUpdate;
            BitmexApi.OrderUpdated += OnOrderReceived;


            //BitmexApi.SetLeverage("XBTUSDT",5.4);



        }

        public void CancelOrder(string OrderID)
        {

            BitmexApi.CancelOrder(OrderID);
        }

        public Action OrderLineUpdated;
        private void UpdateorderLines(Order newOrderData)
        {


            OrderLine neworderLine = new OrderLine
            {
                OrderID = newOrderData.OrderID,
                Price = (decimal)CandlestickChart.MapToScale(double.Parse(newOrderData.Price.ToString())),
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
            OrderLineUpdated?.Invoke();
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


        private void OnPositionUpdate(Position newPositionData)
        {
            var existingPosition = PositionsInfo.FirstOrDefault(p => p.AccountID == newPositionData.AccountID && p.Symbol == newPositionData.Symbol);

            // MessageBox.Show("new pos has commision "+ newPositionData.Symbol+ " "+newPositionData.PosComm.HasValue.ToString()+" "+ newPositionData.PosComm+" "+ newPositionData.CurrentQty);
            //MessageBox.Show("new pos has commision " + newPositionData.Symbol + " " + newPositionData.PosComm.HasValue.ToString() + " " + newPositionData.MarkPrice + " " + newPositionData.AvgEntryPrice);
            //PositionsInfo.Clear();

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
                    RebalancedPnl = newPositionData.RebalancedPnl.HasValue ? newPositionData.RebalancedPnl.Value : existingPosition.RebalancedPnl
                };

                int index = PositionsInfo.IndexOf(existingPosition);
                PositionsInfo[index] = updatedPosition;
                //MessageBox.Show("updated pos has commision " + newPositionData.Symbol + " " + updatedPosition.MarkValue + " " + updatedPosition.RebalancedPnl + " " + updatedPosition.AvgEntryPrice);
                if (updatedPosition.MarkValue == 0 && updatedPosition.RebalancedPnl == 0)
                {

                    PositionsInfo.RemoveAt(index);
                    PositionsdatsUpdated?.Invoke();
                }

            }
            else if (Math.Abs((decimal)newPositionData.CurrentQty) > 0)
            {

                PositionsInfo.Add(newPositionData);
                PositionsdatsUpdated?.Invoke();
            }
        }
        public double EntryAmount
        {
            get => _entryAmount;
            set
            {
                if (Math.Abs(_entryAmount - value) > 0.001) // Avoid unnecessary updates
                {
                    _entryAmount = value;
                    OnPropertyChanged();
                    CalculatePositionValue();
                    CalculateOrderCost();
                }
            }
        }

        public double SliderLeverage
        {
            get => _sliderLeverage;
            set
            {
                if (Math.Abs(_sliderLeverage - value) > 0.001) // Avoid unnecessary updates
                {
                    _sliderLeverage = Math.Round(value);
                    OnPropertyChanged();
                    CalculatePositionValue();
                    CalculateOrderCost();
                }
            }
        }
        private void CalculatePositionValue()
        {

            if (_entryAmount > 0 && _sliderLeverage > 0)
            {

                PositionValue = Math.Round(_entryAmount * _sliderLeverage, 2);

            }
            else if (_entryAmount > 0 && _sliderLeverage <= 0)
            {
                PositionValue = Math.Round(_entryAmount, 2);
            }
            else
            {
                PositionValue = 0;
            }
        }

        private void CalculateOrderCost()
        {
            CalculateQuantity();

            EntryPrice = Math.Round(EntryPrice, 0);

            double EOV = (Quantity) * EntryPrice;

            double BK = EOV + (EOV / SliderLeverage);

            OrderCost = Math.Round((EOV / SliderLeverage) + (EOV + BK) * (0.075 / 100), 2);
        }

        private void CalculateQuantity()
        {
            Quantity = (Math.Round(1000 * EntryAmount * SliderLeverage / EntryPrice) / 1000);
            CalculateActualPositionValue();
        }


        public double PositionValue
        {
            get => _positionValue;
            private set
            {
                if (Math.Abs(_positionValue - value) > 0.001) // Avoid unnecessary updates
                {
                    _positionValue = value;
                    OnPropertyChanged();

                }
            }
        }

        public double EntryPrice
        {
            get => _entryPrice;
            set
            {
                if (Math.Abs(_entryPrice - value) > 0.001) // Avoid unnecessary updates
                {
                    _entryPrice = value;
                    CalculateOrderCost();
                    OnPropertyChanged();
                }
            }
        }


        // this takes the function CreateNewOrder
        private ICommand _createNewOrderCommand;
        public ICommand CreateNewOrderCommand
        {
            get
            {
                if (_createNewOrderCommand == null)
                {
                    _createNewOrderCommand = new RelayCommand(x => CreateNewOrder(x.ToString()));
                }
                return _createNewOrderCommand;
            }
        }

        private double _orderCost;


        public double OrderCost
        {
            get => _orderCost;
            set
            {
                _orderCost = value;
                OnPropertyChanged();



            }
        }

        private double _quantity;


        public double Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }

        private double _actualpositionvalue;

        public double ActualPositionValue
        {
            get => _actualpositionvalue;
            set
            {
                _actualpositionvalue = value;
                OnPropertyChanged();
            }
        }

        private void CalculateActualPositionValue()
        {
            ActualPositionValue = Math.Round(Quantity * EntryPrice, 2);
        }

        public void CreateNewOrder(string Side)
        {

            string orderside = Side.ToLower().Replace(" ", "");


            if (orderside.Contains("buylimit"))
            {

                BitmexApi.CreateOrder(ConfigurationManager.AppSettings["BitMexSymbol"].ToString(),
                                       Quantity * 1000000,
                                       Math.Round(EntryPrice, 0),
                                       "Limit",
                                       "GoodTillCancel",
                                       "Buy",
                                       SliderLeverage);
            }

            else if (orderside.Contains("selllimit"))
            {

                BitmexApi.CreateOrder(ConfigurationManager.AppSettings["BitMexSymbol"].ToString(),
                                       Quantity * 1000000,
                                       Math.Round(EntryPrice, 0),
                                       "Limit",
                                       "GoodTillCancel",
                                       "Sell",
                                       SliderLeverage);
            }

            else if (orderside.Contains("buymarket"))
            {
                BitmexApi.CreateOrder(ConfigurationManager.AppSettings["BitMexSymbol"].ToString(),
                                       Quantity * 1000000,
                                       Math.Round(EntryPrice, 0),
                                       "Market",
                                       "ImmediateOrCancel",
                                       "Buy",
                                       SliderLeverage);
            }

            else if (orderside.Contains("sellmarket"))
            {
                BitmexApi.CreateOrder(ConfigurationManager.AppSettings["BitMexSymbol"].ToString(),
                                       -Quantity * 1000000,
                                       Math.Round(EntryPrice, 0),
                                       "Market",
                                       "ImmediateOrCancel",
                                       "Sell",
                                       SliderLeverage);
            }



        }

        public void StartPriceFeed()
        {
            BinanceApi.GetPriceWSS();
            BitmexApi.GetPositionWSS();

        }


        private string _currency;

        public event Action currencieslist;

        private List<string> _currencies = new List<string>();


        private string _currentBalance;
        public string SelectedCurrency
        {
            get => _currency;
            set
            {
                _currency = value.ToUpper();

                UpdateCurrentBalance();
                OnPropertyChanged();
            }
        }

        public string CurrentBalance
        {
            get => _currentBalance;
            set
            {
                if (_currentBalance != value)
                {
                    _currentBalance = value;
                    OnPropertyChanged(nameof(CurrentBalance));
                }
            }
        }
        private void UpdateCurrentBalance()
        {
            var account = AccountInfos.FirstOrDefault(x => x.CurrencyName.Equals(SelectedCurrency, StringComparison.OrdinalIgnoreCase));
            if (account != null)
            {
                CurrentBalance = account.Balance.ToString(); // This will trigger OnPropertyChanged
            }
            else
            {
                CurrentBalance = "0"; // Or handle as needed if no account is found
            }
        }



        public void GetBalances()
        {
            BitmexApi.GetWallet();
        }

        public List<string> Currencies
        {
            get => _currencies;
            set
            {
                _currencies = value;
                OnPropertyChanged(nameof(Currencies));
            }
        }



        private void OnWalletInfoReceived(Account accountInfo)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (accountInfo != null)
                {

                    AccountInfos.Add(accountInfo);
                    Currencies.Add(accountInfo.CurrencyName);
                    OnPropertyChanged(nameof(Currencies));
                }
            });

            BalanceUpdated?.Invoke();
        }


        private void UpdateOrderLinesRescale(ObservableCollection<Order> Orderinfos)
        {
            OrdersLines.Clear();

            for (int j = 0; j < Orderinfos.Count; j++)
            {
                var Price = (decimal)CandlestickChart.MapToScale((double)Orderinfos[j].Price);

                OrderLine tempOrdlIne = new OrderLine
                {
                    OrderID = Orderinfos[j].OrderID,
                    Price = Price,
                    Side = Orderinfos[j].Side,
                    Symbol = Orderinfos[j].Symbol
                };
                OrdersLines.Add(tempOrdlIne);



            }
        }


        private CandlestickData ScaleCandle(CandlestickData priceData)
        {
            var allValues = PriceData.SelectMany(data => new[] { data.Open, data.High, data.Low, data.Close });
            var minVal = allValues.Min();
            var maxVal = allValues.Max();


            CandlestickChart.minOriginal = minVal;
            CandlestickChart.maxOriginal = maxVal;
            double padding = (CandlestickChart.maxOriginal - CandlestickChart.minOriginal) * CandlestickChart.ScaleFactor;
            CandlestickChart.minOriginal -= padding;
            CandlestickChart.maxOriginal += padding;

            //UpdateOrderLinesRescale(OrdersInfo); 
            CandlestickData temp = new CandlestickData();



            temp.Open = CandlestickChart.MapToScale(priceData.Open);
            temp.High = CandlestickChart.MapToScale(priceData.High);
            temp.Low = CandlestickChart.MapToScale(priceData.Low);
            temp.Close = CandlestickChart.MapToScale(priceData.Close);
            temp.Timestamp = priceData.Timestamp;
            temp.Width = priceData.Width;
            temp.Posx = priceData.Posx;
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


                    existingOrder.Price = (decimal)Math.Round(NewPrice, 0);

                    BitmexApi.AmmendOrder(existingOrder);
                }

            }

        }
        public Action ScaledPriceDataUpdated;


        public void RefreshScaledPriceData()
        {
            ScaledPriceData.Clear();

            int ToSkip = CandlestickChart.CachedCandles - CandlestickChart.CandlesToView;
            for (int i = ToSkip; i < PriceData.Count; i++)
            {
                CandlestickData scaledCandle = ScaleCandle(PriceData[i]);
                scaledCandle.Posx = (i-ToSkip) * CandlestickChart.CandlesInterspace;
                scaledCandle.Width = CandlestickChart.candleWidth;
               
                ScaledPriceData.Add(scaledCandle);

                if (!MainWindow.isDraggingOrderLine)
                {
                    UpdateOrderLinesRescale(OrdersInfo);
                }

                if (ScaledPriceData.Count >= CandlestickChart.CachedCandles)
                {
                    BitmexApi.GetOrdersWSS();

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


                
                if (_priceDataDictionary.ContainsKey(timestamp.ToString()) && priceData != null)
                {

                    AppendPriceData(timestamp, priceData);
                    AppendScaledPriceData(timestamp, priceData);


                    PriceDataUpdated?.Invoke(); // Trigger the event

                     
                }
                else
                {

                    // Add new entry
                    AddNewPriceData( timestamp,  priceData);
                    AddNewScaledPriceData(timestamp, priceData);
                    NewPricedataAdded?.Invoke();

                     
                }


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

        //public void UpdateInitialCandles(int newInitialCandlesNumber)
        //{

        //    BinanceApi.UpdateRestEndpoint($"https://api.binance.com/api/v3/klines?symbol=BTCUSDT&interval={TimeFrame}&limit={newInitialCandlesNumber}");

        //    //MessageBox.Show(_binanceApi.UrlRest);

        //    PriceData = new ObservableCollection<CandlestickData>();

        //    _priceDataDictionary=new Dictionary<string,CandlestickData>();

             

        //    for (int i = BinanceApi.CachedPriceData.Count- newInitialCandlesNumber; i < BinanceApi.CachedPriceData.Count;i++)
        //    {
        //        PriceData.Add(BinanceApi.CachedPriceData[i]);
        //        _priceDataDictionary.TryAdd(BinanceApi.CachedPriceData[i].Timestamp.ToString(), BinanceApi.CachedPriceData[i]);
        //    }

            
        //    //_binanceApi.GetPriceREST(PriceData,_priceDataDictionary); 
        //}


         
    }
}
