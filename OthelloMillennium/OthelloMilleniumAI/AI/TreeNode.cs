using System;
using System.Collections.Generic;

namespace IAOthelloMillenium
{
    class TreeNode
    {
        //TODO choice keep move at each level or board

        public GamePlate GameBoard { get; private set; }
        public bool IsTerminal => GameBoard.GameEnded;
        public int Score(bool isWhite) => isWhite ? GameBoard.WhiteScore: GameBoard.BlackScore;

        public bool WhiteTurn { get; private set; }

        private Dictionary<Tuple<int,int>, TreeNode> whiteMoves;
        private Dictionary<Tuple<int, int>, TreeNode> blackMoves;

        public TreeNode(GamePlate gameBoard, bool whiteTurn)
        {
            GameBoard = gameBoard;
            WhiteTurn = whiteTurn;
        }

        //Return possibles moves
        public Dictionary<Tuple<int,int>, TreeNode> Children(bool whiteTurn)
        {
            if (whiteTurn && whiteMoves == null)
            {
                whiteMoves = new Dictionary<Tuple<int, int>, TreeNode>();
                foreach(Tuple<int,int> move in GameBoard.PossibleMoves(Settings.WHITE))
                {
                    whiteMoves.Add(move, new TreeNode(GameBoard.ApplyMove(move, Settings.WHITE), whiteTurn));
                }
            }
            else if (!whiteTurn && blackMoves == null)
            {
                blackMoves = new Dictionary<Tuple<int, int>, TreeNode>();
                foreach (Tuple<int, int> move in GameBoard.PossibleMoves(Settings.BLACK))
                {
                    blackMoves.Add(move, new TreeNode(GameBoard.ApplyMove(move, Settings.BLACK), whiteTurn));
                }
            }

            return whiteTurn ? whiteMoves : blackMoves;
        }
    }
}
