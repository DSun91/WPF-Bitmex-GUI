using BitmexGUI.Models;
using BitmexGUI.Services.Implementations;
using BitmexGUI.Services.Interfaces;
using BitmexGUI.ViewModels;
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
    /// Interaction logic for LivePriceChart.xaml
    /// </summary>
    public partial class LivePriceChart : UserControl
    { 
         
        private Point clickPositionCanvas;
        public static bool isDraggingCanvas = false;  
        
        public LivePriceChart()
        {

            InitializeComponent();
            
            
            this.Loaded += LivePriceChart_Loaded;
           
        }
         
        private void LivePriceChart_Loaded(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.MouseLeftButtonDown += MoveCanvas_MouseLeftButtonDown;
            DrawingCanvas.MouseMove += MoveCanvas_MouseMove;
            DrawingCanvas.MouseLeftButtonUp += MoveCanvas_MouseLeftButtonUp;

            DrawingCanvas.MouseDown += DrawingCanvas_MouseMiddleButtonDown;
            DrawingCanvas.MouseUp += DrawingCanvas_MouseMiddleButtonUp;
            DrawingCanvas.MouseWheel += DrawingCanvas_MouseWheelEvents;
            OrderLinesCanvas.ParentCanvas = MainRenderingCanvas; // Set the parent canvas for OrderLinesCanvas
        }
        public void MoveCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                isDraggingCanvas = true;
                OrderLines.isDraggingOrderLine = false;
                clickPositionCanvas = e.GetPosition(canvas);

                canvas.CaptureMouse(); // Capture the mouse to receive mouse events even when the cursor is outside the label
                
            }
        }

        public void MoveCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDraggingCanvas && !OrderLines.isDraggingOrderLine && sender is Canvas canvas)
            {

                var mousePos = e.GetPosition(DrawingCanvas);



                double deltaX = mousePos.X - clickPositionCanvas.X;
                double deltaY = mousePos.Y - clickPositionCanvas.Y;

                // Adjust the margin of MainRenderingCanvas based on the mouse movement
                // If dragging right (positive deltaX), increase the left margin and decrease the right margin
                // If dragging left (negative deltaX), decrease the left margin and increase the right margin
                MainRenderingCanvas.Margin = new Thickness(
                MainRenderingCanvas.Margin.Left,
                MainRenderingCanvas.Margin.Top + deltaY,
                MainRenderingCanvas.Margin.Right,
                MainRenderingCanvas.Margin.Bottom - deltaY
            );

                TicksCanvasSettled.Margin = new Thickness(
                TicksCanvasSettled.Margin.Left,
                TicksCanvasSettled.Margin.Top + deltaY,
                TicksCanvasSettled.Margin.Right,
                TicksCanvasSettled.Margin.Bottom - deltaY
            );

                TicksCanvasMarket.Margin = new Thickness(
                TicksCanvasMarket.Margin.Left,
                TicksCanvasMarket.Margin.Top + deltaY,
                TicksCanvasMarket.Margin.Right,
                TicksCanvasMarket.Margin.Bottom - deltaY
            );
                OrderLinesCanvas.Margin = new Thickness(
                OrderLinesCanvas.Margin.Left,
                OrderLinesCanvas.Margin.Top + deltaY,
                OrderLinesCanvas.Margin.Right,
                OrderLinesCanvas.Margin.Bottom - deltaY
            );
                PositionsLinesCanvas.Margin = new Thickness(
                PositionsLinesCanvas.Margin.Left,
                PositionsLinesCanvas.Margin.Top + deltaY,
                PositionsLinesCanvas.Margin.Right,
                PositionsLinesCanvas.Margin.Bottom - deltaY
           );
                if (deltaX > 0 && CandlestickChart.CandlesToView < CandlestickChart.TotalCandlesCount)
                {

                    int deltaCandles = (int)Math.Ceiling(Math.Abs(deltaX) / CandlestickChart.candleWidth);


                    CandlestickChart.CandlesToView += deltaCandles;
                }
                if (deltaX < 0 && CandlestickChart.CandlesToView > 0)
                {
                    int deltaCandles = (int)Math.Ceiling(Math.Abs(deltaX) / CandlestickChart.candleWidth);

                    CandlestickChart.CandlesToView -= deltaCandles;
                }

                // Update the click position for the next movement

                if (CandlestickChart.CandlesToView > CandlestickChart.TotalCandlesCount)
                {
                    CandlestickChart.CandlesToView = CandlestickChart.TotalCandlesCount - 1;
                }

                // refresh or redraw your data as needed
                var vm = DataContext as MainViewModel;
                vm.priceStreamViewModel.RefreshScaledPriceData("FromEvents");
                clickPositionCanvas = mousePos;
            }
        }
        public void MoveCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Canvas Canvas)
            {
                isDraggingCanvas = false;
                Canvas.ReleaseMouseCapture(); // Release the mouse capture when dragging is finished


            }

        }
        public void DrawingCanvas_MouseWheelEvents(object sender, MouseWheelEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            // Get the position of the mouse click relative to the Canvas
            if (e.Delta > 0)
            { 
                //CandlestickChart.ScaleFactor -= 0.01;
                CandlestickChart.candleWidth += 0.2;
                CandlestickChart.CandlesInterspace += 0.2; 
                vm.priceStreamViewModel.RefreshScaledPriceData("FromEvents");

            }
            if (e.Delta < 0)
            {
                 
                CandlestickChart.candleWidth -= 0.2;
                CandlestickChart.CandlesInterspace -= 0.2; 
                if (CandlestickChart.candleWidth < 0.1)
                {
                    CandlestickChart.candleWidth = 0.1;
                    CandlestickChart.CandlesInterspace = 0.1;

                }
                
                vm.priceStreamViewModel.RefreshScaledPriceData("FromEvents");

            }

            e.Handled = true;
        }
         
        private VisualHost _lineVisualHost;
        public void DrawingCanvas_MouseMiddleButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
            {

                // Remove previous line if it exists
                if (_lineVisualHost != null)
                {
                    MainRenderingCanvas.Children.Remove(_lineVisualHost);
                    _lineVisualHost = null;
                }

                Point clickPosition = e.GetPosition(MainRenderingCanvas);
                double y = clickPosition.Y;
                var value = CandlestickChart.InvMapToScale(y)/10e7;
                var PriceVal = Math.Round(value, 3).ToString();
                // Create a new DrawingVisual and render the line
                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen())
                {
                    Pen linePen = new Pen(Brushes.Blue, 1)
                    {
                        DashStyle = new DashStyle(new double[] { 1, 2 }, 0)
                    };
                    dc.DrawLine(linePen, new Point(0, y), new Point(DrawingCanvas.Width, y));
                    FormattedText ft = new FormattedText(PriceVal, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Klavika"), 12, Brushes.Blue);
                    Point textPosition = clickPosition;
                    textPosition.Y -= 15;
                    textPosition.X -= 15;
                    dc.DrawText(ft, textPosition);
                }

                // Add the visual to the canvas and store it in the field
                _lineVisualHost = new VisualHost { Visual = visual };
                MainRenderingCanvas.Children.Add(_lineVisualHost);

                // Display the price
                var vm = DataContext as MainViewModel;
                vm.entryViewModel.EntryPrice = double.Parse( PriceVal);
                
            }
        }

        public void DrawingCanvas_MouseMiddleButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                // Remove the visual when the middle mouse button is released
                if (_lineVisualHost != null)
                {
                    MainRenderingCanvas.Children.Remove(_lineVisualHost);
                    _lineVisualHost = null;
                }


            }
        }

       
    }
}
