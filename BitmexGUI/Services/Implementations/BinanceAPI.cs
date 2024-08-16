using BitmexGUI.Models;
using BitmexGUI.Services.Abstract;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BitmexGUI.Services.Implementations
{
    public class BinanceAPI : GeneralExchangeAPI
    {
        private readonly string ApiID;
        private readonly string ApiKey;


        public event Action<CandlestickData> PriceUpdated;
        public BinanceAPI(string ID, string key, string urlRest, string urlWss) : base(ID, key, urlRest, urlWss)
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
                if (property.Name != "k")
                {
                    data[property.Name] = property.Value.ToObject<object>();
                }
            }

            // Create a nested dictionary for the "k" object
            Dictionary<string, object> kData = new Dictionary<string, object>();

            // Populate the nested dictionary with "k" properties
            foreach (var property in jsonObject["k"].Children<JProperty>())
            {
                kData[property.Name] = property.Value.ToObject<object>();
            }

            // Add the nested dictionary to the main dictionary
            data["k"] = kData;

            var KandleStick = (Dictionary<string, object>)data["k"];
            //System.IO.File.AppendAllText(ConfigurationManager.AppSettings["LogFile"], $"{{ {KandleStick["o"]}f,{KandleStick["h"]}f,{KandleStick["l"]}f,{KandleStick["c"]}f }}" + "\n");


            var priceData = new CandlestickData
            {
                Symbol = data["s"].ToString(),
                Open = double.Parse(KandleStick["o"].ToString()),
                High = double.Parse(KandleStick["h"].ToString()),
                Low = double.Parse(KandleStick["l"].ToString()),
                Close = double.Parse(KandleStick["c"].ToString()),
                Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(KandleStick["t"].ToString())).DateTime
            };
             
            PriceUpdated?.Invoke(priceData);

        }

        public override async void ProcessResponseRest(string response, ObservableCollection<CandlestickData> PriceData, Dictionary<string, CandlestickData> _priceDataDictionary)
        {
            var klines = JArray.Parse(response);

            foreach (var kline in klines)
            {
                long openTime = kline[0].ToObject<long>();

                // Parse prices from the kline data
                double openPrice = double.Parse(kline[1].ToString());
                double highPrice = double.Parse(kline[2].ToString());
                double lowPrice = double.Parse(kline[3].ToString());
                double closePrice = double.Parse(kline[4].ToString());


                Dictionary<string, List<double>> existingData = new Dictionary<string, List<double>>();


                CandlestickData InitData = new CandlestickData();

                InitData.Timestamp = DateTimeOffset.FromUnixTimeSeconds(openTime / 1000).UtcDateTime;
                // Assuming PriceData has properties like Open, High, Low, Close
                InitData.Open = openPrice;
                InitData.High = highPrice;
                InitData.Low = lowPrice;
                InitData.Close = closePrice;
                // Store the prices in a list 

                // Add to the SortedDictionary
                if (!PriceData.Contains(InitData))
                {
                    InitData.Posx = 20 * PriceData.Count;
                    PriceData.Add(InitData);
                    CachedPriceData.Add(InitData);
                }
                if (!_priceDataDictionary.ContainsKey(InitData.Timestamp.ToString()))
                {
                    _priceDataDictionary.TryAdd(InitData.Timestamp.ToString(), InitData);
                }

            }

        }

        public void UpdateRestEndpoint(string newEndpoint)
        {
            // Update the endpoint URL and potentially restart any connections
            UrlRest = newEndpoint;


        }

        public void UpdateWssEndpoint(string newEndpoint)
        {
            // Update the endpoint URL and potentially restart any connections
            UrlWss = newEndpoint;
        }

    }
}
