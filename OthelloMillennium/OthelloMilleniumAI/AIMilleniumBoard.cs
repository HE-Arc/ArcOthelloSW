using System;
using System.Collections.Generic;

namespace IAOthelloMillenium
{
    /// <summary>
    /// Main class
    /// </summary>
    public class IAMilleniumBoard : IPlayable.IPlayable
    {
        private TreeNode node;

        /// <summary>
        /// Constructor
        /// </summary>
        public IAMilleniumBoard()
        {
            node = new TreeNode(GameState.CreateStartState(), false);
        }

        /// <summary>
        /// AlphaBeta recursive algorithm
        /// </summary>
        /// <param name="node">Evaluated node</param>
        /// <param name="depth">Current depth search</param>
        /// <param name="a">Alpha</param>
        /// <param name="b">Beta</param>
        /// <param name="isWhite">CurrentPlayerTurn</param>
        /// <returns>The maximized/minimized score</returns>
        private int AlphaBeta(TreeNode node, int depth, int a, int b, bool isWhite)
        {
            if (depth == 0 || node.IsTerminal)
            {
                return node.Score(isWhite);
            }
            
            if (node.WhiteTurn == isWhite)
            {
                int score = int.MinValue;
                foreach (TreeNode child in node.Children(!node.WhiteTurn).Values)
                {
                    score = Math.Max(score, AlphaBeta(child, depth - 1, a, b, isWhite));
                    a = Math.Max(a, score);
                    if (a >= b)
                    {
                        break;//b cut-off
                    }
                }
                return score;
            }
            else
            {
                int score = int.MaxValue;
                foreach (TreeNode child in node.Children(!node.WhiteTurn).Values)
                {
                    score = Math.Min(score, AlphaBeta(child, depth - 1, a, b, isWhite));
                    b = Math.Min(b, score);
                    if (a >= b)
                    {
                        break;//a cut-off
                    }
                }
                return score;
            }
        }

        /// <summary>
        /// Return the name of the AI
        /// </summary>
        /// <returns>IA's Name</returns>
        public string GetName()
        {
            return "Millenium-AI";
        }

        /// <summary>
        /// Validate if a move is allowed
        /// </summary>
        /// <param name="column">column</param>
        /// <param name="line">line</param>
        /// <param name="isWhite">Player turn</param>
        /// <returns>True if the move is valid</returns>
        public bool IsPlayable(int column, int line, bool isWhite) => node.GameBoard.ValidateMove(new Tuple<int, int>(column, line), isWhite ? Settings.WHITE : Settings.BLACK);

        /// <summary>
        /// Play a move
        /// </summary>
        /// <param name="column">column</param>
        /// <param name="line">line</param>
        /// <param name="isWhite">player</param>
        /// <returns>True if the play is a success</returns>
        public bool PlayMove(int column, int line, bool isWhite)
        {
            node = node.Children(isWhite)[new Tuple<int, int>(column, line)];
            return node == null;
        }

        /// <summary>
        /// Calculate the next move
        /// </summary>
        /// <param name="game"></param>
        /// <param name="level"></param>
        /// <param name="whiteTurn"></param>
        /// <returns></returns>
        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            // Detect block move
            foreach (var play in node.Children(whiteTurn))
            {
                if (play.Value.WhiteTurn == whiteTurn)
                {
                    Console.WriteLine("Killer move skipped player !");
                    return play.Key;
                }
            }

            // Detect corner opportunities
            //foreach (var play in node.Children(whiteTurn))
            //{
            //    foreach (Tuple<int,int> POINT in Settings.CORNERS)
            //    {
            //        if(POINT.Item1 == play.Key.Item1 && POINT.Item2 == play.Key.Item2)
            //        {
            //            Console.WriteLine("Killer move corner !");
            //            return play.Key;
            //        }
            //    }
            //}

            // Search in alpha beta tree
            int bestScore = int.MinValue;
            Tuple<int, int> bestMove = new Tuple<int, int>(-1, -1);
            foreach (var child in node.Children(whiteTurn))
            {
                int score = AlphaBeta(child.Value, level - 1, int.MinValue, int.MaxValue, whiteTurn);
                if (score >= bestScore)
                {
                    bestScore = score;
                    bestMove = child.Key;
                }
            }

            return bestMove;
        }

        /// <summary>
        /// Return the board
        /// </summary>
        /// <returns></returns>
        public int[,] GetBoard() => node.GameBoard.Board;

        /// <summary>
        /// Get the white score
        /// </summary>
        /// <returns>White player score</returns>
        public int GetWhiteScore() => node.GameBoard.WhiteScore;

        /// <summary>
        /// Get the black player score
        /// </summary>
        /// <returns>Black player score</returns>
        public int GetBlackScore() => node.GameBoard.BlackScore;
    }
}
