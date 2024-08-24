using BitmexGUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace BitmexGUI.Views
{
    public partial class CustomPTR : Window
    {
        private List<GridData> _gridData;
        public List<GridData> GridData2
        {
            get => _gridData;
            set
            {
                _gridData = value;

            }
        }
         
        public CustomPTR(int spacing,double max, double min)
        {
            InitializeComponent();
             
            DataContext = this;
            InitializeGrid(spacing,max,min);


        }
        private void InitializeGrid(int spacing, double max, double min)
        {
            
            double canvasHeight = 2550; // Example height
            double canvasWidth = 1550; // Example width
            GridData2 = new List<GridData>();

            // Vertical lines
            for (double x = -canvasWidth; x <= canvasWidth; x += spacing)
            {
                GridData2.Add(new GridData { X1 = x, Y1 = -canvasWidth, X2 = x, Y2 = canvasWidth });
            }

            // Horizontal lines
            for (double y = -canvasHeight; y <= canvasHeight; y += spacing)
            {
                GridData2.Add(new GridData { X1 = -canvasHeight, Y1 = y, X2 = canvasHeight, Y2 = y });
            }

        }

        /// //////////////////////////////////////////////  ORDER LINE DRAGGING
       
        private Point PatternPosition ;
        private Point ClickPointCanvas;
        private List<Point> PatternPoint = new List<Point>();

       

        private void Pattern_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                PatternPosition = e.GetPosition(canvas);
                PatternPoint.Add(PatternPosition);
                Ellipse point = new Ellipse
                {
                    Width = 10, // Diameter is twice the radius
                    Height = 10,
                    Fill = Brushes.Red, // Color of the point
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                   
                };
                point.MouseRightButtonDown += Pattern_MouseRightButtonDown;
                point.MouseMove += Pattern_MouseMove;
                point.MouseRightButtonUp += Pattern_MouseRightButtonUp;
                Canvas.SetLeft(point, PatternPosition.X - point.Width / 2);
                Canvas.SetTop(point, PatternPosition.Y - point.Height / 2);
                canvas.Children.Add(point);

                drawPatternLegs(PatternPoint, canvas);



                canvas.CaptureMouse();  
                }
        }
        private void Pattern_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Canvas Canvas)
            {
                
                Canvas.ReleaseMouseCapture(); // Release the mouse capture when dragging is finished


            }


        }

        private bool isMoving = false;
        private Ellipse currentEllipse = null;

        // Store the index of the moving point
        private int movingIndex = -1;

        private void Pattern_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMoving && currentEllipse != null)
            {
                var mousePos = e.GetPosition(PatterRecognitionCanvas);

                double deltaX = mousePos.X - ClickPointCanvas.X;
                double deltaY = mousePos.Y - ClickPointCanvas.Y;

                // Update the position of the ellipse
                Canvas.SetLeft(currentEllipse, PatternPosition.X - currentEllipse.Width / 2 + deltaX);
                Canvas.SetTop(currentEllipse, PatternPosition.Y - currentEllipse.Height / 2 + deltaY);

                // Update the corresponding point in PatternPoint
                if (movingIndex >= 0 && movingIndex < PatternPoint.Count)
                {
                    PatternPoint[movingIndex] = new Point(
                        Canvas.GetLeft(currentEllipse) + currentEllipse.Width / 2,
                        Canvas.GetTop(currentEllipse) + currentEllipse.Height / 2
                    );
                }

                // Redraw the lines based on the updated points
                drawPatternLegs(PatternPoint, PatterRecognitionCanvas);
            }
        }

        private void Pattern_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Ellipse point)
            {
                isMoving = true;
                currentEllipse = point;

                // Find the index of the point in PatternPoint
                movingIndex = PatternPoint.IndexOf(new Point(
                    Canvas.GetLeft(point) + point.Width / 2,
                    Canvas.GetTop(point) + point.Height / 2
                ));

                // Initialize PatternPosition and ClickPointCanvas
                PatternPosition = new Point(Canvas.GetLeft(point) + point.Width / 2, Canvas.GetTop(point) + point.Height / 2);
                ClickPointCanvas = e.GetPosition(PatterRecognitionCanvas);

                point.CaptureMouse();
            }
        }

        private void Pattern_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isMoving)
            {
                isMoving = false;
                if (currentEllipse != null)
                {
                    currentEllipse.ReleaseMouseCapture();
                    currentEllipse = null;
                    movingIndex = -1;
                }
            }
        }

        private void drawPatternLegs(List<Point> PatternPoint, Canvas canvas)
        {
            // Remove existing lines that match the "leg" tag
            var linesToRemove = canvas.Children.OfType<Line>()
                                      .Where(line => line.Tag != null && line.Tag.ToString().Contains("leg"))
                                      .ToList();

            foreach (var line in linesToRemove)
            {
                canvas.Children.Remove(line);
            }

            // Draw new lines connecting the updated points
            if (PatternPoint.Count > 1)
            {
                for (int i = 0; i < PatternPoint.Count - 1; i++)
                {
                    Point p1 = PatternPoint[i];
                    Point p2 = PatternPoint[i + 1];

                    Line line = new Line
                    {
                        Tag = $"canvas leg{i}",
                        X1 = p1.X,
                        Y1 = p1.Y,
                        X2 = p2.X,
                        Y2 = p2.Y,
                        Stroke = Brushes.Black, // Color of the line
                        StrokeThickness = 2
                    };
                    canvas.Children.Add(line);
                }
            }
        }



        /// //////////////////////////////////////////////  ORDER LINE DRAGGING
    }
}