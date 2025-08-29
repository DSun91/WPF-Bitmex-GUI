using BitmexGUI.Services.Interfaces;
using BitmexGUI.ViewModels;

namespace BitmexGUI.Services.Abstract
{
    public abstract class AbstractCharts : IChart
    {
        private MainViewModel _ViewModel;



        public AbstractCharts()
        {


        }


        public virtual void DrawCharts()
        {

        }

    }
}
