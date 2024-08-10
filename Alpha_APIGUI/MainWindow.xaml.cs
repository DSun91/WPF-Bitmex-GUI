using BitmexGUI.Services.Implementations;
using BitmexGUI.Services.Interfaces;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
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

namespace Alpha_APIGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string ID = "";
        private string APIKEY = "";
        private string BimexEndpointRest = "https://www.bitmex.com/api/v1/instrument?symbol=XBTUSDT&timeframe=nearest&count=1&reverse=false";
        private string BimexEndpointWss = "wss://ws.bitmex.com/realtime?subscribe=instrument:XBTUSDT";
        private string BinanceEndpointRest = "https://www.bitmex.com/api/v1/instrument?symbol=XBTUSDT&timeframe=nearest&count=1&reverse=false";
        private string BinanceEndpointWss = "wss://fstream.binance.com/ws/btcusdt@kline_1m";


        public MainWindow()
        {
            


            InitializeComponent();


            IPrice BitmexAPI = new BitmexAPIPrice(ID, APIKEY, BimexEndpointRest, BimexEndpointWss);
            IPrice BinanceAPI = new BinanceAPIPrice(ID, APIKEY, BimexEndpointRest, BinanceEndpointWss);

            BinanceAPI.GetPriceWSS();
        }


    }

 


}