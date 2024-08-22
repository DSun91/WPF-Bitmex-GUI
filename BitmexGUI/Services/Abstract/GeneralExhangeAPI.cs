using BitmexGUI.Models;
using BitmexGUI.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BitmexGUI.Services.Abstract
{
    public abstract class GeneralExchangeAPI : IPrice, IAccount
    {
        private readonly string ApiID;
        private readonly string ApiKey;
        protected string  UrlRest;
        protected string  UrlWss;
        private string LogfilePath = ConfigurationManager.AppSettings["LogFile"];
        public ObservableCollection<CandlestickData> CachedPriceData = new ObservableCollection<CandlestickData>();


        //Constructor
        public GeneralExchangeAPI(string ID, string key, string urlRest, string urlWss)
        {
            ApiID = ID;
            ApiKey = key;
            UrlRest = urlRest;
            UrlWss = urlWss;

        }
        public async void GetPriceREST(ObservableCollection<CandlestickData> PriceData, Dictionary<string, CandlestickData> _priceDataDictionary)
        {
            System.Net.Http.HttpClient BitmexHttpClient = new System.Net.Http.HttpClient();

            //BitmexHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            

            HttpResponseMessage response = await BitmexHttpClient.GetAsync(UrlRest);


            //MessageBox.Show(response.StatusCode.ToString());


            string content = await response.Content.ReadAsStringAsync();

            ProcessResponseRest(content, PriceData, _priceDataDictionary);
            //MessageBox.Show(content);
             
        }

        public async void GetPriceREST() { }



        public async void GetPriceWSS()
        {
            System.Net.WebSockets.ClientWebSocket HttpClientPriceWSS = new System.Net.WebSockets.ClientWebSocket();


            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

           


            int size = 5000;
            var buffer = new byte[size];

            if(HttpClientPriceWSS.State != WebSocketState.Connecting &&  HttpClientPriceWSS.State != WebSocketState.Open)
            {
                await HttpClientPriceWSS.ConnectAsync(new Uri(UrlWss), token);
            }


            while (HttpClientPriceWSS.State == WebSocketState.Open)
            {
                var result = await HttpClientPriceWSS.ReceiveAsync(buffer, token);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await HttpClientPriceWSS.CloseAsync(WebSocketCloseStatus.NormalClosure, null, token);
                }
                else
                {
                    string resp = Encoding.ASCII.GetString(buffer, 0, result.Count);

                    //System.IO.File.AppendAllText(filePath, resp + "\n");

                    ProcessPriceResponseWss(resp);
                }
            }


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
        public virtual async void ProcessResponseRest(string response, ObservableCollection<CandlestickData> PriceData, Dictionary<string, CandlestickData> _priceDataDictionary)
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
