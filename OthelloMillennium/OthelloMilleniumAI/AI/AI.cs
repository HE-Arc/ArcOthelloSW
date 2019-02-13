using System;

namespace IAOthelloMillenium
{

    internal class AI : IPlayable.IPlayable
    {
        private TreeNode node;

        private static AI instance = null;

        private AI()
        {
            node = new TreeNode(GamePlate.CreateStartState(), false);
        }

        public static AI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AI();
                }
                return instance;
            }
        }

        private int AlphaBeta(TreeNode node, int depth, int a, int b, bool maximizingPlayer, bool isWhite)
        {
            if (depth == 0 || node.IsTerminal) {
                return node.Score(isWhite);
            }
            if (maximizingPlayer) {
                int score = int.MinValue;
                foreach (TreeNode child in node.Children(isWhite).Values) {
                    score = Math.Max(score, AlphaBeta(child, depth-1, a, b, false, isWhite));
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
                foreach (TreeNode child in node.Children(isWhite).Values) {
                    score = Math.Min(score, AlphaBeta(child, depth-1, a, b, true, isWhite));
                    b = Math.Min(b, score);
                    if (a >= b)
                    {
                        break;//a cut-off
                    }
                }
                return score;
            }
        }

        public string GetName()
        {
            return "Millenium-AI";
        }

        public bool IsPlayable(int column, int line, bool isWhite) => node.GameBoard.ValidateMove(new Tuple<int,int>(column, line), isWhite ? Settings.WHITE : Settings.BLACK);

        public bool PlayMove(int column, int line, bool isWhite)
        {
            node = node.Children(isWhite)[new Tuple<int, int>(column, line)];
            return node == null;
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            int bestScore = int.MinValue;
            Tuple<int, int> bestMove = new Tuple<int, int>(-1, -1);
            foreach (var child in node.Children(whiteTurn))
            {
                int score = AlphaBeta(child.Value, level, int.MinValue, int.MaxValue, false, whiteTurn);
                if (score >= bestScore)
                {
                    bestScore = score;
                    bestMove = child.Key;
                }
            }

            return bestMove;
        }

        public int[,] GetBoard() => node.GameBoard.Board;

        public int GetWhiteScore() => node.GameBoard.WhiteScore;

        public int GetBlackScore() => node.GameBoard.BlackScore;
    }
}
