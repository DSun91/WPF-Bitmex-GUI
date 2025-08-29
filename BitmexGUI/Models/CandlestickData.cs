using System.ComponentModel;
  
    namespace BitmexGUI.Models
    {
        public class CandlestickData : INotifyPropertyChanged
        {
            private string _symbol;
            public string Symbol
            {
                get => _symbol;
                set
                {
                    if (_symbol != value)
                    {
                        _symbol = value;
                        OnPropertyChanged(nameof(Symbol));
                    }
                }
            }

            private double _open;
            public double Open
            {
                get => _open;
                set
                {
                    if (_open != value)
                    {
                        _open = value;
                        OnPropertyChanged(nameof(Open));
                        OnPropertyChanged(nameof(Height)); // depends on Open
                    }
                }
            }

            private double _high;
            public double High
            {
                get => _high;
                set
                {
                    if (_high != value)
                    {
                        _high = value;
                        OnPropertyChanged(nameof(High));
                        OnPropertyChanged(nameof(CenterY)); // depends on High
                    }
                }
            }

            private double _low;
            public double Low
            {
                get => _low;
                set
                {
                    if (_low != value)
                    {
                        _low = value;
                        OnPropertyChanged(nameof(Low));
                        OnPropertyChanged(nameof(CenterY)); // depends on Low
                    }
                }
            }

            private double _close;
            public double Close
            {
                get => _close;
                set
                {
                    if (_close != value)
                    {
                        _close = value;
                        OnPropertyChanged(nameof(Close));
                        OnPropertyChanged(nameof(Height)); // depends on Close
                    }
                }
            }

            private DateTime _timestamp;
            public DateTime Timestamp
            {
                get => _timestamp;
                set
                {
                    if (_timestamp != value)
                    {
                        _timestamp = value;
                        OnPropertyChanged(nameof(Timestamp));
                    }
                }
            }

            private double _posx;
            public double Posx
            {
                get => _posx;
                set
                {
                    if (_posx != value)
                    {
                        _posx = value;
                        OnPropertyChanged(nameof(Posx));
                        OnPropertyChanged(nameof(CenterX)); // Posx affects layout
                    }
                }
            }

            private double _width;
            public double Width
            {
                get => _width;
                set
                {
                    if (_width != value)
                    {
                        _width = value;
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


 
