using BitmexGUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.ViewModels
{
    public class CandleStickViewModel : INotifyPropertyChanged
    {
        public CandlestickData Candlestick { get; }

        public CandleStickViewModel(CandlestickData candlestick)
        {
            Candlestick = candlestick;
        }
       
        public string Symbol
        {
            get => Candlestick.Symbol;
            set
            {
                if (Candlestick.Symbol != value)
                {
                    Candlestick.Symbol = value;
                    OnPropertyChanged(nameof(Symbol));
                }
            }
        }

       
        public double Open
        {
            get => Candlestick.Open;
            set
            {
                if (Candlestick.Open != value)
                {
                    Candlestick.Open = value;
                    OnPropertyChanged(nameof(Open));
                    OnPropertyChanged(nameof(Height)); // depends on Open
                }
            }
        }

        
        public double High
        {
            get => Candlestick.High;
            set
            {
                if (Candlestick.High != value)
                {
                    Candlestick.High = value;
                    OnPropertyChanged(nameof(High));
                    OnPropertyChanged(nameof(CenterY)); // depends on High
                }
            }
        }

      
        public double Low
        {
            get => Candlestick.Low;
            set
            {
                if (Candlestick.Low != value)
                {
                    Candlestick.Low = value;
                    OnPropertyChanged(nameof(Low));
                    OnPropertyChanged(nameof(CenterY)); // depends on Low
                }
            }
        }
 
        public double Close
        {
            get => Candlestick.Close;
            set
            {
                if (Candlestick.Close != value)
                {
                    Candlestick.Close = value;
                    OnPropertyChanged(nameof(Close));
                    OnPropertyChanged(nameof(Height)); // depends on Close
                }
            }
        }
         
        public DateTime Timestamp
        {
            get => Candlestick.Timestamp;
            set
            {
                if (Candlestick.Timestamp != value)
                {
                    Candlestick.Timestamp = value;
                    OnPropertyChanged(nameof(Timestamp));
                }
            }
        }

       
        public double Posx
        {
            get => Candlestick.Posx;
            set
            {
                if (Candlestick.Posx != value)
                {
                    Candlestick.Posx = value;
                    OnPropertyChanged(nameof(Posx));
                    OnPropertyChanged(nameof(CenterX)); // Posx affects layout
                }
            }
        }

         
        public double Width
        {
            get => Candlestick.Width;
            set
            {
                if (Candlestick.Width != value)
                {
                    Candlestick.Width = value;
                    OnPropertyChanged(nameof(Width));
                    OnPropertyChanged(nameof(CenterX));
                }
            }
        }

        // Derived properties
        public double Height => Math.Abs(Open - Close) + 1;
        public double CenterY => Math.Abs(High - Low) / 2;
        public double CenterX => Width / 2;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      

         
    }
}
