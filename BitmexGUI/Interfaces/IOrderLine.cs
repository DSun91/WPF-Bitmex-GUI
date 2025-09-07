using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Interfaces
{
    interface IOrderLine
    {
        public string OrderID { get; set; }
        public decimal Price { get; set; }
        public string Side { get; set; }
        public string Symbol { get; set; }
    }
}
