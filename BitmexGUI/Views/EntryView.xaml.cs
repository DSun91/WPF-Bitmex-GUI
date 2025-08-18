using BitmexGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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

namespace BitmexGUI.Views
{
    /// <summary>
    /// Interaction logic for EntryView.xaml
    /// </summary>
    public partial class EntryView : UserControl
    {
        public EntryView()
        {
            InitializeComponent();
        }
        private void AmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {


            Slider sld = sender as Slider;
            EntryAmount.Text = Math.Round(sld.Value, 2).ToString("F2", CultureInfo.InvariantCulture);

            var vm= DataContext as EntryViewModel;

            var existingAccountBalance = double.Parse(vm.CurrentBalance);

            if (existingAccountBalance != null)
            {
                BalancePercent.Content = Math.Round(sld.Value * 100 / existingAccountBalance, 2).ToString() + " %";
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {


        }

        private void BitmexSettled_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            var val = vm.SettledPriceData.LastOrDefault().SettledPriceValue;
            Entryprice.Text = Math.Round(val, 3).ToString();


        }
    }
}
