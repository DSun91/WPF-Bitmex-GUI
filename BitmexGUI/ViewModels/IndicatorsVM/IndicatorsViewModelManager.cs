using BitmexGUI.BaseClasses;
using BitmexGUI.Models;
using BitmexGUI.ViewModels.Utilities;
using BitmexGUI.Views.Indicators;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel; 
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BitmexGUI.ViewModels.Indicators
{
 
  
    public class IndicatorsViewModelManager : INotifyPropertyChanged
    {
        public List<string> IndicatorsItems{ get; set; } = new List<string> { "SMA", "RSI", "B-BANDS", "ICHIMOKU" };

        public ObservableCollection<IndicatorViewModelBase> IndicatorsInView { get; set; }
        private PriceStreamViewModel PriceStreamViewModel { get; set; }
        public IndicatorViewModelBase IndicatorPannelVM { get; set; }

        private IndicatorViewModelBase _indicatorToRemove { get; set; }
        public IndicatorViewModelBase IndicatorToRemove 
        { 
            get
            {
                return _indicatorToRemove;
            }
            set 
            {
                _indicatorToRemove=value;
                RemoveIndicator();
                OnPropertyChanged(nameof(IndicatorToRemove));
            } 
        }

        public SmaViewModel SMA { get; set; }

        public event Action SendRequestClearIndicatorParamsForm;

        #region addButtonFromPannelLogic
         
        private void AddIndicatorToUI()
        {

            IndicatorsInView.Add(IndicatorPannelVM);

            switch (_selectedIndicator)
            {
                case "SMA":
                    
                    foreach (var indicator in IndicatorsInView)
                    {
                        if (indicator is SmaViewModel sma)
                        {
                            sma.drawUI = true;
                        }
                    }
                    break;
                default:
                    break;
            }
            SendRequestClearIndicatorParamsForm?.Invoke();// questo per triggerare il defaultare del valore della combobox

        }
         
        #endregion

        private void RemoveIndicator()
        {
            IndicatorsInView?.Remove(IndicatorToRemove);
        }


        private string _selectedIndicator;
        public string SelectedIndicator
        {
            get { return _selectedIndicator; }
            set
            {
                if (_selectedIndicator == value)
                    return;
                _selectedIndicator = value;

                switch (_selectedIndicator)
                {
                    case "SMA":
                        IndicatorPannelVM = new SmaViewModel(PriceStreamViewModel); 
                        ((SmaViewModel)IndicatorPannelVM).SendRequestAddtoIndicatorsCollection +=  AddIndicatorToUI;  
                        OnPropertyChanged(nameof(IndicatorPannelVM));
                        break;
                    default:
                        break;
                }
                
                OnPropertyChanged(nameof(SelectedIndicator)); 
            }
        }
         
         
        public IndicatorsViewModelManager(PriceStreamViewModel priceStreamViewModel)
        {
            PriceStreamViewModel = priceStreamViewModel;
            IndicatorsInView= new ObservableCollection<IndicatorViewModelBase>();
        }
         

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
