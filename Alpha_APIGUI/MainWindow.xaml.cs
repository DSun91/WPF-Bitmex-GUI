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

        

        public MainWindow()
        {
            


            InitializeComponent();


            IPrice BitmexAPI = new BitmexAPIPrice(ID, APIKEY);
            
            BitmexAPI.GetPriceWSS();
        }


    }

 


}