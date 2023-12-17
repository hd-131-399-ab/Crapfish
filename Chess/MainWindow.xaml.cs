using Chess.Engine.Chess;
using System.Windows;

namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow _MainWindow;

        public MainWindow()
        {
            //Widmung: Magerquark_kls <= 10€

            InitializeComponent();
            _MainWindow = this;

            ChessGame chessGame = new("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", ChessGame.GameMode.LocalMultiplayer);
        }
    }
}

//1g = 54,90€
//1 line = 4,99€

//3549 lines : 17.409,51€
//4102 lines (1.364,60g): 20.468,98€
//M + KG6