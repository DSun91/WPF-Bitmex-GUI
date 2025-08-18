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

        //private MainViewModel ViewModel => (MainViewModel)DataContext;
        private MainViewModel viewModel;
        public event Action<string> OrderLinesUpdated;
        public event Action<string> CancelOrder;
        private string AmendingOrderID;
        private Point clickPositionCanvas;
        private bool isDraggingCanvas = false;
        public static bool isDraggingOrderLine = false;
        private Point clickPositionLabel;
        

        public MainWindow()
        { 
           
            InitializeComponent();


            viewModel = new MainViewModel();
            DataContext = viewModel;
            SymbolTimeFrameSelectionView.DataContext = viewModel.symbolSelectionViewModel;
            EntrySetup.DataContext = viewModel.entryViewModel;
            LiveChart.DataContext = viewModel;

            this.Loaded += MainWindow_Loaded;

            

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
            viewModel.StartPriceFeed();

        }
        public async Task CloseAllConnectionsAsync()
        {
            await WebSocketManager.Instance.CloseAllWebSocketsAsync(CancellationToken.None);
        }

         


       

        private void CancleOpenOrder(object sender, RoutedEventArgs e)
        {
            Button Btn = sender as Button;

            if (Btn != null)
            {
                CancelOrder?.Invoke(Btn.Tag.ToString());
            }



        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                // Clamp to max size
                double targetWidth = Math.Min(MaxWidth, SystemParameters.WorkArea.Width);
                double targetHeight = Math.Min(MaxHeight, SystemParameters.WorkArea.Height);

                // Reset to normal, then resize+center
                WindowState = WindowState.Normal;
                Width = targetWidth;
                Height = targetHeight;
                Left = (SystemParameters.WorkArea.Width - targetWidth) / 2 + SystemParameters.WorkArea.Left;
                Top = (SystemParameters.WorkArea.Height - targetHeight) / 2 + SystemParameters.WorkArea.Top;
            }
        }
        private void LimitTPSLSell_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LivePriceChart_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void Chart_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var Sender=sender as LivePriceChart;
            Sender.DrawingCanvas_MouseWheelEvents(sender,e);

            e.Handled = true;  
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
