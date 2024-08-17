using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Views
{
    
    internal class LocalViewModel
    {
         
        public ObservableCollection<double> HorizontalLines { get; set; }
        public ObservableCollection<double> VerticalLines { get; set; }

        public LocalViewModel()
        {

            int GridSpacing = 50;
            double canvasHeight = 400; // Example height
            double canvasWidth = 700;  // Example width

            HorizontalLines = new ObservableCollection<double>();
            for (double y = 0; y <= canvasHeight; y += GridSpacing)
            {
                HorizontalLines.Add(y);
                
            }

            VerticalLines = new ObservableCollection<double>();
            for (double x = 0; x <= canvasWidth; x += GridSpacing)
            {
                VerticalLines.Add(x);

            }
        }
    }
}
