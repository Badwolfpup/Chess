using System;
using System.Collections.Generic;
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
        List<(int X, int Y)> PossibleDirections { get; set; }

        void AddDirections();


    }
}
