using BitmexGUI.Services.Abstract;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
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
        public BitmexAPIPrice(string ID, string key, string urlRest, string urlWss) : base( ID,  key,  urlRest,  urlWss)
        {
            _ApiID = ID;
            _ApiKey = key;
            _UrlRest = urlRest;
            _UrlWss = urlWss;
        }
        

    }

    public class BinanceAPIPrice : APIPrice
    {
        private readonly string _ApiID;
        private readonly string _ApiKey;
        private readonly string _UrlRest;
        private readonly string _UrlWss;
        public BinanceAPIPrice(string ID, string key, string urlRest, string urlWss) : base(ID, key, urlRest, urlWss)
        {
            _ApiID = ID;
            _ApiKey = key;
            _UrlRest = urlRest;
            _UrlWss = urlWss;
        }

        override public void ProcessResponse(string response)
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

            MessageBox.Show($"Close price: {KandleStick["c"]}");

            // Now you can access the elements like this:
            //Console.WriteLine($"Event type: {data["e"]}");
            //Console.WriteLine($"Event time: {data["E"]}");
            //Console.WriteLine($"Symbol: {data["s"]}");

            // Accessing nested "k" data

            //Console.WriteLine($"Open price: {kDict["o"]}");
            //Console.WriteLine($"Close price: {kDict["c"]}");
            //Console.WriteLine($"High price: {kDict["h"]}");
            //Console.WriteLine($"Low price: {kDict["l"]}");
        }

    }



}
