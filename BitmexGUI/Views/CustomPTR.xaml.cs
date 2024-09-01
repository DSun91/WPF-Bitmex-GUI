using BitmexGUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace BitmexGUI.Views
{
    public partial class CustomPTR : Window, INotifyPropertyChanged
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

        private List<CandlestickData> Candles=new List<CandlestickData>();
        int lenghtPattern = 20;
        public CustomPTR(int spacing, ObservableCollection<CandlestickData> PriceData)
        {
            InitializeComponent();

            DataContext = this;
             
            InitializeGrid(spacing);
            
            foreach (var item in PriceData)
            {
                Candles.Add(item);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void InitializeGrid(int Gridspacing)
        {
            
            double canvasHeight = 2550; // Example height
            double canvasWidth = 1550; // Example width
            GridData2 = new List<GridData>();

            // Vertical lines
            for (double x = -canvasWidth; x <= canvasWidth; x += Gridspacing)
            {
                GridData2.Add(new GridData { X1 = x, Y1 = -canvasWidth, X2 = x, Y2 = canvasWidth });
            }

            // Horizontal lines
            for (double y = -canvasHeight; y <= canvasHeight; y += Gridspacing)
            {
                GridData2.Add(new GridData { X1 = -canvasHeight, Y1 = y, X2 = canvasHeight, Y2 = y });
            }

        }

      
       
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
            ListofPatterns.Clear();
            FindpatterRecursive(PatternPoint);
            
        }


        private int LenghtToCandles(double width, double interspace,double Lenght)
        {
            return (int)Math.Ceiling(((Lenght - width) / interspace) + 1);
        }
        private double CandlesToLengh(int width, int interspace, int NCandles)
        {
            return (interspace*(NCandles - 1)+width);
        }
        
        private List<int> CandlesPosX= new List<int>();

        private int PatternWindow = 50;
         
        private ObservableCollection<Pattern> _patternList = new ObservableCollection<Pattern>();

        public ObservableCollection<Pattern> ListofPatterns
        {
            get { return _patternList; }
            set { _patternList = value;}
        } 
        private bool IsSwing(List<CandlestickData> Candles,int index,string SwingHighorLow,bool onHighLow=false)
        {
            if(onHighLow)
            {
                if (SwingHighorLow.ToLower().Contains("high"))
                {
                    if (Candles[index].High > Candles[index - 1].High && Candles[index].High > Candles[index + 1].High)
                    {
                        return true;
                    }
                }
                else
                {
                    if (Candles[index].Low < Candles[index - 1].Low && Candles[index].Low < Candles[index + 1].Low)
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                if (SwingHighorLow.ToLower().Contains("high"))
                {
                    if (Candles[index].Close > Candles[index - 1].Close && Candles[index].Close > Candles[index + 1].Close)
                    {
                        return true;
                    }
                }
                else
                {
                    if (Candles[index].Close < Candles[index - 1].Close && Candles[index].Close < Candles[index + 1].Close)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        
         

        private List<double> GetDeltaY()
        {
            List<double> deltaY = new List<double>();

            deltaY.Clear();
            for (int i = 0; i < PatternPoint.Count - 1; i++)
            {
                deltaY.Add(-PatternPoint[i + 1].Y + PatternPoint[i].Y);
            }
            
            return deltaY;
        }

        private List<double> GetDeltaX()
        {
            List<double> deltaX = new List<double>();

            deltaX.Clear();
            for (int i = 0; i < PatternPoint.Count - 1; i++)
            {
                deltaX.Add(PatternPoint[i + 1].X - PatternPoint[i].X);
            }
          
            return deltaX;
        }


        List<double> deltaY;
        List<double> deltaX;

        private void FindpatterRecursive(List<Point> PatternPoint)
        {

            if(PatternPoint.Count > 3) 
            {
                deltaY = GetDeltaY();
                deltaX = GetDeltaX();


                List<int> p = new List<int>();


                for (int N = 1; N < Candles.Count - PatternWindow; N++)
                {
                    itercounter = 0;
                    shouldExit = false;
                    p.Clear();
                    p.Add(N);
                    //System.IO.File.AppendAllText(ConfigurationManager.AppSettings["LogFile"], $"\n----------------------------------------------------------------------------------\nfindpatterRecursive N:{N}" + "\n"); 
                    ExecuteForLoop(Candles, 0,ref deltaY, N, ref p);
                    //System.IO.File.AppendAllText(ConfigurationManager.AppSettings["LogFile"], $"Execution returned root function" + "\n");

                }

            }
           
        }

        private bool checkCondition(int i,List<CandlestickData> Candles, List<int> p)
        {
            List<bool> flag = new List<bool>();
            
             
            for (int j=0;j< p.Count;j++)
            {
                double sum = 0;
                for (int k = j; k < p.Count; k++)
                {
                    sum += deltaY[k];
                }
                 
                if (sum > 0)
                {
                    if (Candles[i].Close > Candles[p[j]].Close)
                    {
                        flag.Add(true);
                    }
                    else
                    {
                        flag.Add(false);
                    }
                    
                }
                if(sum<0)
                {
                    if (Candles[i].Close < Candles[p[j]].Close)
                    {
                        flag.Add(true);
                    }
                    else
                    {
                        flag.Add(false);
                    }
                }
                if (sum == 0)
                {
                    flag.Add(true);
                }
                
            }

            if (!flag.Contains(false))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        private bool shouldExit = false;
        int itercounter = 0;
        private void ExecuteForLoop(List<CandlestickData> Candles, int initialpoint,ref List<double> deltaY, int N,ref List<int> p)
        {
             

            for (int variable = initialpoint+1; variable < PatternWindow; variable++)
            {
                if (shouldExit){return;} 
                if (itercounter < deltaY.Count)
                {

                    if (deltaY[itercounter] > 0)
                    {
                        
                        if (checkCondition(N + variable,Candles,p))//if (Candles[N + variable].Close > Candles[p.Last()].Close)
                        {
                            if (IsSwing(Candles, N + variable, "SwingHigh"))
                            {
 
                                
                                p.Add(N + variable);
                                AddPatternToList(p);
                                itercounter++;
                                ExecuteForLoop(Candles, variable, ref deltaY, N, ref p);
                            }
                        }
                        else
                        {
                            shouldExit = true;
                            //System.IO.File.AppendAllText(ConfigurationManager.AppSettings["LogFile"], $"deltaY[{itercounter}] {deltaY[itercounter]}, findpatterRecursive breaking at point {Candles[N + variable].Timestamp} as {Candles[N + variable].Close}<{Candles[p[p.Count - 1]].Close}, {Candles[p[p.Count - 1]].Timestamp}, p.Count: {p.Count}" + "\n");
                            return;
                        }
                        
                    }
                    else
                    {
                        if (checkCondition(N + variable, Candles, p))
                        {
                            if (IsSwing(Candles, N + variable, "SwingLow"))
                            {

                                p.Add(N + variable);
                                AddPatternToList(p);
                                itercounter++;
                                ExecuteForLoop(Candles, variable, ref deltaY, N, ref p);
                            }
                        }
                        else
                        {
                            shouldExit = true;
                            return;
                        }

                    }
                }
              
            }


        }




        private void AddPatternToList(List<int> patternPoints)
        {
            if (patternPoints.Count == 4)
            {
                if (!ListofPatterns.Any(P => P.P0.Timestamp == Candles[patternPoints[0]].Timestamp) &&
                !ListofPatterns.Any(P => P.P1.Timestamp == Candles[patternPoints[1]].Timestamp) &&
                !ListofPatterns.Any(P => P.P2.Timestamp == Candles[patternPoints[2]].Timestamp) &&
                !ListofPatterns.Any(P => P.P3.Timestamp == Candles[patternPoints[3]].Timestamp))
                {

                    Pattern pat = new Pattern
                    {
                        P0 = Candles[patternPoints[0]],
                        P1 = Candles[patternPoints[1]],
                        P2 = Candles[patternPoints[2]],
                        P3 = Candles[patternPoints[3]]


                    };
                    ListofPatterns.Add(pat);
                    OnPropertyChanged(nameof(ListofPatterns));

                    System.IO.File.AppendAllText(ConfigurationManager.AppSettings["LogFile"], "PatternP0 " + Candles[patternPoints[0]].Timestamp.ToString() + " : " + Candles[patternPoints[0]].Close.ToString() + "\n");
                    System.IO.File.AppendAllText(ConfigurationManager.AppSettings["LogFile"], "PatternP1 " + Candles[patternPoints[1]].Timestamp.ToString() + " : " + Candles[patternPoints[1]].Close.ToString() + "\n");
                    System.IO.File.AppendAllText(ConfigurationManager.AppSettings["LogFile"], "PatternP2 " + Candles[patternPoints[2]].Timestamp.ToString() + " : " + Candles[patternPoints[2]].Close.ToString() + "\n");
                    System.IO.File.AppendAllText(ConfigurationManager.AppSettings["LogFile"], "PatternP3 " + Candles[patternPoints[3]].Timestamp.ToString() + " : " + Candles[patternPoints[3]].Close.ToString() + "\n");
                    shouldExit = true;
                }

            }

        }



        /// //////////////////////////////////////////////  ORDER LINE DRAGGING
    }
}