using BitmexGUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BitmexGUI.ViewModels
{
    public class SymbolSelectionViewModel:ISymbolSelection
    {
        public Action SymbolOrTimeFrameSelected { get; set; }// this is attached in the main viewmodel to execute refreshvm
        public SymbolSelectionViewModel() 
        {
            _selectedTicker = Tickers.FirstOrDefault() ?? string.Empty;
            _selectedTimeframe = TimeFrames.FirstOrDefault() ?? string.Empty;
            _selectedExchange = Exchanges.FirstOrDefault() ?? string.Empty;
        }
        
        public List<string> Tickers { get; set; } = new List<string> { "BTCUSDT", "BTCUSD", "ETHUSDT" };
        private string _selectedTicker;

      
        public string SelectedTicker
        {
            get => _selectedTicker;
            set
            {
                _selectedTicker = value; 
                OnPropertyChanged(nameof(SelectedTicker));
                SymbolOrTimeFrameSelected?.Invoke();
            }
        }

        public List<string> TimeFrames { get; set; } = new List<string> { "1m", "5m", "15m", "1h", "4h", "1d", "1w" };
        private string _selectedTimeframe;


        public string SelectedTimeFrame
        {
            get => _selectedTimeframe;
            set
            {
                _selectedTimeframe = value;
                OnPropertyChanged(nameof(SelectedTimeFrame));
                SymbolOrTimeFrameSelected?.Invoke();
            }
        }

        public List<string> Exchanges { get; set; } = new List<string> { "Bitmex" };
        private string _selectedExchange;


        public string SelectedExchange
        {
            get => _selectedExchange;
            set
            {
                _selectedExchange = value;
                OnPropertyChanged(nameof(SelectedExchange));
            }
        }
         

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
