using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Chess
{
    public class King : IPiece, INotifyPropertyChanged
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
                    //ImageSource = $"pack://application:,,,/Images/{(IsInDanger ? "Check" : "")}{(IsWhite ? "White" : "Black")}King.png";
                }
            }
        }
        public bool IsSelected { get; set; }
        private bool _incheck;
        public bool InCheck
        {
            get => _incheck;
            set
            {
                if (value != _incheck)
                {
                    _incheck = value;
                    ImageSource = $"pack://application:,,,/Images/{(InCheck ? "Check" : "")}{(IsWhite ? "White" : "Black")}King.png";
                    OnPropertyChanged(nameof(InCheck));
                }
            }
        }


        public King(bool isWhite)
        {
            IsWhite = isWhite;
            ImageSource = $"pack://application:,,,/Images/{(IsWhite ? "White" : "Black")}King.png";
            AddDirections();
        }

        public void AddDirections()
        {
            PossibleDirections = new List<(int X, int Y)>
            {
                (1, 0),  // Right
                (0, 1),  // Down
                (-1, 0), // Left
                (0, -1), // Up
                (1, 1),  // Down-Right
                (-1, -1),// Up-Left
                (1, -1), // Up-Right
                (-1, 1)  // Down-Left
            };
        }
    }
}
