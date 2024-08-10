using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Services.Abstract
{
    public abstract class APIPrice : BitmexGUI.Services.Interfaces.IPrice
    {
        private readonly string _ApiID;
        private readonly string _ApiKey;
        private readonly string _UrlRest;
        private readonly string _UrlWss;
        private string filePath = Path.Combine("../../../Logs", "logs.txt");

        //Constructor
        public APIPrice(string ID, string key, string urlRest, string urlWss)
        {
            _ApiID = ID;
            _ApiKey = key;
            _UrlRest = urlRest;
            _UrlWss = urlWss;

        }
        public async void GetPriceREST()
        {
            System.Net.Http.HttpClient BitmexHttpClient = new System.Net.Http.HttpClient();

            //BitmexHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);


            HttpResponseMessage response = await BitmexHttpClient.GetAsync(_UrlRest);


            //MessageBox.Show(response.StatusCode.ToString());


            string content = await response.Content.ReadAsStringAsync();

            //MessageBox.Show(content);
        }

        public async void GetPriceWSS()
        {
            System.Net.WebSockets.ClientWebSocket BitmexHttpClientWSS = new System.Net.WebSockets.ClientWebSocket();



            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            await BitmexHttpClientWSS.ConnectAsync(new Uri(_UrlWss), token);


            int size = 5000;
            var buffer = new byte[size];

            while (BitmexHttpClientWSS.State == WebSocketState.Open)
            {
                var result = await BitmexHttpClientWSS.ReceiveAsync(buffer, token);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await BitmexHttpClientWSS.CloseAsync(WebSocketCloseStatus.NormalClosure, null, token);
                }
                else
                {
                    string resp = Encoding.ASCII.GetString(buffer, 0, result.Count); 

                    System.IO.File.AppendAllText(filePath, resp + "\n");

                    ProcessResponse(resp);
                }
            }


        }

        public virtual void ProcessResponse(string response)
        {

        }
    }
}
