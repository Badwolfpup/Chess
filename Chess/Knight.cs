using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class Knight : IPiece, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void OnPropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
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
        public bool IsWhite { get; set; } = false;
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
                    ImageSource = $"pack://application:,,,/Images/{(IsInDanger ? "Red" : "")}{(IsWhite ? "White" : "Black")}Knight.png";
                }
            }
        }
        public bool IsSelected { get; set; }

        public Knight(bool isWhite)
        {
            IsWhite = isWhite;
            ImageSource = $"pack://application:,,,/Images/{(IsWhite ? "White" : "Black")}Knight.png";
            AddDirections();
        }

        public void AddDirections()
        {
            PossibleDirections = new List<(int X, int Y)>
            {
                (2, 1),  // Right-Down
                (2, -1), // Right-Up
                (-2, 1), // Left-Down
                (-2, -1),// Left-Up
                (1, 2),  // Down-Right
                (1, -2), // Up-Right
                (-1, 2), // Down-Left
                (-1, -2) // Up-Left
            };
        }
    }
}
