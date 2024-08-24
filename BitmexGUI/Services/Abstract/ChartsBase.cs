using BitmexGUI.ViewModels;

namespace BitmexGUI.Services.Abstract
{
    public abstract class AbstractCharts : BitmexGUI.Services.Interfaces.IChart
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
