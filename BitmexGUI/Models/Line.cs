using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Models
{
    public class OrdersLines
    {
        public string OrderID { get; set; }
        public decimal Price { get; set; }
        public string Side { get; set; }
        public string Symbol { get; set; }
    }
    public class PositionsLines
    {
        public int AccountID { get; set; }
        public string Symbol { get; set; }
        public decimal Price { get; set; }
    }
}
