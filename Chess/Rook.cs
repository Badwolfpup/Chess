using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class Rook : IPiece, INotifyPropertyChanged
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
                    ImageSource = $"pack://application:,,,/Images/{(IsInDanger ? "Red" : "")}{(IsWhite ? "White" : "Black")}Rook.png";
                }
            }
        }
        public bool IsSelected { get; set; }
        public bool CheckingPiece { get; set; }
        public Rook(bool isWhite)
        {
            PossibleMoves = new ObservableCollection<Square>();
            TestPossibleMoves = new ObservableCollection<Square>();
            IsWhite = isWhite;
            ImageSource = $"pack://application:,,,/Images/{(IsWhite ? "White" : "Black")}Rook.png";
            AddDirections();
        }

        public void AddDirections()
        {
            PossibleDirections = new List<(int X, int Y)>
            {
                (1, 0),  // Right
                (0, 1),  // Down
                (-1, 0), // Left
                (0, -1)  // Up
            };
        }
    }
}
