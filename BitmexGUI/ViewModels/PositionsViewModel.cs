using BitmexGUI.Models;
using BitmexGUI.Services.Implementations;
using BitmexGUI.ViewModels.Utilities;
using BitmexGUI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BitmexGUI.ViewModels
{
    public class PositionsViewModel:INotifyPropertyChanged
    {
        BitmexAPI _bitmexApi;
        public event Action PositionsdatsUpdated;
        public PositionsViewModel(BitmexAPI BitmexAPI)
        {
            _bitmexApi = BitmexAPI;
            _bitmexApi.PositionUpdated += OnPositionUpdate;

        }
        private ObservableCollection<PositionLine> _positionLine = new ObservableCollection<PositionLine>();
        public ObservableCollection<PositionLine> PositionsLines
        {
            get => _positionLine;
            set
            {
                _positionLine = value;
                OnPropertyChanged(nameof(PositionsLines));
            }
        }
        #region POSITIONS_SECTION
        private ObservableCollection<Position> _positionData = new ObservableCollection<Position>();
        public ObservableCollection<Position> PositionsInfo
        {
            get => _positionData;
            set
            {
                _positionData = value;
                OnPropertyChanged();
            }
        }

        
        private void UpdatepositionLines(Position newPositionData)
        {

            PositionLine newpositionLine = new PositionLine
            {
                AccountID = newPositionData.AccountID,
                AvgEntryPrice = (decimal)double.Parse(newPositionData.AvgEntryPrice.ToString()),
                Symbol = newPositionData.Symbol,
                BreakEvenPrice = (decimal)double.Parse(newPositionData.BreakEvenPrice.ToString()),
                UnrealisedPnl = (decimal)newPositionData.UnrealisedPnl,
                LiquidationPrice = (decimal)newPositionData.LiquidationPrice,
                DeltaFromBreakEven = (decimal)newPositionData.DeltaFromBreakEven, 
            };

            var existingPositionLine = PositionsLines.FirstOrDefault(p => p.AccountID.Equals(newPositionData.AccountID) && p.Symbol.Equals(newPositionData.Symbol));
            if (existingPositionLine != null)
            {
                PositionsLines.Remove(existingPositionLine);
            }

            PositionsLines.Add(newpositionLine);



        }

       
        private void OnPositionUpdate(Position newPositionData)
        {
            var existingPosition = PositionsInfo.FirstOrDefault(p => p.AccountID == newPositionData.AccountID && p.Symbol == newPositionData.Symbol);


            if (existingPosition != null)
            {

                //MessageBox.Show("existing pos has commision " + existingPosition.PosComm.HasValue.ToString());
                // Create a new Position object with updated data
                var updatedPosition = new Position
                {
                    AccountID = existingPosition.AccountID,
                    Symbol = existingPosition.Symbol,
                    AvgEntryPrice = newPositionData.AvgEntryPrice.HasValue ? newPositionData.AvgEntryPrice.Value : existingPosition.AvgEntryPrice,
                    MarkPrice = newPositionData.MarkPrice.HasValue ? newPositionData.MarkPrice.Value : existingPosition.MarkPrice,
                    BreakEvenPrice = newPositionData.BreakEvenPrice.HasValue ? newPositionData.BreakEvenPrice.Value : existingPosition.BreakEvenPrice,
                    LiquidationPrice = newPositionData.LiquidationPrice.HasValue ? newPositionData.LiquidationPrice.Value : existingPosition.LiquidationPrice,
                    RealisedPnl = newPositionData.RealisedPnl.HasValue ? newPositionData.RealisedPnl.Value : existingPosition.RealisedPnl,
                    UnrealisedPnl = newPositionData.UnrealisedPnl.HasValue ? newPositionData.UnrealisedPnl.Value : existingPosition.UnrealisedPnl,
                    Commission = newPositionData.Commission.HasValue ? newPositionData.Commission.Value : existingPosition.Commission,
                    Leverage = newPositionData.Leverage.HasValue ? newPositionData.Leverage.Value : existingPosition.Leverage,
                    CurrentQty = newPositionData.CurrentQty.HasValue ? newPositionData.CurrentQty.Value : existingPosition.CurrentQty,
                    CurrentCost = newPositionData.CurrentCost.HasValue ? newPositionData.CurrentCost.Value : existingPosition.CurrentCost,
                    RealisedCost = newPositionData.RealisedCost.HasValue ? newPositionData.RealisedCost.Value : existingPosition.RealisedCost,
                    PosComm = newPositionData.PosComm.HasValue ? newPositionData.PosComm.Value : existingPosition.PosComm,
                    MarkValue = newPositionData.MarkValue.HasValue ? newPositionData.MarkValue.Value : existingPosition.MarkValue,
                    RebalancedPnl = newPositionData.RebalancedPnl.HasValue ? newPositionData.RebalancedPnl.Value : existingPosition.RebalancedPnl,
                    HomeNotional = newPositionData.HomeNotional.HasValue ? newPositionData.HomeNotional.Value : existingPosition.HomeNotional,
                    ForeignNotional = newPositionData.ForeignNotional.HasValue ? newPositionData.ForeignNotional.Value : existingPosition.ForeignNotional,
                };

                int index = PositionsInfo.IndexOf(existingPosition);
                PositionsInfo[index] = updatedPosition;
                UpdatepositionLines(updatedPosition);

                if (updatedPosition.MarkValue == 0 && updatedPosition.RebalancedPnl == 0)
                {
                    PositionsLines.Clear();
                    PositionsInfo.RemoveAt(index); 
                }
                 

            }

             
            else if (Math.Abs((decimal)newPositionData.CurrentQty) > 0)
            {

                PositionsInfo.Add(newPositionData);
                UpdatepositionLines(newPositionData);
                PositionsdatsUpdated?.Invoke();
            }


        }

        private ICommand _closePositionCommand;

         
        public ICommand ClosePositionCommand
        {
            get
            {
                if (_closePositionCommand == null)
                {
                    _closePositionCommand = new RelayCommand(param =>
                    {
                        var parameters = (Tuple<string, string>)param;
                        string symbol = parameters.Item1;
                        string Type = parameters.Item2;
                        ClosePosition(symbol, Type);
                    });
                }
                return _closePositionCommand;
            }
        }

        public void ClosePosition(string Symbol, string Type)
        {

            var existingPosition = PositionsInfo.FirstOrDefault(p => p.Symbol == Symbol);
            if (existingPosition != null)
            {

                //PositionsLines.Clear();
                //PositionsInfo.Clear();
                _bitmexApi.ClosePosition(Type, existingPosition);
            }

        }
         
        #endregion
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
