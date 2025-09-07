using BitmexGUI.Models;
using BitmexGUI.Abstract;
using BitmexGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Interfaces
{
    public interface IOrdersVM : INotifyPropertyChanged
    {
        IAPI API { get; }
        SymbolSelectionViewModel symbolSelectionViewModel { get; } 
        EntryViewModel entryViewModel { get; } 
        PriceStreamViewModel priceStreamViewModel { get; } 
        ObservableCollection<OrderLine> OrdersLines { get; }
        ObservableCollection<Order> OrdersInfo { get; }
        ObservableCollection<Order> HistoricOrdersInfo { get; }

        void CreateNewOrder(string Side);
        void CancelOrder(string OrderID);
        void UpdateAddOrderLines(Order newOrderData); 
        void RemoveorderLines(Order newOrderData);
        void OnOrderReceived(Order newOrderData);
        void HandleOrderLineUpdate(string AmendingOrderID);

    }
}
