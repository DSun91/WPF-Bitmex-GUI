using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Interfaces
{
    public interface ISettledPrice 
    {
        public string Symbol { get; set; }
        public double SettledPriceValue { get; set; }
        public string Timestamp { get; set; }
    }
}
