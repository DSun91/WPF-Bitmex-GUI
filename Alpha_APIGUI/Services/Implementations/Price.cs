using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BitmexGUI.Services.Implementations
{
    internal class BitmexAPIPrice: BitmexGUI.Services.Interfaces.IPrice
    {
        private readonly string _ApiID;
        private readonly string _ApiKey;
        public BitmexAPIPrice(string ID,string key)
        {
            _ApiID = ID;
            _ApiKey = key;

        }
        public async void GetPriceREST()
        {
            System.Net.Http.HttpClient BitmexHttpClient = new System.Net.Http.HttpClient();

            //BitmexHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);


            HttpResponseMessage response = await BitmexHttpClient.GetAsync("https://www.bitmex.com/api/v1/instrument?symbol=XBTUSDT&timeframe=nearest&count=1&reverse=false");


            MessageBox.Show(response.StatusCode.ToString());


            string content = await response.Content.ReadAsStringAsync();

            MessageBox.Show(content);
        }

        public async void GetPriceWSS()
        {
            System.Net.WebSockets.ClientWebSocket BitmexHttpClientWSS = new System.Net.WebSockets.ClientWebSocket();

            //BitmexHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            await BitmexHttpClientWSS.ConnectAsync(new Uri("wss://ws.bitmex.com/realtime?subscribe=instrument:XBTUSDT"), token);


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
                    MessageBox.Show(Encoding.ASCII.GetString(buffer, 0, result.Count));
                }
            }


        }
    }
}
