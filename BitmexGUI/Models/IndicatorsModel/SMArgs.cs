using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BitmexGUI.Models.IndicatorsModel
{
    public class SMArgs
    {
        public int Period { get; set; }
        public Brush Color { get; set; }
        public string Name { get; set; }
        public SMArgs(int period, Brush color,string name)
        {
            Period = period;
            Color = color;
            Name = name;
        }
    }

}
