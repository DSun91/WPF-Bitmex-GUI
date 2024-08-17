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

namespace BitmexGUI.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();
    }
    public class MainViewModel :OservableCollections
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
        private readonly int _maxCandlesLoading;
        private readonly BinanceAPI BinanceApi;
        private readonly BitmexAPI BitmexApi;
        private string TimeFrame= ConfigurationManager.AppSettings["Timeframe"];

       

   
        private double _entryAmount;
        private double _sliderLeverage;
        private double _positionValue;
        private double _entryPrice;




        public MainViewModel(int InitialCandlesNumber)
        {


            int.TryParse(ConfigurationManager.AppSettings["MaxCacheCandles"], out _maxCandlesLoading);


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




        public void HandleOrderLineUpdate(OrderLine updatedOrderLine)
        {
            var existingOrder= OrdersInfo.FirstOrDefault(p => p.OrderID.Equals(updatedOrderLine.OrderID));

            if (existingOrder != null) 
            { 
                double NewPrice = (double)CandlestickChart.InvMapToScale(double.Parse(updatedOrderLine.Price.ToString()));

                var diff = NewPrice - (double)existingOrder.Price; 

                if (Math.Abs(diff) > 0) 
                {
                    //MessageBox.Show(diff.ToString());
                    existingOrder.Price = (decimal)Math.Round(NewPrice,0);
                     
                    BitmexApi.AmmendOrder(existingOrder);
                }
                 
            }
             
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
                    PosComm = newPositionData.PosComm.HasValue ? newPositionData.PosComm.Value: existingPosition.PosComm,
                    MarkValue = newPositionData.MarkValue.HasValue ? newPositionData.MarkValue.Value : existingPosition.MarkValue,
                    RebalancedPnl = newPositionData.RebalancedPnl.HasValue ? newPositionData.RebalancedPnl.Value : existingPosition.RebalancedPnl
                };

                int index = PositionsInfo.IndexOf(existingPosition);
                PositionsInfo[index] = updatedPosition;
                //MessageBox.Show("updated pos has commision " + newPositionData.Symbol + " " + updatedPosition.MarkValue + " " + updatedPosition.RebalancedPnl + " " + updatedPosition.AvgEntryPrice);
                if (updatedPosition.MarkValue==0 && updatedPosition.RebalancedPnl==0)
                {
                    
                    PositionsInfo.RemoveAt(index);
                    PositionsdatsUpdated?.Invoke();
                }

            }
            else if (newPositionData.CurrentQty > 0)
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
            Quantity = (Math.Round(1000 * EntryAmount * SliderLeverage / EntryPrice)/1000);
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
                    _createNewOrderCommand = new RelayCommand(CreateNewOrder);
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
            ActualPositionValue = Math.Round(Quantity * EntryPrice,2);
        }

        public void CreateNewOrder()
        {

            //MessageBox.Show($"Order Cost: {actualtradeAmount * SliderLeverage}, fee {actualtradeAmount * SliderLeverage * 0.00015},total spent {actualtradeAmount + 2 * (actualtradeAmount * SliderLeverage * 0.00015)}");

            BitmexApi.CreateOrder(ConfigurationManager.AppSettings["BitMexSymbol"].ToString(),
                                   Quantity*1000000,
                                   Math.Round(EntryPrice, 0),
                                   "Limit",
                                   "GoodTillCancel",
                                   "Buy",
                                   SliderLeverage);


        }

        public void StartPriceFeed()
        {
            BinanceApi.GetPriceWSS();
            BitmexApi.GetPriceWSS();
           
        }


        private string _currency;

        public event Action currencieslist;

        private List<string> _currencies=new List<string>();


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

        private CandlestickData ScaleCandle(CandlestickData priceData)
        {
            var allValues = PriceData.SelectMany(data => new[] { data.Open, data.High, data.Low, data.Close });
            CandlestickChart.minOriginal = allValues.Min();
            CandlestickChart.maxOriginal = allValues.Max();
            double padding = (CandlestickChart.maxOriginal - CandlestickChart.minOriginal) * double.Parse(ConfigurationManager.AppSettings["ScaleFactor"].ToString());
            CandlestickChart.minOriginal -= padding;
            CandlestickChart.maxOriginal += padding;


            //MessageBox.Show(_ViewModel.SettledPriceData.Count.ToString());
            CandlestickData temp = new CandlestickData();

            temp.Open = CandlestickChart.MapToScale(priceData.Open);
            temp.High = CandlestickChart.MapToScale(priceData.High);
            temp.Low = CandlestickChart.MapToScale(priceData.Low);
            temp.Close = CandlestickChart.MapToScale(priceData.Close);
            temp.Timestamp = priceData.Timestamp;
            temp.Posx = priceData.Posx;
            return temp;
        }

        public Action ScaledPriceDataUpdated;


        private void RefreshScaledPriceData()
        {
            ScaledPriceData.Clear();


            
            foreach (var priceData in PriceData)
            {
                CandlestickData temp = ScaleCandle(priceData);

                ScaledPriceData.Add(temp);
                if (ScaledPriceData.Count >= int.Parse(ConfigurationManager.AppSettings["MaxCacheCandles"].ToString()))
                {
                    BitmexApi.GetPositionWSS();
                    BitmexApi.GetOrdersWSS();
                    
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
                        ScaledPriceData[index] = ScaleCandle(existingData);

                    }
                    PriceDataUpdated?.Invoke(); // Trigger the event

                     
                }
                else
                {
                    
                    // Add new entry
                    _priceDataDictionary[timestamp.ToString()] = priceData;
                    priceData.Posx = CandlestickChart.interspace * PriceData.Count;


                    PriceData.Add(priceData);
                    ScaledPriceData.Add(ScaleCandle(priceData));



                    if (PriceData.Count > _maxCandlesLoading)
                    {
                        PriceData.Remove(PriceData.First());
                        ScaledPriceData.Remove(ScaledPriceData.First());

                        for (int i = 0; i < PriceData.Count; i++)
                        {
                            PriceData[i].Posx = CandlestickChart.interspace * i;
                            ScaledPriceData[i].Posx = CandlestickChart.interspace * i;
                        }
                    }
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
