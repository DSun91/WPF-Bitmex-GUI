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
    /// Interaction logic for SymbolSelectionView.xaml
    /// </summary>
    public partial class SymbolSelectionView : UserControl
    {
        //public MainViewModel ParentViewModel
        //{
        //    get { return (MainViewModel)GetValue(ParentViewModelProperty); }
        //    set { SetValue(ParentViewModelProperty, value); }
        //}

        //public static readonly DependencyProperty ParentViewModelProperty =
        //DependencyProperty.Register(nameof(ParentViewModel),
        //                            typeof(MainViewModel),
        //                            typeof(SymbolSelectionView),
        //                            new PropertyMetadata(null)); 
        public SymbolSelectionView()
        {
            InitializeComponent();  
        }
        
        
    }
}
