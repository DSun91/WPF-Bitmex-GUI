using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BitmexGUI.Views
{
    public class GridData
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }

        public double X2 { get; set; }
        public double Y2 { get; set; }
    }
    internal class LocalViewModel : INotifyPropertyChanged
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
            double canvasHeight = 2550; // Example height
            double canvasWidth = 1550; // Example width
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

 