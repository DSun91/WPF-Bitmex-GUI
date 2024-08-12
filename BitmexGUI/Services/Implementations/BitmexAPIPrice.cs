using BitmexGUI.Models;
using BitmexGUI.Services.Abstract;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BitmexGUI.Services.Implementations
{
    public class BitmexAPIPrice : APIPrice
    {
        private readonly string _ApiID;
        private readonly string _ApiKey;
        private readonly string _UrlRest;
        private readonly string _UrlWss;

        public event Action<SettledPrice> SettledPriceUpdated;
       
        public BitmexAPIPrice(string ID, string key, string urlRest, string urlWss) : base(ID, key, urlRest, urlWss)
        {
            _ApiID = ID;
            _ApiKey = key;
            _UrlRest = urlRest;
            _UrlWss = urlWss;
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

    }
}
