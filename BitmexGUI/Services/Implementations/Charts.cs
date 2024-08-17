using BitmexGUI.Services.Abstract;
using BitmexGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;
using System.Configuration;
using BitmexGUI.Models;
using System.Windows.Documents;
using BitmexGUI.Views;
using System.Xml.Linq;

namespace BitmexGUI.Services.Implementations
{
    internal class CandlestickChart : AbstractCharts
    {
        public static double minOriginal = 0;
        public static double maxOriginal = 70000;
        public event Action<OrdersLines> OrderLinesUpdated;
        // Target range
        public static double minTarget = 0;
        public static double maxTarget = 400;
        //public event Action OrderLinesUpdated;

        public double interspace = 10; // Increased for better visibility
        public double candleWidth = 9; // Increased for better visibility
        double xOffset = 20;
        private MainViewModel ViewModel;
        private Canvas DrawingCanvas;
        public int InView;
        private int TotalCached;

        private bool isDragging = false;
        private Point clickPosition;
        public CandlestickChart(MainViewModel viewModel, Canvas drawingCanvas, int InviewN, int TotalN)
        {
            InView = InviewN;
            TotalCached = TotalN;
            ViewModel = viewModel;
            DrawingCanvas = drawingCanvas;



        }
        public void RefreshCanvas()
        {

            //DrawingCanvas.Children.Clear();
            //ReleaseMouseCaptureIfNeeded();
            DrawGrid();
            //DrawBorder();

            //lock (ViewModel.PriceData)
            //{
            //    if (ViewModel.PriceData.Count == 0)
            //    {
            //        Console.WriteLine("No candle data available.");
            //        return;
            //    }

            //    var allValues = ViewModel.PriceData.SelectMany(data => new[] { data.Open, data.High, data.Low, data.Close });
            //    minOriginal = allValues.Min();
            //    maxOriginal = allValues.Max();

            //    double padding = (maxOriginal - minOriginal) * 0.2;
            //    minOriginal -= padding;
            //    maxOriginal += padding;

            //    int i = 0;

            //    int counter = 0;
            //    //MessageBox.Show(_ViewModel.SettledPriceData.Count.ToString());
            //    foreach (var candleData in ViewModel.PriceData)
            //    {
            //        if(counter >= TotalCached - InView)
            //        {
            //            var candleValues = candleData;

            //            var open = MapToScale(candleData.Open);
            //            var high = MapToScale(candleData.High);
            //            var low = MapToScale(candleData.Low);
            //            var close = MapToScale(candleData.Close);
            //            //this.CurrentPriceTextBlock.Text =Math.Round(candleValues[3],3).ToString();
            //            //this.CurrentPriceTextBlock.Margin = new Thickness(400, close-40, 0, 0);

            //            DrawCharts(i * interspace, open, high, low, close, candleWidth, InView);
            //            //MessageBox.Show(candleData.Timestamp.ToString());
            //            i++;
            //        }
            //        counter++;
            //    }
            //    //MessageBox.Show(counter.ToString()+" "+i.ToString());
            //}
        }
        //    public void DrawCharts(double centerX, double open, double high, double low, double close, double width,int InView)
        //    {

        //        int fontsize = 14;
        //        double mappedSettledPrice = InvMapToScale(MapToScale(ViewModel.SettledPriceData.Last().SettledPriceValue));

        //        //bool isCurrentCandle = (centerX == (ViewModel.PriceData.Count - 1) * interspace);

        //        bool isCurrentCandle = (centerX == (InView-1) * interspace);


        //        string timeframe = ConfigurationManager.AppSettings["Timeframe"];

        //        Label TimeFrame = new Label
        //        {
        //            Content ="Timeframe: "+ timeframe,
        //            Height = 40,
        //            Width = 100,

        //        };
        //        Canvas.SetLeft(TimeFrame, 0 ); // Position horizontally to the right of the candlestick
        //        Canvas.SetTop(TimeFrame, 0);
        //        DrawingCanvas.Children.Add(TimeFrame);



        //        Line SettPriceLine = new Line
        //        {
        //            X1 = centerX + xOffset-10,
        //            Y1 =MapToScale(ViewModel.SettledPriceData.Last().SettledPriceValue),
        //            X2 = centerX + xOffset+10,
        //            Y2 = MapToScale(ViewModel.SettledPriceData.Last().SettledPriceValue),
        //            Stroke = Brushes.DarkViolet,
        //            StrokeThickness = 2
        //        };

        //        Line ClosePriceLine = new Line
        //        {
        //            X1 = centerX + xOffset - 10,
        //            Y1 = close,
        //            X2 = centerX + xOffset + 10,
        //            Y2 = close,
        //            Stroke = Brushes.Black,
        //            StrokeThickness = 2
        //        };

        //        TextBlock SettPriceLabel = new TextBlock
        //        {

        //            Text =mappedSettledPrice.ToString("F2")+$" = {Math.Round(mappedSettledPrice - InvMapToScale(close),2)}", // Format the close value to 2 decimal places
        //            Foreground = Brushes.DarkViolet,
        //            Background = Brushes.Transparent,
        //            FontSize = fontsize,
        //            FontWeight = FontWeights.Bold,
        //            Padding = new Thickness(2) // Optional padding for better visibility
        //        };


        //        Line wick = new Line
        //        {
        //            X1 = centerX + xOffset,
        //            Y1 = high,
        //            X2 = centerX + xOffset,
        //            Y2 = low,
        //            Stroke = isCurrentCandle ? Brushes.Blue : Brushes.Black,
        //            StrokeThickness = isCurrentCandle ? 2 : 1
        //        };
        //        DrawingCanvas.Children.Add(wick);

        //        Rectangle body = new Rectangle
        //        {
        //            Width = width,
        //            Height = Math.Max(1, Math.Abs(close - open)), // Ensure minimum height of 1
        //            Fill = close < open ? Brushes.Green : Brushes.Red,
        //            Stroke = isCurrentCandle ? Brushes.Blue : Brushes.Transparent,
        //            StrokeThickness = isCurrentCandle ? 2 : 0
        //        };
        //        Canvas.SetLeft(body, centerX + xOffset - width / 2);
        //        Canvas.SetTop(body, Math.Min(open, close));
        //        DrawingCanvas.Children.Add(body);


        //        TextBlock closeLabel = new TextBlock
        //        {
        //            Text = InvMapToScale(close).ToString("F2")+ " -", // Format the close value to 2 decimal places
        //            Foreground = Brushes.Black,
        //            Background = Brushes.Transparent,
        //            FontSize = fontsize,
        //            FontWeight = FontWeights.Bold,
        //            Padding = new Thickness(2) // Optional padding for better visibility
        //        };
        //        if (isCurrentCandle)
        //        {

        //            Canvas.SetLeft(closeLabel, centerX + xOffset + width / 2 + 5); // Position horizontally to the right of the candlestick
        //            Canvas.SetTop(closeLabel, close ); // Position vertically at the 'close' value

        //            Canvas.SetLeft(SettPriceLabel, centerX + xOffset + 80); // Position horizontally to the right of the candlestick
        //            Canvas.SetTop(SettPriceLabel, close); // Position vertically at the 'close' value

        //            DrawingCanvas.Children.Add(SettPriceLabel);
        //            DrawingCanvas.Children.Add(closeLabel);


        //            DrawingCanvas.Children.Add(SettPriceLine);
        //            DrawingCanvas.Children.Add(ClosePriceLine);

        //            foreach (OrdersLines orderline in ViewModel.OrderLines)
        //            {
        //                double ypos = Math.Round(MapToScale(double.Parse(orderline.Price.ToString())));

        //                Label OrderLabel = new Label
        //                {
        //                    Name="OrderLabel",
        //                    Tag= orderline.OrderID.ToString(),
        //                    Content = orderline.Side + " " + orderline.Symbol + ": " + Math.Round(orderline.Price).ToString(),
        //                    Foreground = Brushes.DarkRed,
        //                    Background = Brushes.SkyBlue,
        //                    FontSize = 12,
        //                    FontWeight = FontWeights.Bold,
        //                    FontStyle = FontStyles.Italic, 
        //                    Padding = new Thickness(2)

        //                };

        //                OrderLabel.MouseLeftButtonDown += MyLabel_MouseLeftButtonDown;
        //                OrderLabel.MouseMove += MyLabel_MouseMove;
        //                OrderLabel.MouseLeftButtonUp += MyLabel_MouseLeftButtonUp;

        //                Line OrderPriceLine = new Line
        //                {
        //                    Name = "OrderLine",
        //                    Tag = orderline.OrderID,
        //                    X1 = 0,
        //                    Y1 = ypos,
        //                    X2 = DrawingCanvas.Width,
        //                    Y2 = ypos,
        //                    Stroke = Brushes.Red,
        //                    StrokeThickness = 1,
        //                    //StrokeDashArray = new DoubleCollection { 3, 5 }
        //                };




        //                DrawingCanvas.Children.Add(OrderPriceLine);
        //                DrawingCanvas.Children.Add(OrderLabel);
        //                Canvas.SetLeft(OrderLabel, DrawingCanvas.Width * 3 / 4); // Position horizontally to the right of the candlestick
        //                Canvas.SetTop(OrderLabel, ypos);


        //            }

        //        }


        //    }

        //    private void ReleaseMouseCaptureIfNeeded(Line OrderPriceLine,Label OrderLabel,double ypos)
        //    {
        //        if (Mouse.Captured is Label label)
        //        {
        //            label.ReleaseMouseCapture();
        //            isDragging = false; 

        //        }
        //    }
        //    private void MyLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //    {
        //        if (sender is Label label)
        //        {
        //            isDragging = true;
        //            clickPosition = e.GetPosition(label);
        //            label.CaptureMouse(); // Capture the mouse to receive mouse events even when the cursor is outside the label
        //        }
        //    }
        //    private void MyLabel_MouseMove(object sender, MouseEventArgs e)
        //    {
        //        if (isDragging && sender is Label label)
        //        {
        //            var mousePos = e.GetPosition(DrawingCanvas);


        //            double top = mousePos.Y - clickPosition.Y;

        //            if (top < 0) top = 0;


        //            if (top + label.ActualHeight > DrawingCanvas.ActualHeight)
        //                top = DrawingCanvas.ActualHeight - label.ActualHeight;

        //            var existingLine = ViewModel.OrderLines.FirstOrDefault(p => p.OrderID.Equals(label.Tag));

        //            if (existingLine != null)
        //            {
        //                existingLine.Price = (decimal)InvMapToScale(top);
        //                OrderLinesUpdated?.Invoke(existingLine);


        //            }


        //            //foreach (Line labelelem in DrawingCanvas.Children.OfType<Line>())
        //            //{
        //            //    if (labelelem.Name.ToLower().Contains("Orderline".ToLower()))
        //            //    {

        //            //        double y = Canvas.GetTop(labelelem);
        //            //        Canvas.SetTop(labelelem, top);
        //            //        //MessageBox.Show(labelelem.Name.ToString()+" "+ top);
        //            //        //MessageBox.Show(x.ToString() + " " + y.ToString());
        //            //    }


        //            //}

        //            Canvas.SetTop(label, top);


        //        }
        //    }

        //    private void MyLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //    {
        //        if (sender is Label label)
        //        {
        //            isDragging = false;
        //            label.ReleaseMouseCapture(); // Release the mouse capture when dragging is finished
        //            var mousePos = e.GetPosition(DrawingCanvas);



        //        }
        //    }


        public static double MapToScale(double originalValue,string fromewhere="")
        {
            //return maxTarget - ((originalValue - minOriginal) / (maxOriginal - minOriginal)) * (maxTarget - minTarget);
            if(fromewhere.Length > 0)
            {
               
                //MessageBox.Show((maxTarget - ((originalValue - minOriginal) * (maxTarget - minTarget)) / (maxOriginal - minOriginal) + minTarget).ToString());
            }
            
            return maxTarget - ((originalValue - minOriginal) * (maxTarget - minTarget)) / (maxOriginal - minOriginal) + minTarget;
        }

        public static double InvMapToScale(double Value)
        {
            return ((maxTarget - Value) / (maxTarget - minTarget)) * (maxOriginal - minOriginal) + minOriginal;
        }

        private void DrawGrid()
        {
            // Clear existing grid lines if any

            int GridSpacing = 50;
            // Horizontal lines
            for (double y = 0; y <= DrawingCanvas.Height; y += GridSpacing)
            {
                Line horizontalLine = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = DrawingCanvas.Width,
                    Y2 = y,
                    Stroke = Brushes.LightBlue,
                    StrokeThickness = 2,
                    StrokeDashArray = new DoubleCollection { 2, 2 } // Dotted line
                };
                DrawingCanvas.Children.Add(horizontalLine);
            }

            // Vertical lines
            for (double x = 0; x <= DrawingCanvas.Width; x += GridSpacing)
            {
                Line verticalLine = new Line
                {
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = DrawingCanvas.Height,
                    Stroke = Brushes.LightBlue,
                    StrokeThickness = 2,
                    StrokeDashArray = new DoubleCollection { 2, 2 } // Dotted line
                };
                DrawingCanvas.Children.Add(verticalLine);
            }
        }

        //    private void DrawBorder()
        //    {
        //        Border border = new Border()
        //        {
        //            BorderThickness = new Thickness(2),
        //            CornerRadius = new CornerRadius(8),
        //            BorderBrush = Brushes.Black, // Set the color of the border

        //            Height = DrawingCanvas.ActualHeight, // Use ActualHeight to get the current size
        //            Width = DrawingCanvas.ActualWidth // Use ActualWidth to get the current size
        //        };
        //        Canvas.SetLeft(border, 0);
        //        Canvas.SetTop(border, 0);
        //        Panel.SetZIndex(border, -1);

        //        DrawingCanvas.Children.Add(border);
        //        DrawingCanvas.Clip = new RectangleGeometry(new Rect(0, 0, DrawingCanvas.ActualWidth, DrawingCanvas.ActualHeight), 8, 8);
        //    }

        //}

        internal class HeikinAshiChart : AbstractCharts
        {
            public HeikinAshiChart(int n) { }
        }
    }
}
