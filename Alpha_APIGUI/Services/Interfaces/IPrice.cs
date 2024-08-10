using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Services.Interfaces
{
    internal interface IPrice
    {
        async void GetPriceREST() { }
         
        async void GetPriceWSS() { }
    }
}
