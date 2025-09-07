using BitmexGUI.BaseClasses;
using BitmexGUI.Interfaces;
using BitmexGUI.Models;
using BitmexGUI.ViewModels;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Windows;
using static BitmexGUI.BaseClasses.BitmexAPI;

namespace BitmexGUI.Abstract
{
    public abstract class GeneralExchangeAPI : IPriceFeed, IAccount, IAPI
    {
        private readonly string ApiID;
        private readonly string ApiKey;
        protected string UrlRest;
        protected string UrlWss;
        private string LogfilePath = ConfigurationManager.AppSettings["LogFile"];
        public ObservableCollection<CandlestickData> CachedPriceData = new ObservableCollection<CandlestickData>();

        public string ID { get; set; }


        //Constructor
        public GeneralExchangeAPI(string ID, string key, string urlRest, string urlWss)
        {
            ApiID = ID;
            ApiKey = key;
            UrlRest = urlRest;
            UrlWss = urlWss;

        }
        public void GetPriceREST(ObservableCollection<CandleStickViewModel> PriceData,  Dictionary<string, CandlestickData> _priceDataDictionary)
        {
            HttpClient BitmexHttpClient = new HttpClient();

            //BitmexHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);


            HttpResponseMessage response =  BitmexHttpClient.GetAsync(UrlRest).Result;

            //MessageBox.Show(response.StatusCode.ToString());


            string content =  response.Content.ReadAsStringAsync().Result;

            ProcessResponseRest(content, PriceData, _priceDataDictionary);
            //MessageBox.Show(content);

        }

        public async void GetPriceREST() { }



        public virtual async void GetPriceWSS(string WebsocketName)
        {
             
        }

        protected async IAsyncEnumerable<string> FetchWebsocketResponse(ClientWebSocket WebSocket)
        {
            int size = 8912;
            var buffer = new byte[size];
            do
            {
                WebSocketReceiveResult result;
                using var ms = new System.IO.MemoryStream();
                do
                {


                    result = await WebSocket.ReceiveAsync(buffer, CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close && !result.CloseStatus.ToString().ToLower().Contains("normalclos"))
                    {
                        MessageBox.Show(result.CloseStatus.ToString() + "  " + result.CloseStatusDescription);
                        await WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                    }
                    else
                    {

                        ms.Write(buffer, 0, result.Count);



                    }
                } while (!result.EndOfMessage && WebSocket.State == WebSocketState.Open);

                byte[] messageBytes = ms.ToArray();
                string resp = Encoding.ASCII.GetString(messageBytes, 0, messageBytes.Length);

                yield return resp;
            }
            while (WebSocket.State == WebSocketState.Open);
        }

        protected virtual async void ConnectWebSocketEndpoint(ClientWebSocket WebSocket,string WebsocketName)
        {
            if (WebSocket.State != WebSocketState.Connecting && WebSocket.State != WebSocketState.Open)
            {
                await WebSocket.ConnectAsync(new Uri(UrlWss), CancellationToken.None);
                WebSocketManager.Instance.AddWebSocket(WebsocketName, WebSocket);
            }
        }
        protected virtual async Task ConnectWebSocketEndpoint(ClientWebSocket WebSocket, IWssConnectionArgs Params)
        {
        }

        protected virtual void ProcessPriceResponseWss()
        {

        }
        protected virtual void ProcessPriceResponseWss(string response)
        {

        }

        public virtual void ProcessResponseRest()
        {

        }
        public virtual void ProcessResponseRest(string response)
        {

        }
        public virtual void ProcessResponseRest(string response, ObservableCollection<CandleStickViewModel> PriceData, Dictionary<string, CandlestickData> _priceDataDictionary)
        {


        }
        public virtual void GetWallet()
        {

        }

        public virtual async void SetLeverage(string Symbol, double leverage)
        {


        }

        public virtual void GetLeverage(string Symbol)
        {

        }
    }
}
