using BitmexGUI.Models;
using BitmexGUI.Services.Implementations;
using BitmexGUI.Services.Interfaces;
using BitmexGUI.ViewModels;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Reflection.Metadata;
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

namespace BitmexGUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double minOriginal = 0;
        private double maxOriginal = 80000;
        private MainViewModel ViewModel => (MainViewModel)DataContext;
        // Target range
        private double minTarget = 0;
        private double maxTarget = 500;

        private int initialcandles = 85;
        private int maxcandles = 136;
        double interspace = 13; // Increased for better visibility
        double candleWidth = 8; // Increased for better visibility
        double xOffset = 20;
        public MainWindow()
        {
            
             
            InitializeComponent(); 
            var viewModel = new MainViewModel();
            DataContext = viewModel;
            viewModel.StartPriceFeed();
            viewModel.PriceDataUpdated += OnPriceDataUpdated;

        }
        double MapToScale(double originalValue)
        {
            //return maxTarget - ((originalValue - minOriginal) / (maxOriginal - minOriginal)) * (maxTarget - minTarget);
            return maxTarget - ((originalValue - minOriginal) * (maxTarget - minTarget)) / (maxOriginal - minOriginal) + minTarget;
        }

        double InvMapToScale(double Value)
        {
            return ((maxTarget - Value) / (maxTarget - minTarget)) * (maxOriginal - minOriginal) + minOriginal;
        }

        private void RefreshCanvas()
        {
            DrawingCanvas.Children.Clear();
            DrawGrid();
            DrawBorder();
            lock (ViewModel.PriceData)
            {
                if (ViewModel.PriceData.Count == 0)
                {
                    Console.WriteLine("No candle data available.");
                    return;
                }

                var allValues = ViewModel.PriceData.SelectMany(data => new[] { data.Open, data.High, data.Low, data.Close });
                minOriginal = allValues.Min();
                maxOriginal = allValues.Max();

                double padding = (maxOriginal - minOriginal) * 0.05;
                minOriginal -= padding;
                maxOriginal += padding;

                int i = 0;

                foreach (var candleData in ViewModel.PriceData)
                {
                    var candleValues = candleData;

                    var open = MapToScale(candleData.Open);
                    var high = MapToScale(candleData.High);
                    var low = MapToScale(candleData.Low);
                    var close = MapToScale(candleData.Close);
                    //this.CurrentPriceTextBlock.Text =Math.Round(candleValues[3],3).ToString();
                    //this.CurrentPriceTextBlock.Margin = new Thickness(400, close-40, 0, 0);
                    AddCandlesticks(i * interspace, open, high, low, close, candleWidth);
                    i++;
                }
            }
        }
        private void OnPriceDataUpdated()
        {
            // Refresh the canvas when price data is updated
            RefreshCanvas();
        }
        private void AddCandlesticks(double centerX, double open, double high, double low, double close, double width)
        {

             
            bool isCurrentCandle = (centerX == (ViewModel.PriceData.Count - 1) * interspace);

            Line wick = new Line
            {
                X1 = centerX + xOffset,
                Y1 = high,
                X2 = centerX + xOffset,
                Y2 = low,
                Stroke = isCurrentCandle ? Brushes.Blue : Brushes.Black,
                StrokeThickness = isCurrentCandle ? 2 : 1
            };
            DrawingCanvas.Children.Add(wick);

            Rectangle body = new Rectangle
            {
                Width = width,
                Height = Math.Max(1, Math.Abs(close - open)), // Ensure minimum height of 1
                Fill = close < open ? Brushes.Green : Brushes.Red,
                Stroke = isCurrentCandle ? Brushes.Blue : Brushes.Transparent,
                StrokeThickness = isCurrentCandle ? 2 : 0
            };
            Canvas.SetLeft(body, centerX + xOffset - width / 2);
            Canvas.SetTop(body, Math.Min(open, close));
            DrawingCanvas.Children.Add(body);


            TextBlock closeLabel = new TextBlock
            {
                Text = InvMapToScale(close).ToString("F2"), // Format the close value to 2 decimal places
                Foreground = Brushes.Black,
                Background = Brushes.Transparent,
                FontSize = 14,
                Padding = new Thickness(2) // Optional padding for better visibility
            };
            if (isCurrentCandle)
            {
                Canvas.SetLeft(closeLabel, centerX + xOffset + width / 2 + 5); // Position horizontally to the right of the candlestick
                Canvas.SetTop(closeLabel, close); // Position vertically at the 'close' value
                DrawingCanvas.Children.Add(closeLabel);
            }

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

        private void DrawBorder()
        {
            Border border = new Border()
            {
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(8),
                BorderBrush = Brushes.Black, // Set the color of the border

                Height = DrawingCanvas.ActualHeight, // Use ActualHeight to get the current size
                Width = DrawingCanvas.ActualWidth // Use ActualWidth to get the current size
            };
            Canvas.SetLeft(border, 0);
            Canvas.SetTop(border, 0);
            Panel.SetZIndex(border, -1);

            DrawingCanvas.Children.Add(border);
            DrawingCanvas.Clip = new RectangleGeometry(new Rect(0, 0, DrawingCanvas.ActualWidth, DrawingCanvas.ActualHeight), 8, 8);
        }

        private void DrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
                X2 = this.DrawingCanvas.Width,
                Y2 = y,
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 2, 2 }
            };
            DrawingCanvas.Children.Add(priceLine);
            Entryprice.Text = Math.Round(InvMapToScale(y), 3).ToString();

        }



    }
}