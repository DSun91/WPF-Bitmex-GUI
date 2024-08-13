using BitmexGUI.Models;
using BitmexGUI.Services.Implementations;
using BitmexGUI.Services.Interfaces;
using BitmexGUI.ViewModels;
using System.ComponentModel.Design;
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



        private int initialcandles = 250;
        private int Candle_inView = 50;
        private MainViewModel ViewModel => (MainViewModel)DataContext;
        private MainViewModel viewModel;
        // Target range
        private CandlestickChart CandleStickView;


        

        public MainWindow()
        {
            
             
            InitializeComponent();

            viewModel = new MainViewModel(initialcandles); 
            DataContext = viewModel;
            viewModel.StartPriceFeed();
            viewModel.PriceDataUpdated += OnPriceDataUpdated;
            viewModel.BalanceUpdated += OnBalanceInfoUpdated;

            CandleStickView = new CandlestickChart(ViewModel, DrawingCanvas,Candle_inView,initialcandles);

            DrawingCanvas.MouseLeftButtonDown += DrawingCanvas_MouseLeftButtonDown;
            DrawingCanvas.MouseWheel += DrawingCanvas_MouseWheel;
            this.Loaded += MainWindow_Loaded;
            
        }

        private void DrawingCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            
            if (e.Delta > 0)
            {
                CandleStickView.candleWidth += 1;
                CandleStickView.interspace += 1;
                CandleStickView.InView -=1;
            }

            // If the mouse wheel delta is negative, move the box down.
            if (e.Delta < 0)
            {
                
                CandleStickView.candleWidth -= 0.5;
                CandleStickView.interspace -= 0.5;
                CandleStickView.InView += 5;
            }

            if (CandleStickView.candleWidth<0.1)
            {
                CandleStickView.candleWidth = 0.1;
            }
            if (CandleStickView.interspace < 0.1)
            {
                CandleStickView.interspace = 0.1;
            }
             
            CandleStickView.RefreshCanvas();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        { 
            viewModel.GetBalance(CmbCurrency.SelectedValue.ToString().Split(": ")[1].Replace(" ",""));
            SetupElementsValues();
            
        }

        private void SetupElementsValues()
        {
            AmountSlider.Minimum = 0;
            AmountSlider.Maximum = viewModel.AccountInfos[0].Balance;
            


        }



        private void OnBalanceInfoUpdated()
        {
            
            CurrentBalance.Text = viewModel.AccountInfos[0].Balance.ToString();
        }

        private void OnPriceDataUpdated()
        {
           
            CandleStickView.RefreshCanvas();
            


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
                X2 = DrawingCanvas.Width,
                Y2 = y,
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 2, 2 }
            };
            DrawingCanvas.Children.Add(priceLine);
            Entryprice.Text = Math.Round(CandleStickView.InvMapToScale(y), 3).ToString();

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            

            // Check if viewModel is not null
            if (viewModel == null) return; // Early exit if viewModel is null
            if (slider != null)
            {
                int newInitialCandles = (int)Math.Round(slider.Value, 0);
                if (newInitialCandles != initialcandles)
                {
                    viewModel.UpdateInitialCandles(newInitialCandles);
                    
                    
                    // Optionally, you might want to refresh or reset the candlestick view as well
                    CandleStickView.RefreshCanvas();
                }
            }
        }

        private void AmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider sld = sender as Slider;
            EntryAmount.Text=Math.Round(sld.Value,2).ToString();
            BalancePercent.Content=Math.Round(sld.Value * 100 / viewModel.AccountInfos[0].Balance, 2).ToString()+" %";
        }

        
    }
}