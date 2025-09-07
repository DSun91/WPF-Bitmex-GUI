using BitmexGUI.BaseClasses;
using BitmexGUI.Models;
using BitmexGUI.ViewModels;
using BitmexGUI.ViewModels.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BitmexGUI.Views
{
    /// <summary>
    /// Interaction logic for OrderLines.xaml
    /// </summary>
    public partial class OrderLines : UserControl
    {
       
        private string AmendingOrderID;
        public static bool isDraggingOrderLine = false;  
        private Point clickPositionLabel;
        public Canvas ParentCanvas { get; set; }

        public OrderLines()
        {
            InitializeComponent();
            
        }
        
        

        private void OrderTag_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label label)
            { 
                isDraggingOrderLine = true;
                clickPositionLabel = e.GetPosition(label);
                label.CaptureMouse(); // Capture the mouse to receive mouse events even when the cursor is outside the label
            }
        }



        private void OrderTag_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label label)
            {

                label.ReleaseMouseCapture(); // Release the mouse capture when dragging is finished 
                isDraggingOrderLine = false;
            }
            var viewModel = DataContext as MainViewModel; 
            viewModel.ordersViewModel.HandleOrderLineUpdate(AmendingOrderID); 
        }


        private void OrderTag_MouseMove(object sender, MouseEventArgs e)
        {

            if (isDraggingOrderLine && sender is Label label)
            {
                var mousePos = e.GetPosition(ParentCanvas);


                double top = mousePos.Y - clickPositionLabel.Y;

                if (top < 0) top = 0;


                AmendingOrderID = label.Tag.ToString();

                var vm = DataContext as MainViewModel;
                var existingLine = vm.ordersViewModel.OrdersLines.FirstOrDefault(p => p.OrderID.Equals(label.Tag.ToString()));

                if (existingLine != null)
                {
                    var index = vm.ordersViewModel.OrdersLines.IndexOf(existingLine);

                    OrderLine tempLine = new OrderLine
                    {

                        OrderID = existingLine.OrderID,
                        Price = (decimal)(Math.Round(OHLCandlestickChart.InvMapToScale(double.Parse((top).ToString())) / 100000000, 2)),
                        Side = existingLine.Side,
                        Symbol = existingLine.Symbol
                    };


                    if (index >= 0)
                    {
                        vm.ordersViewModel.OrdersLines[index] = tempLine; // Update the item in the ObservableCollection

                    }


                }

                Canvas.SetTop(label, top);


            }
        }
    }
}
