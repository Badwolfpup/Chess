using System.ComponentModel;
using System.Windows;
using System.Windows.Media;


namespace Chess
{
    public class Square : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void OnPropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        public bool BlackOrWhite { get; }

        public bool CanMove { get; set; }

        private IPiece _piece;
        public IPiece Piece
        {
            get => _piece;
            set
            {
                if (value != _piece)
                {
                    _piece = value;
                    OnPropertyChanged(nameof(Piece));
                }
            }
        }

        public Point Position { get; set; }

        private SolidColorBrush _color;
        public SolidColorBrush Color
        {
            get => _color;
            set
            {
                if (value != _color)
                {
                    _color = value;
                    OnPropertyChanged(nameof(Color));
                }
            }
        }

        public Square(bool blackOrWhite, Point position)
        {
            BlackOrWhite = blackOrWhite;
            Position = position;
            Color = blackOrWhite ? Brushes.LightGreen : Brushes.SandyBrown;
        }
    }
}
