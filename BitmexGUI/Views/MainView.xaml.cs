using BitmexGUI.Models;
using BitmexGUI.Services.Implementations;
using BitmexGUI.ViewModels;
using System.Globalization;
using System.Net.WebSockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


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
        private CandlestickChart CandleStickView;
        public event Action<string> OrderLinesUpdated;
        public event Action<string> CancelOrder;
        private string AmendingOrderID;
        private Point clickPositionCanvas;
        private bool isDraggingCanvas = false;
        public static bool isDraggingOrderLine = false;
        private Point clickPositionLabel;
        public static Dictionary<string, string> ExchangeTickersMap = new Dictionary<string, string>
        {
            { "BTCUSDT","XBTUSDT" },
            { "BTCUSD","XBTUSD" },
            { "ETHUSDT","ETHUSDT" }
        };

        public MainWindow()
        {


            InitializeComponent();
            LoadTickers();
            LoadTimeframes();
            LoadExchanges();
            ExchangeSelector.SelectedIndex = 0;
            Ticker.SelectedIndex = 0;
            Timeframe.SelectedIndex = 0;
            Ticker.SelectionChanged += Ticker_SelectionChanged;
            Timeframe.SelectionChanged += Timeframe_SelectionChanged;


            this.Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TryInitializeViewModel();

        }
        public async Task CloseAllConnectionsAsync()
        {
            await WebSocketManager.Instance.CloseAllWebSocketsAsync(CancellationToken.None);
        }
        //private async Task EnsureAllWebSocketsClosedAsync()
        //{
        //    // Assuming WebSocketManager.Instance.GetAllWebSockets() returns a list of WebSocket instances
        //    var webSockets = WebSocketManager.Instance.GetAllWebSockets();

        //    for (int i = 0; i < webSockets.Count; i++)
        //    {
        //        if (webSockets[i].State != WebSocketState.CloseReceived && webSockets[i].State != WebSocketState.Closed)
        //        {
        //            // Optionally, you can force close here
        //            await webSockets[i].CloseAsync(WebSocketCloseStatus.NormalClosure, "Ensuring closure", CancellationToken.None);
        //        }

        //    }

        //}


        private async void TryInitializeViewModel()
        {

            if (Timeframe.SelectedItem != null && Ticker.SelectedItem != null)
            {


                await WebSocketManager.Instance.CloseAllWebSocketsAsync(CancellationToken.None);


                DataContext = null;   // Unbind the DataContext



                viewModel = new MainViewModel(CandlestickChart.CachedCandles,
                                              Ticker.SelectedItem.ToString().Equals("BTCUSD") ? "BTCUSDT" : Ticker.SelectedItem.ToString(),
                                              Timeframe.SelectedItem.ToString(),
                                              ExchangeTickersMap[Ticker.SelectedItem.ToString()]);

                DataContext = viewModel;
                CandleStickView = new CandlestickChart();
                DrawingCanvas.MouseDown += DrawingCanvas_MouseMiddleButtonDown;
                DrawingCanvas.MouseUp += DrawingCanvas_MouseMiddleButtonUp;
                DrawingCanvas.MouseWheel += DrawingCanvas_MouseWheelEvents;
                DrawingCanvas.MouseLeftButtonDown += MoveCanvas_MouseLeftButtonDown;
                DrawingCanvas.MouseMove += MoveCanvas_MouseMove;
                DrawingCanvas.MouseLeftButtonUp += MoveCanvas_MouseLeftButtonUp;

                this.OrderLinesUpdated += (AmendingOrderID) =>
                {
                    if (DataContext is MainViewModel viewModel)
                    {
                        viewModel.HandleOrderLineUpdate(AmendingOrderID);
                    }
                };


                this.CancelOrder += (OrderCanceled) =>
                {
                    if (DataContext is MainViewModel viewModel)
                    {
                        viewModel.CancelOrder(OrderCanceled);
                    }
                };



                viewModel.StartPriceFeed();



            }
        }



        private void Ticker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TryInitializeViewModel();
        }

        private void Timeframe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TryInitializeViewModel();
        }

        private void LoadTickers()
        {
            Ticker.ItemsSource = new List<string> { "BTCUSDT", "BTCUSD", "ETHUSDT" };
        }

        private void LoadTimeframes()
        {
            Timeframe.ItemsSource = new List<string> { "1m", "5m", "15m", "1h", "4h", "1d", "1w" };
        }

        private void LoadExchanges()
        {
            ExchangeSelector.ItemsSource = new List<string> { "Bitmex" };
        }

        //private void test(object sender, RoutedEventArgs e)
        //{
        //    Button Btn = sender as Button;

        //    MessageBox.Show("test");


        //}

        /// //////////////////////////////////////////////  ORDER LINE DRAGGING

        private void OrderTag_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label label)
            {
                isDraggingCanvas = false;
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

            OrderLinesUpdated?.Invoke(AmendingOrderID);
        }

        private void OrderTag_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDraggingOrderLine && sender is Label label)
            {
                var mousePos = e.GetPosition(MainRenderingCanvas);


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

                        OrderID = existingLine.OrderID,
                        Price = (decimal)top,
                        Side = existingLine.Side,
                        Symbol = existingLine.Symbol
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

        private void MoveCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                isDraggingCanvas = true;
                isDraggingOrderLine = false;
                clickPositionCanvas = e.GetPosition(canvas);

                canvas.CaptureMouse(); // Capture the mouse to receive mouse events even when the cursor is outside the label

            }
        }

        private void MoveCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDraggingCanvas && !isDraggingOrderLine && sender is Canvas canvas)
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
                if (deltaX > 0 && CandlestickChart.CandlesToView < CandlestickChart.CachedCandles)
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

                if (CandlestickChart.CandlesToView > CandlestickChart.CachedCandles)
                {
                    CandlestickChart.CandlesToView = CandlestickChart.CachedCandles - 1;
                }
                // Optionally, refresh or redraw your data as needed

                viewModel.RefreshScaledPriceData();
                clickPositionCanvas = mousePos;
            }
        }

        private void MoveCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Canvas Canvas)
            {
                isDraggingCanvas = false;
                Canvas.ReleaseMouseCapture(); // Release the mouse capture when dragging is finished


            }

        }  //    foreach (OrderLine x in ViewModel.OrdersLines)


        /// //////////////////////////////////////////////  CANDLESTICK DRAGGING  



        /// ////////////////////////////////////////////// MOUSEWHEEL horizontal ZOOM
        private void DrawingCanvas_MouseWheelEvents(object sender, MouseWheelEventArgs e)
        {
            // Get the position of the mouse click relative to the Canvas
            if (e.Delta > 0)
            {

                //CandlestickChart.ScaleFactor -= 0.01;
                CandlestickChart.candleWidth += 0.2;
                CandlestickChart.CandlesInterspace += 0.2;

                viewModel.RefreshScaledPriceData();




            }
            if (e.Delta < 0)
            {

                //CandlestickChart.CandlesToView += 1;
                CandlestickChart.candleWidth -= 0.2;
                CandlestickChart.CandlesInterspace -= 0.2;
                //CandlestickChart.ScaleFactor += 0.01;
                if (CandlestickChart.candleWidth < 0.1)
                {
                    CandlestickChart.candleWidth = 0.1;
                    CandlestickChart.CandlesInterspace = 0.1;

                }
                viewModel.RefreshScaledPriceData();



            }
        }

        /// ////////////////////////////////////////////// MOUSEWHEEL horizontal ZOOM
        private VisualHost _lineVisualHost;

        private void DrawingCanvas_MouseMiddleButtonDown(object sender, MouseButtonEventArgs e)
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
                var PriceVal = Math.Round(CandlestickChart.InvMapToScale(y), 3).ToString();
                // Create a new DrawingVisual and render the line
                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen())
                {
                    Pen linePen = new Pen(Brushes.Blue, 1)
                    {
                        DashStyle = new DashStyle(new double[] { 1, 2 }, 0)
                    };
                    dc.DrawLine(linePen, new Point(0, y), new Point(MainRenderingCanvas.Width, y));
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
                Entryprice.Text = PriceVal;
            }
        }

        private void DrawingCanvas_MouseMiddleButtonUp(object sender, MouseButtonEventArgs e)
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
        private void AmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider sld = sender as Slider;
            EntryAmount.Text = Math.Round(sld.Value, 2).ToString();

            var existingAccountBalance = viewModel.AccountInfos.FirstOrDefault(p => p.CurrencyName == CmbCurrency.SelectedValue);
            if (existingAccountBalance != null)
            {
                BalancePercent.Content = Math.Round(sld.Value * 100 / existingAccountBalance.Balance, 2).ToString() + " %";
            }
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
            Button Btn = sender as Button;

            if (Btn != null)
            {
                CancelOrder?.Invoke(Btn.Tag.ToString());
            }



        }

        private void LimitTPSLSell_Click(object sender, RoutedEventArgs e)
        {

        }
    }
    class VisualHost : FrameworkElement
    {
        public DrawingVisual Visual { get; set; }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Visual != null)
            {
                drawingContext.DrawDrawing(Visual.Drawing);
            }
        }
    }
}
