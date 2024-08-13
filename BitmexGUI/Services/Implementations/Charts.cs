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

namespace BitmexGUI.Services.Implementations
{
    internal class CandlestickChart : AbstractCharts
    {
        private double minOriginal = 0;
        private double maxOriginal = 80000;

        // Target range
        private double minTarget = 0;
        private double maxTarget = 400;

        
        public double interspace = 8; // Increased for better visibility
        public double candleWidth = 7; // Increased for better visibility
        double xOffset = 20;
        private MainViewModel ViewModel;
        private Canvas DrawingCanvas;
        public int InView;
        private int TotalCached;
        public CandlestickChart(MainViewModel viewModel, Canvas drawingCanvas,int InviewN, int TotalN) 
        {
            InView = InviewN;
            TotalCached=TotalN;
            ViewModel = viewModel;
            DrawingCanvas = drawingCanvas; 
        }
        public void RefreshCanvas()
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
                
                int counter = 0;
                //MessageBox.Show(_ViewModel.SettledPriceData.Count.ToString());
                foreach (var candleData in ViewModel.PriceData)
                {
                    if(counter >= TotalCached - InView)
                    {
                        var candleValues = candleData;

                        var open = MapToScale(candleData.Open);
                        var high = MapToScale(candleData.High);
                        var low = MapToScale(candleData.Low);
                        var close = MapToScale(candleData.Close);
                        //this.CurrentPriceTextBlock.Text =Math.Round(candleValues[3],3).ToString();
                        //this.CurrentPriceTextBlock.Margin = new Thickness(400, close-40, 0, 0);

                        DrawCharts(i * interspace, open, high, low, close, candleWidth, InView);
                        //MessageBox.Show(candleData.Timestamp.ToString());
                        i++;
                    }
                    counter++;
                }
            }
        }
        public void DrawCharts(double centerX, double open, double high, double low, double close, double width,int InView)
        {

            int fontsize = 14;
            double mappedSettledPrice = InvMapToScale(MapToScale(ViewModel.SettledPriceData.Last().SettledPriceValue));

             //isCurrentCandle = (centerX == (ViewModel.PriceData.Count - 1) * interspace);

            bool isCurrentCandle = (centerX == (InView - 1) * interspace);

            Line SettPrice = new Line
            {
                X1 = centerX + xOffset-10,
                Y1 =MapToScale(ViewModel.SettledPriceData.Last().SettledPriceValue),
                X2 = centerX + xOffset+10,
                Y2 = MapToScale(ViewModel.SettledPriceData.Last().SettledPriceValue),
                Stroke = Brushes.DarkViolet,
                StrokeThickness = 2
            };

            Line ClosePrice = new Line
            {
                X1 = centerX + xOffset - 10,
                Y1 = close,
                X2 = centerX + xOffset + 10,
                Y2 = close,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            TextBlock SettPriceLabel = new TextBlock
            {
                
                Text =mappedSettledPrice.ToString("F2")+$" = {Math.Round(mappedSettledPrice - InvMapToScale(close),2)}", // Format the close value to 2 decimal places
                Foreground = Brushes.DarkViolet,
                Background = Brushes.Transparent,
                FontSize = fontsize,
                FontWeight = FontWeights.Bold,
                Padding = new Thickness(2) // Optional padding for better visibility
            };




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
                Text = InvMapToScale(close).ToString("F2")+ " -", // Format the close value to 2 decimal places
                Foreground = Brushes.Black,
                Background = Brushes.Transparent,
                FontSize = fontsize,
                FontWeight = FontWeights.Bold,
                Padding = new Thickness(2) // Optional padding for better visibility
            };
            if (isCurrentCandle)
            {
                Canvas.SetLeft(closeLabel, centerX + xOffset + width / 2 + 5); // Position horizontally to the right of the candlestick
                Canvas.SetTop(closeLabel, close ); // Position vertically at the 'close' value
                
                Canvas.SetLeft(SettPriceLabel, centerX + xOffset + 80); // Position horizontally to the right of the candlestick
                Canvas.SetTop(SettPriceLabel, close); // Position vertically at the 'close' value
                DrawingCanvas.Children.Add(SettPriceLabel);
                DrawingCanvas.Children.Add(closeLabel);
                DrawingCanvas.Children.Add(SettPrice);
                DrawingCanvas.Children.Add(ClosePrice);
            }

        }
        public double MapToScale(double originalValue)
        {
            //return maxTarget - ((originalValue - minOriginal) / (maxOriginal - minOriginal)) * (maxTarget - minTarget);
            return maxTarget - ((originalValue - minOriginal) * (maxTarget - minTarget)) / (maxOriginal - minOriginal) + minTarget;
        }

        public double InvMapToScale(double Value)
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
      
    }

    internal class HeikinAshiChart : AbstractCharts
    {
        public HeikinAshiChart(int n){ }
    }
}
