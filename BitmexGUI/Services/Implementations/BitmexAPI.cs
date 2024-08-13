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
        private string BaseUrl = "https://www.bitmex.com";
        private int  expires = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 1800);
        public event Action<SettledPrice> SettledPriceUpdated;
        public event Action<Account> AccountInfo;
        public BitmexAPI(string ID, string key, string urlRest, string urlWss) : base(ID, key, urlRest, urlWss)
        {
            ApiID = ConfigurationManager.AppSettings["ID"];
            ApiKey = ConfigurationManager.AppSettings["SecretKey"];
            UrlRest = urlRest;
            UrlWss = urlWss;
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

        public void GetBalance(string Symbol)
        {
            string verb = "GET";
            string path = "/api/v1/user/wallet?currency=all";
            int expires = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 1800); // 30 minutes from now

            string signature = GenerateSignature(ApiKey, verb, path, expires, "");

            string URL = "https://www.bitmex.com" + path;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("api-expires", expires.ToString());
                client.DefaultRequestHeaders.Add("api-key", ApiID);
                client.DefaultRequestHeaders.Add("api-signature", signature);

                HttpResponseMessage response = client.GetAsync(URL).Result;
                string responseBody = response.Content.ReadAsStringAsync().Result;
                 
                List<Dictionary<string, object>> data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseBody);

                foreach (var x in data)
                {
                    if (x["currency"].ToString().ToLower().Contains(Symbol.ToLower())) 
                    {
                        
                        var CurrentBalance = new Account
                        {
                            Balance = Math.Round(double.Parse(x["amount"].ToString())/1000000,2)
                        };
                        
                        AccountInfo?.Invoke(CurrentBalance);
                    
                    }
                      
                }
                
            }
        }

        private async void SetLeverage(string Symbol,double leverage)
        {
            HttpClient client = new HttpClient();
            string FunctionUrl = "/api/v1/position/leverage";
            string url = BaseUrl + FunctionUrl;
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
            string url = BaseUrl + FunctionUrl;
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
