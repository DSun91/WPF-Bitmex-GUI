using BitmexGUI.ViewModels;
using BitmexGUI.ViewModels.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BitmexGUI.Views.Indicators
{
    /// <summary>
    /// Interaction logic for IndicatorsPannel.xaml
    /// </summary>
    public partial class IndicatorsPannel : UserControl
    {
        MainViewModel mainVM;


        public IndicatorsPannel()
        {
            InitializeComponent();
            Loaded += LoadHooks;
        }



        private void LoadHooks(object sender, RoutedEventArgs e)
        {

            mainVM = DataContext as MainViewModel;

            mainVM.indicatorsViewModelManager.SendRequestClearIndicatorParamsForm += () =>
            {
                IndicatorVMContainer.Content = null; 
                SelectedIndicatorCombobox.SelectedIndex = -1;
                DeselectedIndicator.SelectedIndex = -1;
            };



        }


    }
}
