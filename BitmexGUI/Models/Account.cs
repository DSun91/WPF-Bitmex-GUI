using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Models
{
    public class Account
    {
        public double Balance { get; set; }
    }

    public class Instrument
    {
        public string Name { get; set; }

        public double makerFee { get; set; }

        public double takerFee { get; set; }
    }
}
