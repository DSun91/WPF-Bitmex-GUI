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



        private int CachedCandleSize;  
        private int Candles_inView;
        private MainViewModel ViewModel => (MainViewModel)DataContext;
        private MainViewModel viewModel;
        // Target range
        private CandlestickChart CandleStickView;


        

        public MainWindow()
        {
            
             
            InitializeComponent();
            int.TryParse(ConfigurationManager.AppSettings["MaxCacheCandles"].ToString(), out CachedCandleSize);
            int.TryParse(ConfigurationManager.AppSettings["CandlesInView"],out Candles_inView);
            viewModel = new MainViewModel(CachedCandleSize); 
            DataContext = viewModel;
            viewModel.StartPriceFeed();
             
            viewModel.BalanceUpdated += OnBalanceInfoUpdated;
            viewModel.PriceDataUpdated += OnPriceDataUpdated;
            CandleStickView = new CandlestickChart(ViewModel, DrawingCanvas,Candles_inView, CachedCandleSize);

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
            
            viewModel.GetBalances(); 
            
            
        }

        private void SetupElementsValues()
        {
            var account = viewModel.AccountInfos.FirstOrDefault(x => x.CurrencyName.Equals(viewModel.SelectedCurrency, StringComparison.OrdinalIgnoreCase));
            AmountSlider.Minimum = 0;
            AmountSlider.Maximum = account?.Balance ?? 0.1 ;



        }

        //private ObservableCollection<string> Currencies=new ObservableCollection<string>();

        private void OnBalanceInfoUpdated()
        {

          
            //Currencies.Clear();

            //// Populate the list with new currency names
            //foreach (var accountInfo in viewModel.AccountInfos)
            //{
            //    if (!Currencies.Contains(accountInfo.CurrencyName))
            //    {
            //        Currencies.Add(accountInfo.CurrencyName);
            //    }
            //}

            //// Set the updated list as the ItemsSource for the ComboBox
            //CmbCurrency.ItemsSource = Currencies;

            //string currencyname = CmbCurrency.Items[0].ToString();



            //var account = viewModel.AccountInfos.FirstOrDefault(account => account.CurrencyName.Equals(currencyname, StringComparison.OrdinalIgnoreCase));

            //CurrentBalance.Text = account?.Balance.ToString() ?? "No balance available";  // default message if no account is found


        }

        private void OnPriceDataUpdated()
        {

            CandleStickView.RefreshCanvas();


        }

        //private void OnPriceDataAdded()
        //{

        //    //MessageBox.Show("new data added");


        //}

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
            Entryprice.Text = Math.Round(CandlestickChart.InvMapToScale(y), 3).ToString();

        }

        //private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    Slider slider = sender as Slider;


        //    // Check if viewModel is not null
        //    if (viewModel == null) return; // Early exit if viewModel is null
        //    if (slider != null)
        //    {
        //        int newInitialCandles = (int)Math.Round(slider.Value, 0);
        //        if (newInitialCandles != CachedCandleSize)
        //        {
        //            viewModel.UpdateInitialCandles(newInitialCandles);


        //            // Optionally, you might want to refresh or reset the candlestick view as well
        //            CandleStickView.RefreshCanvas();
        //        }
        //    }
        //}

        private void AmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider sld = sender as Slider;
            EntryAmount.Text=Math.Round(sld.Value,2).ToString();
            BalancePercent.Content=Math.Round(sld.Value * 100 / viewModel.AccountInfos[0].Balance, 2).ToString()+" %";
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            CandleStickView.InView = Candles_inView;
            CandleStickView.RefreshCanvas();
        }

        private void BitmexSettled_Click(object sender, RoutedEventArgs e)
        {
             
            Entryprice.Text = Math.Round(viewModel.SettledPriceData.Last().SettledPriceValue, 3).ToString();
          

        }
      
    }
    
}
