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
            int GridSpacing = 25;
            double canvasHeight = 3550;  
            double canvasWidth = 1550;  
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

