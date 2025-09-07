using BitmexGUI.Abstract;
using System.Windows;

namespace BitmexGUI.BaseClasses
{
    public class OHLCandlestickChart : AbstractCharts
    {
        public static double minOriginal = 0;
        public static double maxOriginal = 7000;

        // Target range
        public static double minTarget = 0;
        public static double maxTarget = 400;
        public static int TotalCandlesCount = 500;
        public static int CandlesToView = 75;
        public static double ScaleFactor = 0.1;
        public static double CandlesInterspace = 6; // Increased for better visibility
        public static double candleWidth = 5; // Increased for better visibility
        public static double VerticalOffset = 0;
        public int InView;

        public OHLCandlestickChart()
        {

        }


        public static double MapToScale(double originalValue, string fromewhere = "")
        {
            //return maxTarget - ((originalValue - minOriginal) / (maxOriginal - minOriginal)) * (maxTarget - minTarget);
            if (fromewhere.Length > 0)
            {

                MessageBox.Show(originalValue.ToString() + " " + (maxTarget - (originalValue - minOriginal) * (maxTarget - minTarget) / (maxOriginal - minOriginal) + minTarget).ToString());
            }
            var value= maxTarget - (originalValue - minOriginal) * (maxTarget - minTarget) / (maxOriginal - minOriginal) + minTarget;
            return value;
        }

        public static double InvMapToScale(double Value, string fromewhere = "")
        {
            if (fromewhere.Length > 0)
            { 
                MessageBox.Show(Value.ToString() + " " + ((maxTarget - Value) / (maxTarget - minTarget) * (maxOriginal - minOriginal) + minOriginal).ToString());
            }
            return (maxTarget - Value) / (maxTarget - minTarget) * (maxOriginal - minOriginal) + minOriginal;
        }

        internal class HeikinAshiChart : AbstractCharts
        {
            public HeikinAshiChart(int n) { }
        }
    }
}
