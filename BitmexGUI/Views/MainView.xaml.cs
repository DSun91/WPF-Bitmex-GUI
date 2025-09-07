using BitmexGUI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace BitmexGUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        //private MainViewModel ViewModel => (MainViewModel)DataContext;
        private MainViewModel viewModel;
        public event Action<string> OrderLinesUpdated; 
        
        

        public MainWindow()
        { 
           
            InitializeComponent();
             
            this.Loaded += MainWindow_Loaded;

            

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = new MainViewModel();
            DataContext = viewModel;
            SymbolTimeFrameSelectionView.DataContext = viewModel.symbolSelectionViewModel;
            EntrySetup.ParentViewModel = viewModel;
            EntrySetup.DataContext = viewModel.entryViewModel;
            LiveChart.DataContext = viewModel;
            OrdersTable.DataContext = viewModel;
            



        }


        //private bool _isDragging;
        //private Point _clickPosition;

        //private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    _isDragging = true;
        //    _clickPosition = e.GetPosition(SymbolTimeFrameSelectionView);
        //    SymbolTimeFrameSelectionView.CaptureMouse();
        //}

        //private void Control_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (_isDragging)
        //    {
        //        var canvasPos = e.GetPosition(MainCanvas);
        //        double left = canvasPos.X - _clickPosition.X;
        //        double top = canvasPos.Y - _clickPosition.Y;

        //        Canvas.SetLeft(SymbolTimeFrameSelectionView, left);
        //        Canvas.SetTop(SymbolTimeFrameSelectionView, top);
        //    }
        //}

        //private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    _isDragging = false;
        //    SymbolTimeFrameSelectionView.ReleaseMouseCapture();
        //}





        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                // Clamp to max size
                double targetWidth = Math.Min(MaxWidth, SystemParameters.WorkArea.Width);
                double targetHeight = Math.Min(MaxHeight, SystemParameters.WorkArea.Height);

                // Reset to normal, then resize+center
                WindowState = WindowState.Normal;
                Width = targetWidth;
                Height = targetHeight;
                Left = (SystemParameters.WorkArea.Width - targetWidth) / 2 + SystemParameters.WorkArea.Left;
                Top = (SystemParameters.WorkArea.Height - targetHeight) / 2 + SystemParameters.WorkArea.Top;
            }
        }
       
        private void Chart_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var Sender=sender as LivePriceChart;
            Sender.DrawingCanvas_MouseWheelEvents(sender,e);

            e.Handled = true;  
        }
    }
    class VisualHost : FrameworkElement
    {
        public DrawingVisual Visual { get; set; }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Visual != null)
            {
                drawingContext.DrawDrawing(Visual.Drawing);
            }
        }
    }
}
