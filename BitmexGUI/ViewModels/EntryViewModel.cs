using BitmexGUI.Models;
using BitmexGUI.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BitmexGUI.ViewModels
{
    public class EntryViewModel: INotifyPropertyChanged
    {

        private double _entryAmount;
        private double _sliderLeverage;
        private double _positionValue;
        private double _entryPrice;
        BitmexAPI _BitmexApi;
        private ObservableCollection<Account> _accountData = new ObservableCollection<Account>(); 
        public ObservableCollection<Account> AccountInfos
        {
            get => _accountData;
            set
            {
                _accountData = value;
                OnPropertyChanged(nameof(AccountInfos));
            }
        }
        public EntryViewModel(BitmexAPI BitmexApi)
        {
            _BitmexApi=BitmexApi;
            _BitmexApi.AccountInfo += OnWalletInfoReceived;
            RefreshWalletInfo();
            
        }
        public void RefreshWalletInfo()
        {
            _BitmexApi.GetWallet();
        }
        protected void OnWalletInfoReceived(Account accountInfo)
        {
           
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (accountInfo != null)
                {

                    if (!AccountInfos.Any(a => a.CurrencyName == accountInfo.CurrencyName))
                    {
                        AccountInfos.Add(accountInfo);
                    }
                    else
                    {
                        AccountInfos.Where(a => a.CurrencyName == accountInfo.CurrencyName)
                            .ToList()
                            .ForEach(a =>
                            {
                                a.Balance = accountInfo.Balance; 
                                a.CurrencyName = accountInfo.CurrencyName;
                            });
                    }

                    if (!Currencies.Contains(accountInfo.CurrencyName))
                    {
                        Currencies.Add(accountInfo.CurrencyName);
                    }
                   
                        EntryAmount = 0;
                    SliderLeverage = 1;
                    AmountSlider = 0;
                    UpdateCurrentTickerBalance();
                    OnPropertyChanged(nameof(Currencies)); 
                }
            });

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
            get
            {
                if (_sliderLeverage <= 0)
                {
                    _sliderLeverage = 1; // Default leverage if not set
                }
                return _sliderLeverage;
            }
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
        private double _AmountSlider;
        public double AmountSlider
        {
            get => _AmountSlider; 
            set
            {
                _AmountSlider = value;
                OnPropertyChanged();
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
            if (SliderLeverage != 0 && EntryPrice != 0)
            {
                Quantity = Math.Round(EntryAmount * SliderLeverage / EntryPrice, 3);
            }
            else if (EntryPrice != 0)
            {
                Quantity = (Math.Round(EntryAmount / EntryPrice, 3));
            }
            else
            {
                Quantity = 0;
            }
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



        private string _currency;
        public string SelectedCurrency
        {
            get => _currency;
            set
            { 
                _currency = value.ToUpper();

                UpdateCurrentTickerBalance();
                OnPropertyChanged();
            }
        }



        private ObservableCollection<string> _currencies = new ObservableCollection<string>();

        public ObservableCollection<string> Currencies
        {
            get => _currencies;
            set
            {
                _currencies = value;
                OnPropertyChanged(nameof(Currencies));
            }
        }

        private string _currentBalance;

        public string CurrentBalance
        {
            get => _currentBalance;
            set
            {
                if (_currentBalance != value)
                {
                    _currentBalance =Math.Round( double.Parse(value),3).ToString();
                    OnPropertyChanged(nameof(CurrentBalance));
                }
            }
        }

        private void UpdateCurrentTickerBalance()
        {
            
            var account = AccountInfos.FirstOrDefault(x => x.CurrencyName.Equals(SelectedCurrency, StringComparison.OrdinalIgnoreCase));
            if (account != null)
            {
                CurrentBalance = account.Balance.ToString();
                
                
            }
            else
            {
                CurrentBalance = "0";
            }
        }
      

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
