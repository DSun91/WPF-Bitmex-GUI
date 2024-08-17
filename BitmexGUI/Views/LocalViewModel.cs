using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmexGUI.Views
{
    
    internal class LocalViewModel
    {
         
        public ObservableCollection<double> HorizontalLines { get; set; }
        public ObservableCollection<double> VerticalLines { get; set; }

        public LocalViewModel()
        {

            int GridSpacing = 50;
            double canvasHeight = 400; // Example height
            double canvasWidth = 700;  // Example width

            HorizontalLines = new ObservableCollection<double>();
            for (double y = 0; y <= canvasHeight; y += GridSpacing)
            {
                HorizontalLines.Add(y);
                
            }

            VerticalLines = new ObservableCollection<double>();
            for (double x = 0; x <= canvasWidth; x += GridSpacing)
            {
                VerticalLines.Add(x);

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
    }
}
