using BitmexGUI.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.BaseClasses
{
    public class IndicatorViewModelBase: IIndicatorVM,INotifyPropertyChanged
    {
        public int Period { get; set; }
        public string Name { get; set; }

        public event Action SendRequestAddtoIndicatorsCollection;

        public void RaiseSendRequestAddtoIndicatorsCollection()
        {
            SendRequestAddtoIndicatorsCollection?.Invoke();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
