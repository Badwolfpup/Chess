using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Chess
{
    public interface IPiece
    {
        string ImageSource { get; set; }
        bool IsWhite { get; set; }
        bool IsInDanger { get; set; }
        bool IsSelected { get; set; }

        bool CheckingPiece { get; set; }
        List<(int X, int Y)> PossibleDirections { get; set; }

        ObservableCollection<Square> PossibleMoves { get; set; }
        ObservableCollection<Square> TestPossibleMoves { get; set; }
        void AddDirections();


    }
}
