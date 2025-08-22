using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
            
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
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var squareColor = (row + col) % 2 == 0 ? Brushes.White : Brushes.Black;
                    var square = new Square(squareColor, new Point(col, row));
                    Squares.Add(square);
                    // Here you would typically add the square to a grid or canvas in your UI
                    // For example: chessBoard.Children.Add(square);
                }
            }
        }
    }
}