using System.CodeDom;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Square> Squares { get; set; }
        private bool _whiteplayerturn;
        public bool WhitePlayerTurn
        {
            get => _whiteplayerturn;
            set
            {
                if (value != _whiteplayerturn)
                {
                    _whiteplayerturn = value;
                    if (WhitePlayerTurn)
                    {
                        ShowWhitePawn = true;
                        ShowBlackPawn = false;
                    }
                    else
                    {
                        ShowWhitePawn = false;
                        ShowBlackPawn = true;
                    }
                }
            }
        }

        private bool _showblackpawn;
        public bool ShowBlackPawn
        {
            get => _showblackpawn;
            set
            {
                if (value != _showblackpawn)
                {
                    _showblackpawn = value;
                    OnPropertyChanged(nameof(ShowBlackPawn));
                }
            }
        }

        private bool _showwhitepawn;
        public bool ShowWhitePawn
        {
            get => _showwhitepawn;
            set
            {
                if (value != _showwhitepawn)
                {
                    _showwhitepawn = value;
                    OnPropertyChanged(nameof(ShowWhitePawn));
                }
            }
        }

        private Border? _selectedBorder;
        private Square? _checkingPiece;
        private string _ganeinfo;
        public string GameInfo
        {
            get => _ganeinfo;
            set
            {
                if (value != _ganeinfo)
                {
                    _ganeinfo = value;
                    OnPropertyChanged(nameof(GameInfo));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
            WhitePlayerTurn = true;
            DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InitializeBoard()
        {
            Squares = new();
            for (int i = 0; i < 64; i++)
            {
                var squareColor = i % 2 == 0;
                var square = new Square(squareColor, new Point(i%8+1, 8-i/8));
                Squares.Add(square);
            }
            AddPieces();
        }

        private void AddPieces()
        {
            Squares[0].Piece = new Rook(false);
            Squares[1].Piece = new Knight(false);
            Squares[2].Piece = new Bishop(false);
            Squares[3].Piece = new Queen(false);
            Squares[4].Piece = new King(false);
            Squares[5].Piece = new Bishop(false);
            Squares[6].Piece = new Knight(false);
            Squares[7].Piece = new Rook(false);
            for (int i = 8; i < 16; i++)
            {
                Squares[i].Piece = new Pawn(false);
            }
            for (int i = 48; i < 56; i++)
            {
                Squares[i].Piece = new Pawn(true);
            }
            Squares[56].Piece = new Rook(true);
            Squares[57].Piece = new Knight(true);
            Squares[58].Piece = new Bishop(true);
            Squares[59].Piece = new Queen(true);
            Squares[60].Piece = new King(true);
            Squares[61].Piece = new Bishop(true);
            Squares[62].Piece = new Knight(true);
            Squares[63].Piece = new Rook(true);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Square square && square != null)
            {
                if (_selectedBorder == null && square.Piece == null) return; //If nothing is selected and the square is empty, do nothing
                if (_selectedBorder == null && square.Piece != null && square.Piece.IsWhite == WhitePlayerTurn) // If nothing is selected and the square has a piece, select it
                {
                    _selectedBorder = border;
                    square.Color = Brushes.BlueViolet;
                    square.Piece.IsSelected = true;
                    if (square.Piece is Pawn) CheckPawnsPossiblePaths(square, square.Piece);
                    else CheckPossiblePaths(square, square.Piece);

                    return;
                }
                //If a piece is selected and the player clicks one of his own pieces, change the selection
                if (_selectedBorder != null && _selectedBorder.DataContext is Square selectedSquare && selectedSquare != null && square.Piece != null && square.Piece.IsWhite == selectedSquare.Piece.IsWhite)
                {
                    RevertBackground();
                    square.Color = Brushes.BlueViolet;
                    _selectedBorder = border;
                    var newSquare = _selectedBorder.DataContext as Square;
                    if (newSquare == null || newSquare.Piece == null) return;
                    newSquare.Piece.IsSelected = true;
                    if (square.Piece is Pawn) CheckPawnsPossiblePaths(square, square.Piece);
                    else CheckPossiblePaths(square, square.Piece);
                    return;
                }
                //If a piece is selected and the player clicks a valid square, move the piece
                if (Squares.Any(item => item.Piece != null && item.Piece.IsSelected) && square.Color == Brushes.PaleVioletRed)
                {
                    
                    var oldSquare = Squares.FirstOrDefault(item => item.Piece != null && item.Piece.IsSelected);
                    if (oldSquare != null)
                    {
                        square.Piece = oldSquare.Piece;
                        if (square.Piece is Pawn pawn)
                        {
                            //Check for en passant
                            if (square.Position.Y - oldSquare.Position.Y == 2 || square.Position.Y - oldSquare.Position.Y == -2)
                            {
                                pawn.EnPassant = true;
                            }
                            pawn.MadeFirstMove = true;
                        }
                        //Checks if a move results in blocking a existing check
                        oldSquare.Piece = null;
                        if (CheckIfOwnCheck())
                        {
                            GameInfo = "Invalid move. You cannnot end a move in check!";
                            oldSquare.Piece = square.Piece;
                            square.Piece = null;
                              return;
                        }
                        else
                        {
                            if (CheckIfKingInCheck(square, square.Piece)) GameInfo = $"{(WhitePlayerTurn ? "Black" : "White")} king is checked";
                            else
                            {
                                var kings = Squares.Select(x => x.Piece).OfType<King>().ToList();

                                foreach (var king in kings)
                                {
                                    king.InCheck = false;
                                }
                                GameInfo = "";

                            }
                            oldSquare.Piece = null;
                        }
                            
                    }
                    Squares.Where(item => item.Piece != null && item.Piece.IsWhite == !WhitePlayerTurn && item.Piece is Pawn pawn && pawn.EnPassant).ToList().ForEach(item =>
                    {
                        var pawn1 = item.Piece as Pawn;
                        if (pawn1 != null) pawn1.EnPassant = false;
                    }); //When a player makes a move, remove the en passant ability from all opponent pawns
                    WhitePlayerTurn = !WhitePlayerTurn;
                    RevertBackground();
                    if (IsCheckmate())
                    {
                        GameInfo = $"Checkmate! {(WhitePlayerTurn ? "Black" : "White")} player wins!";
                    }
                    //else if (_checkingPiece != null && _checkingPiece.Piece != null)
                    //{
                    //    GameInfo = $"{(_checkingPiece.Piece.IsWhite ? "White" : "Black")} King is in check!";
                    //}
                    return;
                }
                if (_selectedBorder != null && square.Piece != null && square.Piece.IsSelected)
                {
                    RevertBackground();
                    return;
                }

            }
        }

        private void RevertBackground()
        {
            //Restore default color and images
            foreach (var item in Squares)
            {
                item.Color = item.BlackOrWhite ? Brushes.LightGreen : Brushes.SandyBrown;
                if (item.Piece != null)
                {
                    //if (item.Piece is King king && king != null && king.InCheck) continue; //If the king is in check, don't change its image
                    item.Piece.IsInDanger = false;
                    item.Piece.IsSelected = false;

                }
            }
            _selectedBorder = null;
        }

        private void CheckPawnsPossiblePaths(Square square, IPiece piece)
        {
            //Check moving 1 step, either forward or diagonal
            if (piece.IsWhite ? square.Position.Y + 1 <= 8 : square.Position.Y - 1 >=1)
            {
                //Diagonal left
                if (square.Position.X - 1 >= 1)
                {
                    var newsquare = Squares.FirstOrDefault(x => x.Position.X == square.Position.X - 1 && x.Position.Y == (piece.IsWhite ? square.Position.Y + 1 : square.Position.Y - 1));
                    if (newsquare != null && newsquare.Piece != null && newsquare.Piece.IsWhite != piece.IsWhite)
                    {
                        newsquare.Color = Brushes.PaleVioletRed;
                        newsquare.Piece.IsInDanger = true;
                    }
                    var enpassantsquare = Squares.FirstOrDefault(x => x.Position.X == square.Position.X - 1 && x.Position.Y == square.Position.Y);
                    if (enpassantsquare != null && enpassantsquare.Piece != null && enpassantsquare.Piece is Pawn pawn && pawn.IsWhite != piece.IsWhite && pawn.EnPassant)
                    {
                        var targetSquare = Squares.FirstOrDefault(x => x.Position.X == square.Position.X - 1 && x.Position.Y == square.Position.Y);
                        if (targetSquare != null && targetSquare.Piece != null)
                        {
                            targetSquare.Color = Brushes.PaleVioletRed;
                            pawn.IsInDanger = true;
                        }
                    }
                }
                //Diagonal right
                if (square.Position.X + 1 <= 8)
                {
                    var newsquare = Squares.FirstOrDefault(x => x.Position.X == square.Position.X + 1 && x.Position.Y == (piece.IsWhite ? square.Position.Y + 1 : square.Position.Y - 1));
                    if (newsquare != null && newsquare.Piece != null && newsquare.Piece.IsWhite != piece.IsWhite)
                    {
                        newsquare.Color = Brushes.PaleVioletRed;
                        newsquare.Piece.IsInDanger = true;
                    }
                    var enpassantsquare = Squares.FirstOrDefault(x => x.Position.X == square.Position.X + 1 && x.Position.Y == square.Position.Y);
                    if (enpassantsquare != null && enpassantsquare.Piece != null && enpassantsquare.Piece is Pawn pawn && pawn.IsWhite != piece.IsWhite && pawn.EnPassant)
                    {
                        var targetSquare = Squares.FirstOrDefault(x => x.Position.X == square.Position.X + 1 && x.Position.Y == square.Position.Y);
                        if (targetSquare != null && targetSquare.Piece != null)
                        {
                            targetSquare.Color = Brushes.PaleVioletRed;
                            pawn.IsInDanger = true;
                        }
                    }
                }
                //One step forward
                
                var forwardsquare = Squares.FirstOrDefault(x => x.Position.X == square.Position.X && x.Position.Y == (piece.IsWhite ? square.Position.Y + 1 : square.Position.Y - 1));
                if (forwardsquare != null && forwardsquare.Piece == null)
                {
                    forwardsquare.Color = Brushes.PaleVioletRed;

                    //Check moving 2 steps
                    var pawn = piece as Pawn;
                    if (pawn == null) return;
                    if (!pawn.MadeFirstMove && (piece.IsWhite ? square.Position.Y + 2 <= 8 : square.Position.Y - 2 >= 1))
                    {
                        var twosquare = Squares.FirstOrDefault(x => x.Position.X == square.Position.X && x.Position.Y == (piece.IsWhite ? square.Position.Y + 2 : square.Position.Y - 2));
                        if (twosquare != null && twosquare.Piece == null)
                        {
                            twosquare.Color = Brushes.PaleVioletRed;

                        }
                    }
                }             
            }
            //check for en passant

        }

        private void CheckPossiblePaths(Square square, IPiece piece)
        {
            piece.PossibleDirections.ForEach(item => {
                var newX = square.Position.X + item.X;
                var newY = square.Position.Y + item.Y;
                while (newX >= 1 && newX <= 8 && newY >= 1 && newY <= 8)
                {
                    var newsquare = Squares.FirstOrDefault(x => x.Position.X == newX && x.Position.Y == newY);
                    if (newsquare != null)
                    {
                        if (newsquare.Piece != null && newsquare.Piece.IsWhite == piece.IsWhite) break;
                        newsquare.Color = Brushes.PaleVioletRed;
                        if (newsquare.Piece != null && newsquare.Piece.IsWhite != piece.IsWhite)
                        {
                            newsquare.Piece.IsInDanger = true;
                            break;
                        }
                    }
                    newX += item.X;
                    newY += item.Y;
                    if (piece is Knight || piece is King || piece is Pawn) break; //Knight and King can only move one step in each direction
                }
            });
        }

        private bool CheckIfKingInCheck(Square square, IPiece piece)
        {
            foreach (var item in piece.PossibleDirections) {
                var newX = square.Position.X + item.X;
                var newY = square.Position.Y + item.Y;
                while (newX >= 1 && newX <= 8 && newY >= 1 && newY <= 8)
                {
                    var newsquare = Squares.FirstOrDefault(x => x.Position.X == newX && x.Position.Y == newY);
                    if (newsquare != null)
                    {
                        if (newsquare.Piece != null && newsquare.Piece.IsWhite == piece.IsWhite) return false;
                        if (newsquare.Piece != null && newsquare.Piece.IsWhite != piece.IsWhite && newsquare.Piece is not King) return false;
                        if (newsquare.Piece != null && newsquare.Piece.IsWhite != piece.IsWhite && newsquare.Piece is King)
                        {
                            _checkingPiece = square;
                            var king = newsquare.Piece as King;
                            if (king == null)
                            {
                                return false;
                            }
                            king.InCheck = true;
                            return true;
                        }
                    }
                    newX += item.X;
                    newY += item.Y;
                    if (piece is Knight || piece is King || piece is Pawn) break; //Knight and King can only move one step in each direction
                }               
            }

            return false;
        }

        private bool CheckIfOwnCheck()
        {
            var king = Squares.FirstOrDefault(x => x.Piece != null && x.Piece is King && x.Piece.IsWhite == WhitePlayerTurn);
            if (king == null || king.Piece == null) return false;
            foreach (var item in Squares)
            {
                if (item.Piece != null && item.Piece.IsWhite != WhitePlayerTurn)
                {
                    foreach (var direction in item.Piece.PossibleDirections)
                    {
                        var newX = item.Position.X + direction.X;
                        var newY = item.Position.Y + direction.Y;
                        while (newX >= 1 && newX <= 8 && newY >= 1 && newY <= 8)
                        {
                            var newsquare = Squares.FirstOrDefault(x => x.Position.X == newX && x.Position.Y == newY);
                            if (newsquare != null)
                            {
                                if (newsquare.Piece != null && newsquare.Piece.IsWhite == item.Piece.IsWhite) break;
                                if (newsquare.Piece != null && newsquare.Piece.IsWhite != item.Piece.IsWhite && newsquare != king) break;
                                if (newsquare == king)
                                {
                                    return true;
                                }
                            }
                            newX += direction.X;
                            newY += direction.Y;
                            if (item.Piece is Knight || item.Piece is King || item.Piece is Pawn) break; //Knight and King can only move one step in each direction
                        }
                    }
                }
            }
            return false;
        }

        private bool IsCheckmate()
        {
            var kingSquare = Squares.FirstOrDefault(x => x.Piece is King && x.Piece.IsWhite == WhitePlayerTurn);
            if (kingSquare?.Piece is not King king) return false;
            if (!king.InCheck) return false;

            var checkingSquares = new List<Square>();
            foreach (var item in Squares)
            {
                if (item.Piece != null && item.Piece.IsWhite != WhitePlayerTurn)
                {
                    foreach (var dir in item.Piece.PossibleDirections)
                    {
                        int newX = (int)(item.Position.X + dir.X);
                        int newY = (int)(item.Position.Y + dir.Y);
                        while (newX >= 1 && newX <= 8 && newY >= 1 && newY <= 8)
                        {
                            var target = Squares.FirstOrDefault(s => s.Position.X == newX && s.Position.Y == newY);
                            if (target == kingSquare)
                            {
                                checkingSquares.Add(item);
                                break;
                            }
                            if (target?.Piece != null) break;
                            if (item.Piece is Knight || item.Piece is King || item.Piece is Pawn) break;
                            newX += dir.X;
                            newY += dir.Y;
                        }
                    }
                }
            }

            if (checkingSquares.Count == 0) return false;
            var checkingSquare = checkingSquares.First();

            if (checkingSquares.Count > 1)
            {
                foreach (var dir in king.PossibleDirections)
                {
                    int newX = (int)(kingSquare.Position.X + dir.X);
                    int newY = (int)(kingSquare.Position.Y + dir.Y);
                    if (newX < 1 || newX > 8 || newY < 1 || newY > 8) continue;
                    var targetSquare = Squares.FirstOrDefault(s => s.Position.X == newX && s.Position.Y == newY);
                    if (targetSquare == null) continue;
                    if (targetSquare.Piece != null && targetSquare.Piece.IsWhite == king.IsWhite) continue;

                    IPiece? originalPiece = targetSquare.Piece;
                    targetSquare.Piece = king;
                    kingSquare.Piece = default!;
                    bool stillInCheck = CheckIfOwnCheck();
                    kingSquare.Piece = king;
                    targetSquare.Piece = originalPiece;
                    if (!stillInCheck) return false;
                }
                return true;
            }

            foreach (var dir in king.PossibleDirections)
            {
                int newX = (int)(kingSquare.Position.X + dir.X);
                int newY = (int)(kingSquare.Position.Y + dir.Y);
                if (newX < 1 || newX > 8 || newY < 1 || newY > 8) continue;
                var targetSquare = Squares.FirstOrDefault(s => s.Position.X == newX && s.Position.Y == newY);
                if (targetSquare == null) continue;
                if (targetSquare.Piece != null && targetSquare.Piece.IsWhite == king.IsWhite) continue;

                IPiece? originalPiece = targetSquare.Piece;
                targetSquare.Piece = king;
                kingSquare.Piece = default!;
                bool stillInCheck = CheckIfOwnCheck();
                kingSquare.Piece = king;
                targetSquare.Piece = originalPiece;
                if (!stillInCheck) return false;
            }

            var blockSquares = new List<Square>();
            var dx = Math.Sign(checkingSquare.Position.X - kingSquare.Position.X);
            var dy = Math.Sign(checkingSquare.Position.Y - kingSquare.Position.Y);
            int x = (int)(kingSquare.Position.X + dx);
            int y = (int)(kingSquare.Position.Y + dy);
            while (x != checkingSquare.Position.X || y != checkingSquare.Position.Y)
            {
                var sq = Squares.FirstOrDefault(s => s.Position.X == x && s.Position.Y == y);
                if (sq != null) blockSquares.Add(sq);
                x += dx;
                y += dy;
            }

            if (checkingSquare.Piece is Rook || checkingSquare.Piece is Bishop || checkingSquare.Piece is Queen)
            {
                var originalPiece = checkingSquare.Piece;
                foreach (var pieceSquare in Squares.Where(s => s.Piece != null && s.Piece.IsWhite == WhitePlayerTurn && !(s.Piece is King)))
                {
                    var piece = pieceSquare.Piece!;
                    foreach (var dir in piece.PossibleDirections)
                    {
                        int newX = (int)(pieceSquare.Position.X + dir.X);
                        int newY = (int)(pieceSquare.Position.Y + dir.Y);
                        while (newX >= 1 && newX <= 8 && newY >= 1 && newY <= 8)
                        {
                            var targetSquare = Squares.FirstOrDefault(s => s.Position.X == newX && s.Position.Y == newY);
                            if (targetSquare == null) break;
                            if (targetSquare.Piece != null && targetSquare.Piece.IsWhite == piece.IsWhite) break;

                            foreach (var blockSquare in blockSquares)
                            {
                                if (targetSquare == blockSquare)
                                {
                                    IPiece? temp = targetSquare.Piece;
                                    targetSquare.Piece = piece;
                                    pieceSquare.Piece = null;
                                    bool stillInCheck = CheckIfOwnCheck();
                                    pieceSquare.Piece = piece;
                                    targetSquare.Piece = temp;
                                    if (!stillInCheck) return false;
                                }
                            }

                            if (piece is Knight || piece is King || piece is Pawn) break;
                            newX += dir.X;
                            newY += dir.Y;
                        }
                    }
                }
            }

            return true;
        }

    }
}