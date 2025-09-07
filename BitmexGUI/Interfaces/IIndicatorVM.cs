using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Interfaces
{
    public interface IIndicatorVM
    {
        int Period { get; set; }
        string Name { get; set; }

        public event Action SendRequestAddtoIndicatorsCollection; 
        public void RaiseSendRequestAddtoIndicatorsCollection();
    }
}
