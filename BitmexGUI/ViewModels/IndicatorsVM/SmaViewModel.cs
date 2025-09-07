using BitmexGUI.BaseClasses;
using BitmexGUI.Interfaces;
using BitmexGUI.Models;
using BitmexGUI.Models.IndicatorsModel;
using BitmexGUI.ViewModels.Utilities;
using BitmexGUI.Views.Indicators;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace BitmexGUI.ViewModels.Indicators
{


    public class SmaViewModel : IndicatorViewModelBase, INotifyPropertyChanged
    {
        
        PriceStreamViewModel _priceStreamViewModel { get; set; }


        //UI bound Series live charting refreshing ned an observable collection as polyline takes whole serie
        private ObservableCollection<PointSerie> _smaSerie = new ObservableCollection<PointSerie>();
        public ObservableCollection<PointSerie> SmaSerie
        {
            get
            { 
                return _smaSerie;
            }
            set 
            {   _smaSerie = value; 
                OnPropertyChanged(nameof(SmaSerie)); 
            }
        }


        private Color _color;
        public Color Color
        {
            get => _color;
            set { _color = value; OnPropertyChanged(); }
        }

        
        public SmaViewModel(PriceStreamViewModel priceStreamViewModel)//ObservableCollection<string> selectedIndicators
        {
            _priceStreamViewModel = priceStreamViewModel; 

            _priceStreamViewModel.ScaledPriceUpdated += () => SMA();
        }


        public bool drawUI { get; set; } = false; // set at the moment of clicking Add the indicator to the view
        public void SMA()
        {
            if(Period!= 0 && drawUI)
            {
                Name = $"SMA-{base.Period}";

                if (Color!=null)
                {
                    SmaSerie.Clear();

                    List<xyPoint> unscaledPriceIndicator = CalcUnscaledSMAOnWholePriceData(Period);

                    PointCollection scaledPriceIndicator = ScaleIndicator(Period, unscaledPriceIndicator);

                    var series = new PointSerie($"SMA-{Period}", scaledPriceIndicator, new SolidColorBrush(Color));

                    SmaSerie.Add(series);
                }
            }
                 
        }

        private List<xyPoint> CalcUnscaledSMAOnWholePriceData(int period)
        {
            List<xyPoint> unscaledPriceIndicator = new List<xyPoint>();

            //i=Period because we need at least 'Period' number of data points to calculate the first SMA value
            for (int i = period; i < _priceStreamViewModel.PriceData.Count; i++)
            {
                double sum = 0;

                for (int j = i - period; j < i; j++)
                {
                    sum += _priceStreamViewModel.PriceData[j].Close;
                }

                unscaledPriceIndicator.Add(new xyPoint
                {
                    x = _priceStreamViewModel.PriceData[i].Posx,
                    y = sum / period
                });

            }

            return unscaledPriceIndicator;
        }

        private PointCollection ScaleIndicator(int period, List<xyPoint> unscaledPriceIndicator)
        {
            int CandlesToSkip = OHLCandlestickChart.TotalCandlesCount - OHLCandlestickChart.CandlesToView;

            PointCollection scaledPriceIndicator = new PointCollection();

            for (int i = CandlesToSkip - period; i < unscaledPriceIndicator.Count; i++)
            {
                if (i < 0)
                {
                    i = 0;
                }
                unscaledPriceIndicator[i].x = (i - CandlesToSkip + period) * OHLCandlestickChart.CandlesInterspace;
                scaledPriceIndicator.Add(new Point(unscaledPriceIndicator[i].x, ScalePrice(unscaledPriceIndicator[i].y)));
            }
            return scaledPriceIndicator;
        }


        private double ScalePrice(double price)
        {
            var allValues = _priceStreamViewModel.PriceData.Skip(OHLCandlestickChart.TotalCandlesCount - OHLCandlestickChart.CandlesToView).Take(OHLCandlestickChart.TotalCandlesCount).SelectMany(data => new[] { data.Open, data.High, data.Low, data.Close });
            var minVal = allValues.Min();
            var maxVal = allValues.Max();


            OHLCandlestickChart.minOriginal = minVal;
            OHLCandlestickChart.maxOriginal = maxVal;
            double padding = (OHLCandlestickChart.maxOriginal - OHLCandlestickChart.minOriginal) * OHLCandlestickChart.ScaleFactor;
            OHLCandlestickChart.minOriginal -= padding;
            OHLCandlestickChart.maxOriginal += padding;

            var retval = OHLCandlestickChart.MapToScale(price) + OHLCandlestickChart.VerticalOffset;
            return retval;
        }


        private ICommand _addSMA;
        private void SendRequestAddSMA(object param)
        {
            RaiseSendRequestAddtoIndicatorsCollection();// Invoke IndicatorsVM add function

        }
        public ICommand AddSMA
        {
            get
            {
                if (_addSMA == null)
                {
                    _addSMA = new RelayCommand(SendRequestAddSMA);
                }
                return _addSMA;
            }
        }

       

    }
}
