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

        public event Action<SettledPrice> SettledPriceUpdated;
        public event Action<Account> AccountInfo;
        public BitmexAPI(string ID, string key, string urlRest, string urlWss) : base(ID, key, urlRest, urlWss)
        {
            ApiID = ID;
            ApiKey = key;
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

            string signature = GenerateSignature(ConfigurationManager.AppSettings["SecretKey"], verb, path, expires, "");

            string URL = "https://www.bitmex.com" + path;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("api-expires", expires.ToString());
                client.DefaultRequestHeaders.Add("api-key", ConfigurationManager.AppSettings["ID"]);
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
                            Balance = double.Parse(x["amount"].ToString())/1000000
                        };
                        
                        AccountInfo?.Invoke(CurrentBalance);
                    
                    }
                      
                }
                
            }
        }



    }
}
