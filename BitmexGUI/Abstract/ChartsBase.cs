using BitmexGUI.Interfaces;
using BitmexGUI.ViewModels;

namespace BitmexGUI.Abstract
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
