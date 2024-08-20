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
using Newtonsoft.Json.Linq;

namespace BitmexGUI.Services.Implementations
{
    internal class CandlestickChart : AbstractCharts
    {
        public static double minOriginal = 0;
        public static double maxOriginal = 70000;
         
        // Target range
        public static double minTarget = 0;
        public static double maxTarget = 500;

        public static int CachedCandles = 50;

        public static int CandlesToView = 15;

        //public event Action OrderLinesUpdated;
        public static double ScaleFactor =1.5;
        public static double CandlesInterspace = 6; // Increased for better visibility
        public static double candleWidth =5; // Increased for better visibility
        double xOffset = 20; 
        public int InView;
        
        public CandlestickChart()
        {
             
        }
      

        public static double MapToScale(double originalValue,string fromewhere="")
        {
            //return maxTarget - ((originalValue - minOriginal) / (maxOriginal - minOriginal)) * (maxTarget - minTarget);
            if(fromewhere.Length > 0)
            {
               
                MessageBox.Show(originalValue.ToString() + " " + (maxTarget - ((originalValue - minOriginal) * (maxTarget - minTarget)) / (maxOriginal - minOriginal) + minTarget).ToString());
            }
            
            return maxTarget - ((originalValue - minOriginal) * (maxTarget - minTarget)) / (maxOriginal - minOriginal) + minTarget;
        }

        public static double InvMapToScale(double Value, string fromewhere = "")
        {
            if (fromewhere.Length > 0)
            {

                MessageBox.Show(Value.ToString()+" "+(((maxTarget - Value) / (maxTarget - minTarget)) * (maxOriginal - minOriginal) + minOriginal).ToString());
            }
            return ((maxTarget - Value) / (maxTarget - minTarget)) * (maxOriginal - minOriginal) + minOriginal;
        }

        internal class HeikinAshiChart : AbstractCharts
        {
            public HeikinAshiChart(int n) { }
        }
    }
}
