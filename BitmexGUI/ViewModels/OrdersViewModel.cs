using BitmexGUI.Abstract;
using BitmexGUI.BaseClasses;
using BitmexGUI.Models;
using BitmexGUI.ViewModels.Utilities;
using BitmexGUI.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BitmexGUI.ViewModels
{
    public class OrdersViewModel: INotifyPropertyChanged
    { 
        BitmexAPI BitmexApi;
        SymbolSelectionViewModel symbolSelectionViewModel;
        EntryViewModel entryViewModel;
        PriceStreamViewModel priceStreamViewModel;

        #region Properties
        private ObservableCollection<OrderLine> _orderLines = new ObservableCollection<OrderLine>(); 
        public ObservableCollection<OrderLine> OrdersLines
        {
            get => _orderLines;
            set
            {
                _orderLines = value;
                OnPropertyChanged(nameof(OrdersLines));
            }
        }
        
        private ObservableCollection<Order> _orderData = new ObservableCollection<Order>();
        public ObservableCollection<Order> OrdersInfo
        {
            get => _orderData;
            set
            {
                _orderData = value;
                OnPropertyChanged(nameof(OrdersInfo));
            }
        }
        #endregion
        public OrdersViewModel(GeneralExchangeAPI API,
                               SymbolSelectionViewModel SymbolSLVM,
                               EntryViewModel EntryVM,
                               PriceStreamViewModel pricevm) 
        {
            try
            {
                BitmexApi = (BitmexAPI)API;
                symbolSelectionViewModel = SymbolSLVM;
                entryViewModel = EntryVM;
                priceStreamViewModel = pricevm;
                priceStreamViewModel.ScaledPriceUpdated += () => UpdateOrderLinesRescale(OrdersInfo.ToList());
                BitmexApi.OrderUpdated += OnOrderReceived;


            }
            catch(Exception ex)
            {
                MessageBox.Show("Error in OrdersViewModel constructor: " + ex.Message + "\n" + ex.StackTrace);
            }   
            
             
        }

        private ICommand _cancelOrder;
        private void ExecuteCancelOrder(object param)
        {
            string OrderID = param.ToString();
            CancelOrder(OrderID);
        }
        public ICommand CancelOrderCommand
        {
            get
            {
                if (_cancelOrder == null)
                {
                    _cancelOrder = new RelayCommand(ExecuteCancelOrder);
                }
                return _cancelOrder;
            }
        }
        public void CancelOrder(string OrderID)
        {

            BitmexApi.CancelOrder(OrderID);
        }



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

                BitmexApi.CreateOrder(SymbolSelectionViewModel.ExchangeTickersMap[symbolSelectionViewModel.SelectedTicker],
                                       entryViewModel.Quantity * 1000000,
                                       Math.Round(entryViewModel.EntryPrice, 0),
                                       "Limit",
                                       "GoodTillCancel",
                                       "Buy",
                                       entryViewModel.SliderLeverage);
            }

            else if (orderside.Contains("selllimit"))
            {

                BitmexApi.CreateOrder(SymbolSelectionViewModel.ExchangeTickersMap[symbolSelectionViewModel.SelectedTicker],
                                       entryViewModel.Quantity * 1000000,
                                       Math.Round(entryViewModel.EntryPrice, 0),
                                       "Limit",
                                       "GoodTillCancel",
                                       "Sell",
                                       entryViewModel.SliderLeverage);
            }

            else if (orderside.Contains("buymarket"))
            {
                BitmexApi.CreateOrder(SymbolSelectionViewModel.ExchangeTickersMap[symbolSelectionViewModel.SelectedTicker],
                                       entryViewModel.Quantity * 1000000,
                                       Math.Round(entryViewModel.EntryPrice, 0),
                                       "Market",
                                       "ImmediateOrCancel",
                                       "Buy",
                                       entryViewModel.SliderLeverage);
            }

            else if (orderside.Contains("sellmarket"))
            {
                BitmexApi.CreateOrder(SymbolSelectionViewModel.ExchangeTickersMap[symbolSelectionViewModel.SelectedTicker],
                                       -entryViewModel.Quantity * 1000000,
                                       Math.Round(entryViewModel.EntryPrice, 0),
                                       "Market",
                                       "ImmediateOrCancel",
                                       "Sell",
                                       entryViewModel.SliderLeverage);
            }

        }
       
       

        
        private void UpdateAddOrderLines(Order newOrderData)
        {


            OrderLine neworderLine = new OrderLine
            {
                OrderID = newOrderData.OrderID,
                Price = (decimal)(double.Parse(newOrderData.Price.ToString())),
                Symbol = newOrderData.Symbol,
                Side = newOrderData.Side

            };
            var existingOrderLine = OrdersLines.FirstOrDefault(p => p.OrderID.Equals(newOrderData.OrderID));
            if (existingOrderLine != null)
            {
                OrdersLines.Remove(existingOrderLine);
            }
            OrdersLines.Add(neworderLine); 

        }

        private void RemoveorderLines(Order newOrderData)
        {

            var existingOrderLine = OrdersLines.FirstOrDefault(p => p.OrderID.Equals(newOrderData.OrderID));
            if (existingOrderLine != null)
            {
                OrdersLines.Remove(existingOrderLine);
            }
        }
        private ObservableCollection<Order> _historicorderData = new ObservableCollection<Order>();
        public ObservableCollection<Order> HistoricOrdersInfo
        {
            get => _historicorderData;
            set
            {
                _historicorderData = value;
                OnPropertyChanged(nameof(HistoricOrdersInfo));
            }
        }
        public void OnOrderReceived(Order newOrderData)
        {
            if (newOrderData != null)
            { 
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


                            UpdateAddOrderLines(newOrderData);
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
                            UpdateAddOrderLines(TempOrder);
                        }
                        else
                        {
                            // If no existing order is found, add the new order to OrdersInfo
                            OrdersInfo.Add(newOrderData);
                            UpdateAddOrderLines(newOrderData);
                        }
                    }
                }

                var existingOrderHistory = HistoricOrdersInfo.FirstOrDefault(p => p.OrderID.Equals(newOrderData.OrderID));
                if (existingOrderHistory != null)
                {
                    HistoricOrdersInfo.Add(newOrderData);
                }
               
                
            }
        }
       
        public void UpdateOrderLinesRescale(List<Order> Orderinfos)
        {
            try
            {
                if (!OrderLines.isDraggingOrderLine && OrdersInfo.Count > 0)//this OrderLines is  the usercontrol view
                {
                    OrdersLines.Clear();

                    for (int j = 0; j < Orderinfos.Count; j++)
                    {
                        if(Orderinfos[j].Price.HasValue)
                        {
                            var Price = (double)(Orderinfos[j].Price.Value);

                            OrderLine tempOrdlIne = new OrderLine
                            {
                                OrderID = Orderinfos[j].OrderID,
                                Price = (decimal)Orderinfos[j].Price,
                                Side = Orderinfos[j].Side,
                                Symbol = Orderinfos[j].Symbol
                            };
                            OrdersLines.Add(tempOrdlIne);
                        }
                       
                    }

                }
            }
          catch(Exception ex)
            {
                MessageBox.Show("Error in UpdateOrderLinesRescale: " + ex.Message + "\n" + ex.StackTrace);
            }

        }
         
        public void HandleOrderLineUpdate(string AmendingOrderID)
        {


            var existingOrder = OrdersInfo.FirstOrDefault(p => p.OrderID.Equals(AmendingOrderID));
            var updatedorderPrice = OrdersLines.FirstOrDefault(p => p.OrderID.Equals(AmendingOrderID));
            if (existingOrder != null)
            {

                double NewPrice = (double)double.Parse(updatedorderPrice.Price.ToString());

                var diff = NewPrice - (double)existingOrder.Price;

                if (Math.Abs(diff) > 0)
                {
                    existingOrder.Price = (decimal)Math.Round(NewPrice, 2);

                    BitmexApi.AmmendOrder(existingOrder);
                }

            }

        }
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action OrderLineUpdated;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
       
    }
}
