using BitmexGUI.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BitmexGUI.ViewModels
{
    public class mainViewProperties : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<CandlestickData> _priceData = new ObservableCollection<CandlestickData>();
        private ObservableCollection<SettledPrice> _settledPriceData = new ObservableCollection<SettledPrice>();
        private ObservableCollection<Account> _accountData = new ObservableCollection<Account>();
        private ObservableCollection<Instrument> _instrumentData = new ObservableCollection<Instrument>();
        private ObservableCollection<Position> _positionData = new ObservableCollection<Position>();
        private ObservableCollection<Order> _orderData = new ObservableCollection<Order>();
        private ObservableCollection<Order> _historicorderData = new ObservableCollection<Order>();
        private ObservableCollection<CandlestickData> _scaledpriceData = new ObservableCollection<CandlestickData>();
        private ObservableCollection<OrderLine> _orderLines = new ObservableCollection<OrderLine>();
        private ObservableCollection<PositionLine> _positionLine = new ObservableCollection<PositionLine>();


        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ObservableCollection<OrderLine> OrdersLines
        {
            get => _orderLines;
            set
            {
                _orderLines = value;
                OnPropertyChanged(nameof(OrdersLines));
            }
        }

        public ObservableCollection<PositionLine> PositionsLines
        {
            get => _positionLine;
            set
            {
                _positionLine = value;
                OnPropertyChanged(nameof(PositionsLines));
            }
        }



        public ObservableCollection<CandlestickData> ScaledPriceData
        {
            get => _scaledpriceData;
            set
            {
                _scaledpriceData = value;
                OnPropertyChanged(nameof(ScaledPriceData));
            }
        }
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
        public ObservableCollection<Account> AccountInfos
        {
            get => _accountData;
            set
            {
                _accountData = value;
                OnPropertyChanged(nameof(AccountInfos));
            }
        }
        public ObservableCollection<Instrument> InstrumentInfo
        {
            get => _instrumentData;
            set
            {
                _instrumentData = value;
                OnPropertyChanged(nameof(InstrumentInfo));
            }
        }
        public ObservableCollection<Order> OrdersInfo
        {
            get => _orderData;
            set
            {
                _orderData = value;
                OnPropertyChanged(nameof(OrdersInfo));
            }
        }

        public ObservableCollection<Order> HistoricOrdersInfo
        {
            get => _historicorderData;
            set
            {
                _historicorderData = value;
                OnPropertyChanged(nameof(HistoricOrdersInfo));
            }
        }

        public ObservableCollection<Position> PositionsInfo
        {
            get => _positionData;
            set
            {
                _positionData = value;
                OnPropertyChanged();
            }
        }




    }
}
