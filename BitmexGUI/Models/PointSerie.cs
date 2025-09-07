using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BitmexGUI.Models
{
    public class PointSerie
    {
        public string Name { get; set; }    
        public PointCollection Points { get; set; }
        public Brush? Stroke { get; set; } = null;  

        public PointSerie(string name, PointCollection points, Brush stroke)
        {
            Name = name;
            Points = points;
            Stroke = stroke;
        }

    }
}
