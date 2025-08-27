using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class Bishop : IPiece, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void OnPropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        public ObservableCollection<Square> PossibleMoves { get; set; }
        public ObservableCollection<Square> TestPossibleMoves { get; set; }
        private string _imagesource;
        public string ImageSource
        {
            get => _imagesource;
            set
            {
                if (value != _imagesource)
                {
                    _imagesource = value;
                    OnPropertyChanged(nameof(ImageSource));
                }
            }
        }
        public bool IsWhite { get; set; }
        public List<(int X, int Y)> PossibleDirections { get; set; }
        private bool _isindanger;
        public bool IsInDanger
        {
            get => _isindanger;
            set
            {
                if (value != _isindanger)
                {
                    _isindanger = value;
                    ImageSource = $"pack://application:,,,/Images/{(IsInDanger ? "Red" : "")}{(IsWhite ? "White" : "Black")}Bishop.png";
                }
            }
        }
        public bool IsSelected { get; set; }
        public bool CheckingPiece { get; set; }
        public Bishop(bool isWhite)
        {
            IsWhite = isWhite;
            TestPossibleMoves = new ObservableCollection<Square>();
            PossibleMoves = new ObservableCollection<Square>();
            ImageSource = $"pack://application:,,,/Images/{(IsWhite ? "White" : "Black")}Bishop.png";
            AddDirections();
        }

        public void AddDirections()
        {
            PossibleDirections = new List<(int X, int Y)>
            {
                (1, 1),  // Up-Right
                (1, -1),  // Down-Right
                (-1, 1), // Up-Left
                (-1, -1), // Down-left

            };
        }
    }
}
