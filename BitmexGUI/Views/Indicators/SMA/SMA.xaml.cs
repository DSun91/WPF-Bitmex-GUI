using BitmexGUI.BaseClasses;
using BitmexGUI.Models;
using BitmexGUI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BitmexGUI.Views.Indicators
{
    /// <summary>
    /// Interaction logic for SMA.xaml
    /// </summary>
    public partial class SMA : UserControl
    {
        public SMA()
        {
            InitializeComponent();
        }
        public static T FindParent<T>(DependencyObject child, string name = null) where T : FrameworkElement
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            if (parentObject is T parent && (name == null || parent.Name == name))
                return parent;
            else
                return FindParent<T>(parentObject, name);
        }
        

        // event Handler usato per far vedere il tooltip con il valore della media mobile
        private void Polyline_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos;
            var polyline = sender as Polyline;
            var seriesVm = polyline.DataContext as PointSerie;
            var canvas = FindParent<Canvas>(this, "MainRenderingCanvas");
            if (canvas != null)
            {
                 pos = e.GetPosition(canvas);
                
            }
             
            if (polyline.ToolTip is Border border && border.Child is TextBlock tb)
            {
                double y = pos.Y;
                var value = OHLCandlestickChart.InvMapToScale(y) / 10e7;
                var PriceVal = Math.Round(value, 3).ToString();
           
                tb.Text = $"{seriesVm.Name}\n{PriceVal}";
               
            }
        }

    }
}
