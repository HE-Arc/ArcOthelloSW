using IAOthelloMillenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloMilleniumTestIA
{
    class Program
    {
        const int BOARDSIZE_X = 9;
        const int BOARDSIZE_Y = 7;

        static void Main(string[] args)
        {

            int[,] board = GameState.CreateStartState().Board;
            int level = 5;
            IAMilleniumBoard ia1 = new IAMilleniumBoard();
            IAMilleniumBoard ia2 = new IAMilleniumBoard();


            IAMilleniumBoard iaTurn = ia1;
            bool isWhite = false;
            while (true)
            {
                Tuple<int, int> move = iaTurn.GetNextMove(board, level, isWhite);
                ia1.PlayMove(move.Item1, move.Item2, isWhite);
                ia2.PlayMove(move.Item1, move.Item2, isWhite);

                isWhite = !isWhite;
                iaTurn = isWhite ? ia2 : ia1;

                Program.DrawBoard(iaTurn.GetBoard(), "Test", iaTurn.GetBlackScore(), iaTurn.GetWhiteScore());
                Console.WriteLine("Stable 1, 0");
                Console.WriteLine(ia1.node.GameBoard.GetNbStableToken(1));
                Console.WriteLine(ia2.node.GameBoard.GetNbStableToken(0));

            }

            Console.ReadLine();
        }

        /// <summary>
        /// Displays in the console the board given as argument with players scores
        /// </summary>
        /// <param name="board"></param>
        public static void DrawBoard(int[,] board, string name, int blackScore, int whiteScore)
        {
            Console.WriteLine(name + "\tBLACK [X]:" + blackScore + "\tWHITE [O]:" + whiteScore);
            Console.WriteLine("  A B C D E F G H I");
            for (int line = 0; line < BOARDSIZE_Y; line++)
            {
                Console.Write($"{(line + 1)}");
                for (int col = 0; col < BOARDSIZE_X; col++)
                {
                    Console.Write((board[col, line] == -1) ? " -" : (board[col, line] == 0) ? " O" : " X");
                }
                Console.Write("\n");
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
