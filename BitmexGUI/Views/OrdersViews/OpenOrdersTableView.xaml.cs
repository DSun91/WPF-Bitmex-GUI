using BitmexGUI.Models;
using BitmexGUI.ViewModels;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ClosedPositionView.xaml
    /// </summary>
    public partial class OpenOrdersTableView : UserControl
    {
        public OpenOrdersTableView()
        {
            InitializeComponent();

            Loaded += OpenOrdersTableView_Loaded;

        }
        private void OpenOrdersTableView_Loaded(object sender, RoutedEventArgs e)
        {
            CreateGrid();
        }
        public void CreateGrid()
        {
            //Orders.Columns.Clear();

            //var props = typeof(Order).GetProperties();
            //foreach (var prop in props)
            //{
            //    if ((DataContext as MainViewModel).ordersViewModel.OrdersInfo.Any(o => prop.GetValue(o) != null))
            //    {
            //        Orders.Columns.Add(new DataGridTextColumn
            //        {
            //            Header = prop.Name,
            //            Binding = new Binding(prop.Name)
            //        });
            //    }
            //}
        }
    }
}
