using BitmexGUI.Models;
using BitmexGUI.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Services.Abstract
{
    public abstract class AbstractCharts: BitmexGUI.Services.Interfaces.IChart
    {
        private MainViewModel _ViewModel;


         
        public AbstractCharts()
        {
             
             
        }
       
        public void GetInitialData()
        {
            


            

        }


        public virtual void DrawCharts()
        {

        }

    }
}
