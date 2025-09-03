using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Backing Fields
        private double _screenheight;
        private double _screenwidth;
        private bool _whiteplayerturn;
        private bool _showblackpawn;
        private bool _showwhitepawn;
        private Border? _selectedBorder;
        private Square _clickedsquare;
        private string _ganeinfo;
        #endregion

        #region Private Fields
        private bool _isgameover;
        private bool _hasmoved;
        #endregion

        #region Public properties
        public ObservableCollection<Square> Squares { get; set; }
        public Square ClickedSquare
        {
            get => _clickedsquare;
            set
            {
                if (value != _clickedsquare)
                {
                    _clickedsquare = value;
                    OnPropertyChanged(nameof(ClickedSquare));
                }
            }
        }
        public double ScreenHeight
        {
            get => _screenheight;
            set
            {
                if (_screenheight != value)
                {
                    _screenheight = value;
                    OnPropertyChanged(nameof(ScreenHeight));
                }
            }
        }
        public double ScreenWidth
        {
            get => _screenwidth;
            set
            {
                if (_screenwidth != value)
                {
                    _screenwidth = value;
                    OnPropertyChanged(nameof(ScreenWidth));
                }
            }
        }
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
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            SetScreenDimensions();
            InitializeBoard();
            WhitePlayerTurn = true;
            DataContext = this;
        }

        private void SetScreenDimensions()
        {
            ScreenHeight = SystemParameters.PrimaryScreenHeight < 1200 ? SystemParameters.PrimaryScreenHeight * 0.9 : 1100;
            ScreenWidth = ScreenHeight;
        }

        //Generates the squares
        private void InitializeBoard()
        {
            ClickedSquare = default!;
            Squares = new();
            for (int i = 0; i < 64; i++)
            {
                var squareColor = i % 2 == 0;
                var square = new Square(squareColor, new Point(i % 8 + 1, 8 - i / 8));
                Squares.Add(square);
            }
            AddPieces();
        }

        //Add the pieces to the board
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

        //Resets the background of the board as well as some flags
        private void Reset()
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
                    item.Piece.CheckingPiece = false;
                }
            }
            _hasmoved = false;
            _selectedBorder = null;
        }

        //Base-handler for all interactivity
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Square square && square != null)
            {
                if (_isgameover) return; //If the game is over, do nothing
                if (_selectedBorder == null && square.Piece == null) return; //If nothing is selected and the square is empty, do nothing

                // If nothing is selected and the square has a piece, select it
                if (_selectedBorder == null && square.Piece != null && square.Piece.IsWhite == WhitePlayerTurn)
                {
                    Reset();
                    ClickedSquare = square;
                    _selectedBorder = border;
                    square.Color = Brushes.BlueViolet;
                    square.Piece.IsSelected = true;
                    CheckAllPaths();
                    return;
                }

                //If a piece is selected and the player clicks one of his own pieces, change the selection (or do nothing if its same pice)
                if (_selectedBorder != null && _selectedBorder.DataContext is Square selectedSquare && selectedSquare != null && square.Piece != null && square.Piece.IsWhite == selectedSquare.Piece.IsWhite)
                {
                    Reset();
                    ClickedSquare = square;
                    square.Color = Brushes.BlueViolet;
                    _selectedBorder = border;
                    var newSquare = _selectedBorder.DataContext as Square;
                    if (newSquare == null || newSquare.Piece == null) return;
                    newSquare.Piece.IsSelected = true;
                    CheckAllPaths();
                    return;
                }
                //If a piece is selected and the player clicks a valid square, move the piece
                if (Squares.Any(item => item.Piece != null && item.Piece.IsSelected) && ClickedSquare.Piece.PossibleMoves.Any(x => x == square))
                {
                    var newsquare = ClickedSquare.Piece.PossibleMoves.FirstOrDefault(x => x == square); //Matches the clicked square against the previously clicked square's possible move
                    if (newsquare == null) return;
                    var savenewsquarepiece = newsquare.Piece != null ? newsquare.Piece : default!;
                    newsquare.Piece = ClickedSquare.Piece;
                    ClickedSquare.Piece = default!;
                    CheckAllPaths();
                    if (Squares.Where(x => x.Piece != null && x.Piece.IsWhite != WhitePlayerTurn).Any(x => x.Piece != null && x.Piece.PossibleMoves.Any(x => x == Squares.FirstOrDefault(x => x.Piece != null && x.Piece is King && x.Piece.IsWhite == WhitePlayerTurn))))
                    {
                        GameInfo = "Invalid move. You cannnot end a move in check!";
                        ClickedSquare.Piece = newsquare.Piece;
                        newsquare.Piece = savenewsquarepiece != null ? savenewsquarepiece : default!;
                        CheckAllPaths();
                        return;
                    }
                    //If the move is valid it checks if the move is enpassant
                    else
                    {
                        if (newsquare.Piece is Pawn pawn)
                        {
                            //If the moved piece is a pawn and a initial two-square move and it lands right next to an enemy pawn, it will be marked as enpassant pawn
                            if (newsquare.Position.Y - ClickedSquare.Position.Y == 2 || newsquare.Position.Y - ClickedSquare.Position.Y == -2)
                            {
                                var leftsquare = Squares.FirstOrDefault(x => (newsquare.Position.X - 1 >= 1 && x.Position.X == newsquare.Position.X - 1) && x.Position.Y == newsquare.Position.Y);
                                var rightsquare = Squares.FirstOrDefault(x => (newsquare.Position.X + 1 <= 8 && x.Position.X == newsquare.Position.X + 1) && x.Position.Y == newsquare.Position.Y);
                                if ((leftsquare != null && leftsquare.Piece != null && leftsquare.Piece.IsWhite != WhitePlayerTurn) || (rightsquare != null && rightsquare.Piece != null && rightsquare.Piece.IsWhite != WhitePlayerTurn))
                                {
                                    pawn.EnPassant = true;
                                }
                            }
                            if (savenewsquarepiece is Pawn p && p.EnPassant)
                            {
                                var enpassantsquare = Squares.FirstOrDefault(x => x.Position == new Point(newsquare.Position.X, newsquare.Position.Y + (newsquare.Piece.IsWhite ? 1 : -1)));
                                if (enpassantsquare != null)
                                {
                                    enpassantsquare.Piece = newsquare.Piece;
                                    newsquare.Piece = default!;
                                }
                            }
                        }
                        _hasmoved = true;
                    }
                    //Rechecks all possible moves after a change has been made
                    CheckAllPaths();

                    //When a player makes a move, remove the en passant ability from all opponent pawns
                    Squares.Where(item => item.Piece != null && item.Piece.IsWhite == !WhitePlayerTurn && item.Piece is Pawn pawn && pawn.EnPassant).ToList().ForEach(item =>
                    {
                        var pawn1 = item.Piece as Pawn;
                        if (pawn1 != null) pawn1.EnPassant = false;
                    });
                    Reset();
                    WhitePlayerTurn = !WhitePlayerTurn;
                    return;
                }
            }
        }



        //Check which pieces a selected piece can knock off the board
        private void PieceCanBeTaken()
        {
            foreach (var item in Squares)
            {
                if (item.Piece != null) item.Piece.IsInDanger = false;
            }
            if (ClickedSquare == null || ClickedSquare.Piece == null) return;
            foreach (var item in ClickedSquare.Piece.PossibleMoves)
            {
                if (item.Piece != null && item.Piece.IsWhite != WhitePlayerTurn) item.Piece.IsInDanger = true;
            }
        }

        //Adds all possible move for every piece 
        private void PossiblePaths(Square square, IPiece piece)
        {
            piece.PossibleDirections.ForEach(item =>
            {
                var newX = square.Position.X + item.X;
                var newY = square.Position.Y + item.Y;
                while (newX >= 1 && newX <= 8 && newY >= 1 && newY <= 8)
                {
                    var newsquare = Squares.FirstOrDefault(x => x.Position.X == newX && x.Position.Y == newY);
                    if (newsquare != null)
                    {
                        if (newsquare.Piece != null && newsquare.Piece.IsWhite == piece.IsWhite) break; //You cannot knock your own piece off the board
                        if (piece is Pawn)
                        {
                            if (newsquare.Piece == null && newX != square.Position.X) break; //The pawn cannot move diagonal to an emtpy square
                            if (newsquare.Piece != null && newX == square.Position.X) break; //The pawn cannot knock a piece with a forward move
                            if ((item.Y == 2 || item.Y == -2) && !(square.Position.Y == 2 || square.Position.Y == 7)) break; //Two square movement is only allowed from initial position
                            else if ((item.Y == 2 || item.Y == -2) && (square.Position.Y == 2 || square.Position.Y == 7))
                            {
                                if (item.Y == 2 && Squares.FirstOrDefault(x => x.Position.X == item.X && x.Position.Y == newsquare.Position.Y - 1) is Square x && x != null && x.Piece != null) break; //Checks if the first square was empty
                                if (item.Y == -2 && Squares.FirstOrDefault(x => x.Position.X == item.X && x.Position.Y == newsquare.Position.Y + 1) is Square y && y != null && y.Piece != null) break; //Checks if the first square was empty
                            }
                            if ((item.X == -1 && item.Y == 0) || (item.X == 1 && item.Y == 0)) //Only adds the enpassant move if the piece has that flag toggled
                            {
                                if (newsquare.Piece == null) break;
                                if (newsquare.Piece != null && newsquare.Piece is not Pawn) break;
                                if (newsquare.Piece != null && newsquare.Piece is Pawn p && !p.EnPassant) break;
                            }
                        }
                        piece.PossibleMoves.Add(newsquare);
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

        //Every time you click on a piece it will check the possbile movements of every piece, and which pieces the clicked piece can knock off the board and check if the king is  in check
        private void CheckAllPaths()
        {
            //Checks the possible squares every piece can move to
            foreach (var item in Squares)
            {
                if (item.Piece != null)
                {
                    item.Piece.PossibleMoves.Clear();
                    PossiblePaths(item, item.Piece);
                }
            }

            //Checks which pieces the clicked piece can knock off the board
            PieceCanBeTaken();

            //Checks it the king is curerntly in check
            KingIsChecked();

            // Notify UI to refresh all squares
            foreach (var square in Squares)
            {
                square.OnPropertyChanged(nameof(Square.Color));
            }
        }

        //Checks if the kings is in check, as well as it it is checkmate
        private void KingIsChecked()
        {

            if (Squares.Where(x => x.Piece != null && x.Piece.IsWhite == WhitePlayerTurn).SelectMany(y => y.Piece.PossibleMoves).Any(z => z.Piece is King))
            {
                var checkingpiece = Squares.Where(x => x.Piece != null && x.Piece.IsWhite == WhitePlayerTurn).FirstOrDefault(y => y.Piece.PossibleMoves.Any(z => z.Piece is King));
                if (checkingpiece == null) return;
                checkingpiece.Piece.CheckingPiece = true;
                if (IsCheckmate(checkingpiece))
                {
                    _isgameover = true;
                    GameInfo = $"Checkmate! {(!WhitePlayerTurn ? "Black" : "White")} player wins!";
                    return;
                }
                else
                {
                    GameInfo = $"{(WhitePlayerTurn ? "Black" : "White")} king is checked";
                }
            }
            else if (_hasmoved) GameInfo = "";

        }

        //Handles the logic to check if the king can move to a safe spot or if any other piece can break the check
        private bool IsCheckmate(Square checkingpiece)
        {

            if (checkingpiece == null || checkingpiece.Piece == null) return false;
            var kingSquare = Squares.FirstOrDefault(x => x.Piece != null && x.Piece is King && x.Piece.IsWhite != WhitePlayerTurn);
            if (kingSquare == null) return false;
            //Checks if thw king has any possible moves that would get it out of check
            foreach (var item in kingSquare.Piece.PossibleMoves)
            {
                if (item.Piece != null && item.Piece.IsWhite == WhitePlayerTurn) continue; //If the square is occupied by a piece of the same color, skip it
                if (item.Piece == null && !CanKingMove(item)) return false; //If the square is empty and not in check, the king can move there
                if (item.Piece != null && item.Piece.IsWhite != WhitePlayerTurn && !CanKingMove(item)) return false; //If square has piece of opposite color and that square is not in check, the kingcan move there
            }
            //Doesnt only check for the swuares which check..
            var checkingsquares = FindCheckingSquares(checkingpiece);
            foreach (var item in checkingsquares)
            {
                if (CanPieceBlockChess(item)) return false;
            }
            if (CanPieceBlockChess(checkingpiece)) return false; //Check if any piece can take the checking piece
            return true;
        }

        private List<Square> FindCheckingSquares(Square checkingpiece)
        {
            var list = new List<Square>();
            if (checkingpiece == null || checkingpiece.Piece == null) return default!;
            list.Add(checkingpiece);
            if (checkingpiece.Piece is Knight || checkingpiece.Piece is Pawn) return list;

            var king = Squares.FirstOrDefault(x => x.Piece != null && x.Piece.IsWhite != WhitePlayerTurn && x.Piece is King);
            if (king != null)
            {
                var diffX = checkingpiece.Position.X - king.Position.X;
                var diffY = checkingpiece.Position.Y - king.Position.Y;
                while (diffX != 0 && diffY != 0)
                {
                    diffX = diffX > 0 ? diffX - 1 : diffX + 1;
                    diffY = diffY > 0 ? diffY - 1 : diffY + 1;
                    var s = Squares.FirstOrDefault(x => x.Position.X == king.Position.X + diffX && x.Position.Y == king.Position.Y + diffY);
                    if (s != null && s != king) list.Add(s);
                }
            }
            return list;
        }

        //Checks if the checked king's potential moves would result in still beeing in check
        private bool CanKingMove(Square square)
        {
            var oppositepieces = Squares.Where(x => x.Piece != null && x.Piece.IsWhite == WhitePlayerTurn).ToList(); //Get all pieces of the player who is checking the king
            var allmoves = oppositepieces.SelectMany(x => x.Piece.PossibleMoves).ToList(); //Get all possible moves of those pieces
            if (allmoves.Any(x => x == square)) return true; //If any of those moves can move to the square the king wants to move to, the king cannot move there
            return false;

        }

        //Checks if any other pice than the king could make a move the would either result in blocking the chess or knocking the checking piece off the board
        private bool CanPieceBlockChess(Square square)
        {
            var oppositepieces = Squares.Where(x => x.Piece != null && x.Piece.IsWhite != WhitePlayerTurn && x.Piece is not King).ToList(); //Get all pieces of the player who beeing checked
            var allmoves = oppositepieces.SelectMany(x => x.Piece.PossibleMoves).ToList(); //Get all possible moves of those pieces
            var king = Squares.FirstOrDefault(x => x.Piece != null && x.Piece.IsWhite != WhitePlayerTurn && x.Piece is King);

            var blockingsquares = oppositepieces.Where(x => x.Piece.PossibleMoves.Any(y => y == square && y != king)).ToList();
            var move = allmoves.FirstOrDefault(x => x == square);
            if (allmoves.Any(x => x == square)) return true; //If any of those moves can move to the checkingpiece's possible moves, the piece can block the check
            return false;
        }




    }

    //Handles the coloring of the squares
    public class BGColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var square = values[1] as Square;
            var clicked = values[2] as Square;

            if (square == null) return Brushes.Transparent;

            if (square.Color == Brushes.BlueViolet) return Brushes.BlueViolet;

            if (clicked != null && clicked.Piece != null && clicked.Piece.PossibleMoves != null)
            {
                if (clicked.Piece.PossibleMoves.Count > 0 && clicked.Piece.PossibleMoves.Any(x => x == square)) return Brushes.PaleVioletRed;
                else return square.BlackOrWhite ? Brushes.LightGreen : Brushes.SandyBrown;
            }
            else return square.BlackOrWhite ? Brushes.LightGreen : Brushes.SandyBrown;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //Handles dynamic resizing depending on screen resolution
    public class MainGridDimensionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            var dim = (double)value;
            return value;
            //return dim / 11 * 10;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}