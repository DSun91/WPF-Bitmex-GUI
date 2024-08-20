using BitmexGUI.Models;
using BitmexGUI.Services.Implementations;
using BitmexGUI.Services.Interfaces;
using BitmexGUI.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Security.Principal;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Globalization;
using System.Windows.Data; 


namespace BitmexGUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {



        
        private MainViewModel ViewModel => (MainViewModel)DataContext;
        private MainViewModel viewModel;
        // Target range
        private CandlestickChart CandleStickView;
        public event Action<string> OrderLinesUpdated;
        public event Action<string> CancelOrder;


        public string vale;

        private string AmendingOrderID;

        public MainWindow()
        {
            
             
            InitializeComponent();
             
            viewModel = new MainViewModel(CandlestickChart.CachedCandles); 
            DataContext = viewModel;
            
            
            CandleStickView = new CandlestickChart();

            DrawingCanvas.MouseRightButtonDown += DrawingCanvas_MouseRightButtonDown;
            DrawingCanvas.MouseWheel += DrawingCanvas_MouseWheelEvents;
            DrawingCanvas.MouseLeftButtonDown += VerticalZoom_MouseLeftButtonDown;
            DrawingCanvas.MouseMove += VerticalZoom_MouseMove;
            DrawingCanvas.MouseLeftButtonUp += VerticalZoom_MouseLeftButtonUp;
            this.OrderLinesUpdated += (AmendingOrderID) =>
            {
                if (DataContext is MainViewModel viewModel)
                {
                    viewModel.HandleOrderLineUpdate(AmendingOrderID);
                }
            };
            this.Loaded += MainWindow_Loaded;


             this.CancelOrder += (OrderCanceled) =>
            {
                if (DataContext is MainViewModel viewModel)
                {
                    viewModel.CancelOrder(OrderCanceled);
                }
            };
             
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.GetBalances();
            
            viewModel.StartPriceFeed();

        }

        public void test(string testing)
        {
            MessageBox.Show("here");
        }
        public static bool isDraggingOrderLine = false;
      
        private Point clickPositionLabel;


    
        /// //////////////////////////////////////////////  ORDER LINE DRAGGING
       
        private void OrderTag_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label label)
            {
                isDraggingOrderLine = true;
                clickPositionLabel = e.GetPosition(label);
                label.CaptureMouse(); // Capture the mouse to receive mouse events even when the cursor is outside the label
            }
        }
        private void OrderTag_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label label)
            {
                
                label.ReleaseMouseCapture(); // Release the mouse capture when dragging is finished
                
                isDraggingOrderLine = false;
            }

            OrderLinesUpdated?.Invoke(AmendingOrderID);
        }
        
        private void OrderTag_MouseMove(object sender, MouseEventArgs e) 
            {
                if (isDraggingOrderLine && sender is Label label)
                {
                    var mousePos = e.GetPosition(DrawingCanvas);


                    double top = mousePos.Y - clickPositionLabel.Y;

                    if (top < 0) top = 0;


                    if (top + label.ActualHeight > DrawingCanvas.ActualHeight)
                        top = DrawingCanvas.ActualHeight - label.ActualHeight;


                    AmendingOrderID = label.Tag.ToString();
                    var existingLine = ViewModel.OrdersLines.FirstOrDefault(p => p.OrderID.Equals(label.Tag.ToString()));

                    if (existingLine != null)
                    {
                        var index = ViewModel.OrdersLines.IndexOf(existingLine);

                     
                       OrderLine tempLine = new OrderLine 
                       {

                          OrderID=existingLine.OrderID,
                          Price= (decimal)top,
                          Side=existingLine.Side,
                          Symbol=existingLine.Symbol 
                       };


                        if (index >= 0)
                        {
                            ViewModel.OrdersLines[index] = tempLine; // Update the item in the ObservableCollection
                            
                        }
                     

                    }
                 
                    Canvas.SetTop(label, top);


                }
            }

        /// //////////////////////////////////////////////  ORDER LINE DRAGGING

        /// //////////////////////////////////////////////  CANDLESTICK DRAGGING
        private Point clickPositionCanvas;
        private bool isMoving = false;
        private void VerticalZoom_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle canvas)
            {
                isMoving = true;
                clickPositionCanvas = e.GetPosition(canvas);
               
                canvas.CaptureMouse(); // Capture the mouse to receive mouse events even when the cursor is outside the label
               
            }
        }
        
        private void VerticalZoom_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMoving && sender is Rectangle canvas)
            {
                var mousePos = e.GetPosition(DrawingCanvas);

                

                double direction = mousePos.X - clickPositionCanvas.X;

                if(direction >0 ) 
                {
                    for (int i=0 ;i < viewModel.PriceData.Count;i++)
                    {
                        viewModel.PriceData[i].Posx = viewModel.PriceData[i].Posx + 1;

                    }
                    viewModel.RefreshScaledPriceData();
                }
                else
                {

                }

           

            }
        }

        private void VerticalZoom_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Canvas Canvas)
            {
                isMoving = false;
                Canvas.ReleaseMouseCapture(); // Release the mouse capture when dragging is finished
                 

            }

        }  //    foreach (OrderLine x in ViewModel.OrdersLines)


        /// //////////////////////////////////////////////  CANDLESTICK DRAGGING  



        /// ////////////////////////////////////////////// MOUSEWHEEL VERTICAL ZOOM
        private void DrawingCanvas_MouseWheelEvents(object sender, MouseWheelEventArgs e)
        {
            // Get the position of the mouse click relative to the Canvas
            if (e.Delta < 0)
            {

                CandlestickChart.ScaleFactor += 0.1; 
                viewModel.RefreshScaledPriceData();
               

            }
            if (e.Delta > 0)
            {

                CandlestickChart.ScaleFactor -= 0.1;
                viewModel.RefreshScaledPriceData();

            } 
        }

        /// ////////////////////////////////////////////// MOUSEWHEEL VERTICAL ZOOM
        private void DrawingCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get the position of the mouse click relative to the Canvas
            Point clickPosition = e.GetPosition(DrawingCanvas);

            // Extract X and Y coordinates
            double x = clickPosition.X;
            double y = clickPosition.Y;

            Line priceLine = new Line
            {
                X1 = 0,
                Y1 = y,
                X2 = DrawingCanvas.Width,
                Y2 = y,
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 2, 2 }
            };
            DrawingCanvas.Children.Add(priceLine);
            Entryprice.Text = Math.Round(CandlestickChart.InvMapToScale(y), 3).ToString();

        }
         
        private void AmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider sld = sender as Slider;
            EntryAmount.Text=Math.Round(sld.Value,2).ToString();
            BalancePercent.Content=Math.Round(sld.Value * 100 / viewModel.AccountInfos[0].Balance, 2).ToString()+" %";
        }
        
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
             
          
        }

        private void BitmexSettled_Click(object sender, RoutedEventArgs e)
        {
             
            Entryprice.Text = Math.Round(viewModel.SettledPriceData.Last().SettledPriceValue, 3).ToString();
          

        }

        private void CancleOpenOrder(object sender, RoutedEventArgs e)
        {
            Button Btn=sender as Button;

            if (Btn != null)
            {
                CancelOrder?.Invoke(Btn.Tag.ToString());
            }
             


        }

    }
    
}
