using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;


namespace Chess
{
    public class Square
    {
        public SolidColorBrush BlackOrWhite { get; }

        public IPiece Piece { get; set; }

        public Point Position { get; set; }

        public Square(SolidColorBrush blackOrWhite, Point position)
        {
            BlackOrWhite = blackOrWhite;
            Position = position;
        }
    }
}
