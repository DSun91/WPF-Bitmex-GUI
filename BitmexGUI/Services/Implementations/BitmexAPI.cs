using BitmexGUI.Models;
using BitmexGUI.Services.Abstract;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BitmexGUI.Services.Implementations
{
    public class BitmexAPI : GeneralExchangeAPI
    {
        private readonly string ApiID;
        private readonly string ApiKey;
        private readonly string UrlRest;
        private readonly string UrlWss;
        private string BaseUrl = ConfigurationManager.AppSettings["BaseBitmexUrl"];
        private int expires = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 1800);
        public event Action<SettledPrice> SettledPriceUpdated;
        public event Action<Account> AccountInfo;
        public event Action<Position> PositionUpdated;
        public BitmexAPI(string ID, string key, string urlRest, string urlWss) : base(ID, key, urlRest, urlWss)
        {
            ApiID = ConfigurationManager.AppSettings["ID"];
            ApiKey = ConfigurationManager.AppSettings["SecretKey"];
            UrlRest = urlRest;
            UrlWss = urlWss;
        }


        private string GenerateSignature(string secret, string verb, string path, int expires, string data)
        {


            if (string.IsNullOrEmpty(data))
            {
                data = string.Empty;
            }

            string message = $"{verb}{path}{expires}{data}";


            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private async Task SendMessageAsync(string message, ClientWebSocket BitmexHttpClientWSS)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await BitmexHttpClientWSS.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }



        public async void GetPositionWSS()
        {
            string BITMEX_URL = "wss://testnet.bitmex.com/realtime";


            string verb = "GET";
            string path = "/realtime";
            string signature = GenerateSignature(ApiKey, verb, path, expires, "");

            ClientWebSocket BitmexHttpClientWSS = new System.Net.WebSockets.ClientWebSocket();



            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            await BitmexHttpClientWSS.ConnectAsync(new Uri(BITMEX_URL), token);
            if (BitmexHttpClientWSS.State == WebSocketState.Open)
            {
                var auth_message = new
                {
                    op = "authKeyExpires",
                    args = new object[] { ApiID, expires, signature }
                };

                string dataJson = JsonConvert.SerializeObject(auth_message);
                await SendMessageAsync(dataJson, BitmexHttpClientWSS);


                var subscribe_message = new
                {
                    op = "subscribe",
                    args = new object[] { "position" }
                };

                string dataJsonSub = JsonConvert.SerializeObject(subscribe_message);
                await SendMessageAsync(dataJsonSub, BitmexHttpClientWSS);
            }

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
                    //MessageBox.Show(resp);
                    System.IO.File.AppendAllText(ConfigurationManager.AppSettings["LogFile"], resp + "\n");

                    ProcessResponsePosition(resp);
                }
            }



        }

        override public void ProcessResponseWss(string response)
        {
            //MessageBox.Show(response);
            JObject jsonObject = JObject.Parse(response);

            // Create a dictionary to store the parsed data
            Dictionary<string, object> data = new Dictionary<string, object>();

            // Populate the dictionary with top-level properties
            foreach (var property in jsonObject.Properties())
            {
                if (property.Name == "data")
                {
                    //MessageBox.Show(jsonObject["data"].ToString());

                    if (jsonObject["data"].ToString().Contains("indicativeSettlePrice"))
                    {
                        //MessageBox.Show(jsonObject["data"].ToString());
                        JObject SettlePrice = JObject.Parse(jsonObject["data"][0].ToString());

                        //MessageBox.Show(SettlePrice["symbol"].ToString()+" "+SettlePrice["indicativeSettlePrice"].ToString());

                        var SetpriceData = new SettledPrice
                        {
                            Symbol = SettlePrice["symbol"].ToString(),
                            SettledPriceValue = double.Parse(SettlePrice["indicativeSettlePrice"].ToString()),
                            Timestamp = SettlePrice["timestamp"].ToString()
                        };
                        SettledPriceUpdated?.Invoke(SetpriceData);
                    }

                }
            }



        }



        private void ProcessResponsePosition(string response)
        {

            List<string> positionFields = new List<string>();

            JObject jsonObject = JObject.Parse(response);

            // Define the fields you expect to handle.
            
            Type positionType = typeof(Position);

            // Retrieve all properties of the Position class
            PropertyInfo[] properties = positionType.GetProperties();
             
           

            foreach (var property in jsonObject.Properties())
            {
                if (property.Name == "data")
                {
                    foreach (var data in jsonObject["data"])
                    {
                        JObject positionData = JObject.Parse(data.ToString());

                        foreach (var prop in positionData.Properties())
                        {
                            positionFields.Add(prop.Name);
                        }
                        // Initialize a Position object
                        var position = new Position();
                        
                        // Process only the fields that are present in the current message
                        foreach (string field in positionFields)
                        {
                            
                            if (positionData.ContainsKey(field))
                            {
                                
                                switch (field)
                                {
                                    
                                    case "account":
                                        if (!string.IsNullOrEmpty(positionData["account"].ToString()))
                                            position.AccountID = int.Parse(positionData["account"].ToString());
                                        break;
                                    case "symbol":
                                        if (!string.IsNullOrEmpty(positionData["symbol"].ToString()))
                                            position.Symbol = positionData["symbol"].ToString();
                                        break;
                                    case "avgEntryPrice":
                                        if (!string.IsNullOrEmpty(positionData["avgEntryPrice"].ToString()))
                                            position.AvgEntryPrice = float.Parse(positionData["avgEntryPrice"].ToString());
                                        break;
                                    case "markPrice":
                                        if (!string.IsNullOrEmpty(positionData["markPrice"].ToString()))
                                            position.MarkPrice = float.Parse(positionData["markPrice"].ToString());
                                        break;
                                    case "breakEvenPrice":
                                        if (!string.IsNullOrEmpty(positionData["breakEvenPrice"].ToString()))
                                            position.BreakEvenPrice = float.Parse(positionData["breakEvenPrice"].ToString());
                                        break;
                                    case "liquidationPrice":
                                        if (!string.IsNullOrEmpty(positionData["liquidationPrice"].ToString()))
                                            position.LiquidationPrice = float.Parse(positionData["liquidationPrice"].ToString());
                                        break;
                                    case "realisedPnl":
                                        if (!string.IsNullOrEmpty(positionData["realisedPnl"].ToString()))
                                            position.RealisedPnl = int.Parse(positionData["realisedPnl"].ToString());
                                        break;
                                    case "posLoss": 
                                        if (!string.IsNullOrEmpty(positionData["posLoss"].ToString()))
                                            position.PosLoss = int.Parse(positionData["posLoss"].ToString());
                                        break;
                                    case "unrealisedPnl":
                                        if (!string.IsNullOrEmpty(positionData["unrealisedPnl"].ToString()))
                                            position.UnrealisedPnl = int.Parse(positionData["unrealisedPnl"].ToString());
                                        break;
                                    case "commission":
                                        if (!string.IsNullOrEmpty(positionData["commission"].ToString()))
                                            position.Commission = float.Parse(positionData["commission"].ToString());
                                        break;
                                    case "leverage":
                                        if (!string.IsNullOrEmpty(positionData["leverage"].ToString()))
                                            position.Leverage = float.Parse(positionData["leverage"].ToString());
                                        break;
                                    case "currentQty":
                                        if (!string.IsNullOrEmpty(positionData["currentQty"].ToString()))
                                            position.CurrentQty = int.Parse(positionData["currentQty"].ToString());
                                        break;
                                    case "currentCost":
                                        if (!string.IsNullOrEmpty(positionData["currentCost"].ToString()))
                                            position.CurrentCost = int.Parse(positionData["currentCost"].ToString());
                                        break; 
                                    case "realisedCost":
                                        if (!string.IsNullOrEmpty(positionData["realisedCost"].ToString()))
                                            position.RealisedCost = int.Parse(positionData["realisedCost"].ToString());
                                        break;
                                   case "posComm":
                                        if (!string.IsNullOrEmpty(positionData["posComm"].ToString()))
                                            position.PosComm = int.Parse(positionData["posComm"].ToString());
                                        break;
                                    case "isOpen":
                                        if (!string.IsNullOrEmpty(positionData["isOpen"].ToString()))
                                            position.IsOpen = bool.Parse(positionData["isOpen"].ToString());
                                        break;


                                }
                            }
                        }

                        // Check if the Position object has the required fields before invoking the event
                        if (position.AccountID != 0 && !string.IsNullOrEmpty(position.Symbol))
                        {
                            PositionUpdated?.Invoke(position);
                        }
                    }
                }
            }

        }

    
        public void GetWallet()
        {
            string verb = "GET";
            string path = "/api/v1/user/wallet?currency=all";
            int expires = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 1800); // 30 minutes from now

            string signature = GenerateSignature(ApiKey, verb, path, expires, "");

            string URL = BaseUrl + path;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("api-expires", expires.ToString());
                client.DefaultRequestHeaders.Add("api-key", ApiID);
                client.DefaultRequestHeaders.Add("api-signature", signature);

                HttpResponseMessage response = client.GetAsync(URL).Result;
                string responseBody = response.Content.ReadAsStringAsync().Result;

                //MessageBox.Show(responseBody);
                List<Dictionary<string, object>> data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseBody);


                var accounts = new List<Account>();

                foreach (var x in data)
                {

                    if (x["currency"].ToString() != null)
                    {

                        var Account = new Account
                        {
                            Balance = Math.Round(double.Parse(x["amount"].ToString()) / 1000000, 2),
                            CurrencyName = x["currency"].ToString().ToUpper()
                        };

                        accounts.Add(Account);

                    } 
                }
                foreach (var account in accounts)
                {
                    AccountInfo?.Invoke(account);
                }
            }
        }

        private async void SetLeverage(string Symbol,double leverage)
        {
            HttpClient client = new HttpClient();
            string FunctionUrl = "/api/v1/position/leverage";
            string url ="https:"+ BaseUrl.Split(":")[1] + FunctionUrl;
            string Verb = "POST";

            var data = new
            {
                symbol = Symbol,
                leverage =Math.Round(leverage,0)
            };
            string dataJson = JsonConvert.SerializeObject(data);

            string signature = GenerateSignature(ApiKey, Verb, FunctionUrl, expires, dataJson);

            var headers = new HttpRequestMessage(HttpMethod.Post, url);
            headers.Headers.Add("api-expires", expires.ToString());
            headers.Headers.Add("api-key", ApiID);
            headers.Headers.Add("api-signature", signature);

            headers.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.SendAsync(headers);
            string responseString = await response.Content.ReadAsStringAsync();

            //MessageBox.Show(responseString);
             
        }

        public void GetLeverage(string Symbol)
        {

        }

        public async void CreateOrder(string Symbol,double Qty,double Price,string Type,string TimeInForce,string side,double leverage=0)
        {
            HttpClient client = new HttpClient();
            string FunctionUrl = "/api/v1/order";
            string url = "https:" + BaseUrl.Split(":")[1] + FunctionUrl;
            string Verb = "POST";



            if(leverage > 0) 
            {
                SetLeverage(Symbol, leverage);
            }

            var data = new  {
                                symbol= Symbol,
                                orderQty = Qty,
                                price = Price,
                                ordType = Type,
                                timeInForce = TimeInForce,
                                side = side
                             };


            string dataJson = JsonConvert.SerializeObject(data);

            string signature = GenerateSignature(ApiKey, Verb, FunctionUrl, expires, dataJson);

            var headers = new HttpRequestMessage(HttpMethod.Post, url);
            headers.Headers.Add("api-expires", expires.ToString());
            headers.Headers.Add("api-key", ApiID);
            headers.Headers.Add("api-signature", signature);

            headers.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.SendAsync(headers);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    MessageBox.Show("Order Created Successfully!");
                }
                else
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(responseString);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

         

    }
}
