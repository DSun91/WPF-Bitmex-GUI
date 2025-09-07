using BitmexGUI.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BitmexGUI.ViewModels
{
  
    public class GridDataViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<GridData>? _gridData;
        public ObservableCollection<GridData> GridData
        {
            get
            {
                return _gridData;
            }
            set
            {
                _gridData = value;
                OnPropertyChanged(nameof(GridData));
            }
        }

        public GridDataViewModel()
        {
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            int GridSpacing = 50;
            double canvasHeight = 450;  
            double canvasWidth = 1400;  
            GridData = new ObservableCollection<GridData>();


             

            // Vertical lines
            for (double x = 100; x <= canvasWidth; x += GridSpacing)
            {
                GridData.Add(new GridData { X1 = x, Y1 = 0, X2 = x, Y2 = canvasHeight });
            }

            // Horizontal lines
            for (double y = 0; y <= canvasHeight; y += GridSpacing)
            {
                GridData.Add(new GridData { X1 = 100, Y1 = y, X2 = canvasWidth, Y2 = y });
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

