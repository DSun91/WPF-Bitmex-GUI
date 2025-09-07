using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Interfaces
{
    public interface ISymbolSelection :INotifyPropertyChanged
    {
        public List<string> Tickers { get; set; }
        public List<string> TimeFrames { get; set; }
        public List<string> Exchanges { get; set; }
        string SelectedTicker { get; set; }
        string SelectedTimeFrame { get; set; }
        string SelectedExchange { get; set; }
    }
}
