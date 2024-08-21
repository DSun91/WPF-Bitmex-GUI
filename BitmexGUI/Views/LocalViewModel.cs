using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Views
{
    public class GridData
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }

        public double X2 { get; set; }
        public double Y2 { get; set; }
    }
    internal class LocalViewModel:INotifyPropertyChanged
    {

        private ObservableCollection<GridData> _gridData;
        public ObservableCollection<GridData> GridData
        {
            get => _gridData;
            set
            {
                _gridData = value;
                OnPropertyChanged(nameof(GridData));
            }
        }

        public LocalViewModel()
        {
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            int GridSpacing = 50;
            double canvasHeight = 700; // Example height
            double canvasWidth = 700; // Example width
            GridData = new ObservableCollection<GridData>();

            // Vertical lines
            for (double x = -canvasWidth; x <= canvasWidth; x += GridSpacing)
            {
                GridData.Add(new GridData { X1 = x, Y1 = -canvasWidth, X2 = x, Y2 = canvasWidth });
            }

            // Horizontal lines
            for (double y = -canvasHeight; y <= canvasHeight; y += GridSpacing)
            {
                GridData.Add(new GridData { X1 = -canvasHeight, Y1 = y, X2 = canvasHeight, Y2 = y });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}



        
 
        //    private void MyLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //    {
        //        if (sender is Label label)
        //        {
        //            isDragging = true;
        //            clickPosition = e.GetPosition(label);
        //            label.CaptureMouse(); // Capture the mouse to receive mouse events even when the cursor is outside the label
        //        }
        //    }
        //    private void MyLabel_MouseMove(object sender, MouseEventArgs e)
        //    {
        //        if (isDragging && sender is Label label)
        //        {
        //            var mousePos = e.GetPosition(DrawingCanvas);


        //            double top = mousePos.Y - clickPosition.Y;

        //            if (top < 0) top = 0;


        //            if (top + label.ActualHeight > DrawingCanvas.ActualHeight)
        //                top = DrawingCanvas.ActualHeight - label.ActualHeight;

        //            var existingLine = ViewModel.OrderLines.FirstOrDefault(p => p.OrderID.Equals(label.Tag));

        //            if (existingLine != null)
        //            {
        //                existingLine.Price = (decimal)InvMapToScale(top);
        //                OrderLinesUpdated?.Invoke(existingLine);


        //            }


        //            //foreach (Line labelelem in DrawingCanvas.Children.OfType<Line>())
        //            //{
        //            //    if (labelelem.Name.ToLower().Contains("Orderline".ToLower()))
        //            //    {

        //            //        double y = Canvas.GetTop(labelelem);
        //            //        Canvas.SetTop(labelelem, top);
        //            //        //MessageBox.Show(labelelem.Name.ToString()+" "+ top);
        //            //        //MessageBox.Show(x.ToString() + " " + y.ToString());
        //            //    }


        //            //}

        //            Canvas.SetTop(label, top);


        //        }
        //    }
 