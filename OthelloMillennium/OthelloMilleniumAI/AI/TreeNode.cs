using System;
using System.Collections.Generic;

namespace IAOthelloMillenium
{
    /// <summary>
    /// Single TreeNode for the AI
    /// </summary>
    public class TreeNode
    {
        public GameState GameBoard { get; private set; }

        //Current player turn
        public bool WhiteTurn { get; private set; }

        public bool IsTerminal => GameBoard.GameEnded;
        public int Score(bool isWhite) => isWhite ? GameBoard.WhiteScore: GameBoard.BlackScore;


        private Dictionary<Tuple<int,int>, TreeNode> whiteMoves;
        private Dictionary<Tuple<int, int>, TreeNode> blackMoves;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameBoard">GameBoard</param>
        /// <param name="whiteTurn">PlayerTurn</param>
        public TreeNode(GameState gameBoard, bool whiteTurn)
        {
            GameBoard = gameBoard;
            WhiteTurn = whiteTurn;
        }

        //Score of this specific node
        private int? evaluation = null;

        //Matrix for computation
        private static int[,] SQUARE_SCORE = {
            {100, -50,  8,  6,  6,  6,  8, -50,  100},
            {-50,-100, -4, -4, -4, -4, -4,-100,  -50},
            {  8,  -4,  6,  4,  4,  4,  6,  -4 ,   8},
            {  6,  -4,  4,  0,  0,  4,  4,  -4 ,   6},
            {  8,  -4,  6,  0,  0,  4,  6,  -4 ,   8},
            {-50,-100, -4, -4, -4, -4, -4, -100, -50},
            {100, -50,  8,  6,  6,  6,  8,  -50, 100}
        };

        private static int EARLY_GAME = Settings.SIZE_WIDTH* Settings.SIZE_HEIGHT / 3;
        private static int MID_GAME = Settings.SIZE_WIDTH* Settings.SIZE_HEIGHT * 2 / 3;

        /// <summary>
        /// Eval the locations of the pawns
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private int Placement(int player)
        {
            int opponent = (player == Settings.WHITE) ? Settings.BLACK : Settings.WHITE;

            int myScore = 0;
            int opponentScore = 0;

            for (int i = 0; i < Settings.SIZE_WIDTH; i++)
            {
                for (int j = 0; j < Settings.SIZE_HEIGHT; j++)
                {
                    if (GameBoard.Board[i, j] == player) myScore += SQUARE_SCORE[j, i];
                    if (GameBoard.Board[i, j] == opponent) opponentScore += SQUARE_SCORE[j, i];
                }
            }

            return myScore - opponentScore;
        }

        /// <summary>
        /// Calc the difference between the number of disc
        /// </summary>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        private int DiscDiff(bool isWhite)
        {
            return (GameBoard.WhiteScore - GameBoard.BlackScore) * (isWhite ? 1 : -1);
        }

        /// <summary>
        /// Difference between the stable discs
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private int DiscDiffStable(bool isWhite)
        {
            //TODO Difference between stable discs
            return (GameBoard.GetNbStableToken(Settings.WHITE) - GameBoard.GetNbStableToken(Settings.BLACK)) * (isWhite ? 1 : -1);
        }

        /// <summary>
        /// Calc the number of move available for the opponent
        /// </summary>
        /// <param name="isWhite">Player turn</param>
        /// <returns>Mobility of the player</returns>
        private int Mobility(bool isWhite)
        {
            //Return a positive number if we skip the other player turn
            return Children(WhiteTurn).Count * (isWhite == WhiteTurn ? 1 : -1);
        }

        /// <summary>
        /// Calc the number of corner in possession
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private int NbCorner(int player)
        {
            int opponent = (player == Settings.WHITE) ? Settings.BLACK : Settings.WHITE;

            int score = 0;
            foreach(Tuple<int,int> corner in Settings.CORNERS)
            {
                if(GameBoard.Board[corner.Item1, corner.Item2] == player)
                {
                    ++score;
                }
                else if(GameBoard.Board[corner.Item1, corner.Item2] == opponent)
                {
                    score -= 5;
                }
            }
            return score;
        }

        /// <summary>
        /// Eval the board
        /// </summary>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        public int Evaluate(bool isWhite)
        {
            if(evaluation == null)
            {
                int player = isWhite ? Settings.WHITE : Settings.BLACK;
                int nbPawns = Score(true) + Score(false);

                //Ended
                if (GameBoard.GameEnded)
                {
                    return 100000 * DiscDiff(isWhite);
                }

                //Evaluation juste one
                if (nbPawns < EARLY_GAME)
                {
                    evaluation = 2000 * NbCorner(player) + 200 * Placement(player) + 600 * Mobility(isWhite);
                }
                else if (nbPawns < MID_GAME)
                {
                    evaluation = 2000 * NbCorner(player) + 100 * DiscDiffStable(isWhite) + 250 * Mobility(isWhite);
                }
                else
                {
                    evaluation = 2000 * NbCorner(player) + 50 * DiscDiff(isWhite) + 100 * DiscDiffStable(isWhite) + 100 * Mobility(isWhite);
                }
            }
            return (int)evaluation;
        }

        //Return possibles moves
        public Dictionary<Tuple<int,int>, TreeNode> Children(bool whiteTurn)
        {
            if (whiteTurn && whiteMoves == null)
            {
                whiteMoves = new Dictionary<Tuple<int, int>, TreeNode>();
                foreach(Tuple<int,int> move in GameBoard.PossibleMoves(Settings.WHITE))
                {
                    GameState gameBoard = GameBoard.ApplyMove(move, Settings.WHITE);
                    whiteMoves.Add(move, new TreeNode(gameBoard, !gameBoard.BlackCanPlay()));
                }
            }
            else if (!whiteTurn && blackMoves == null)
            {
                blackMoves = new Dictionary<Tuple<int, int>, TreeNode>();
                foreach (Tuple<int, int> move in GameBoard.PossibleMoves(Settings.BLACK))
                {
                    GameState gameBoard = GameBoard.ApplyMove(move, Settings.BLACK);
                    blackMoves.Add(move, new TreeNode(gameBoard, gameBoard.WhiteCanPlay()));
                }
            }

            return whiteTurn ? whiteMoves : blackMoves;
        }
    }
}
