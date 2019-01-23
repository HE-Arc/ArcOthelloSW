using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Tools;

namespace OthelloMillenniumServer
{
    public class GameBoard
    {
        #region Internal Classes
        internal static class CellState
        {
            //Default State = 0 -> EMPTY // DON'T CHANGE THOSE VALUES
            public const int EMPTY = 0;
            public const int BLACK = 1;
            public const int WHITE = 2;
        }

        #endregion

        #region Attributs

        //Directions to explore
        private static readonly Tuple<int, int>[] directions =
        {
            new Tuple<int, int>(-1, -1), new Tuple<int, int>(0, -1), new Tuple<int, int>(1, -1),
            new Tuple<int, int>(-1,  0),                             new Tuple<int, int>(1,  0),
            new Tuple<int, int>(-1,  1), new Tuple<int, int>(0,  1), new Tuple<int, int>(1,  1),
        };

        private readonly Dictionary<int, int> cellStateCount;

        #endregion

        #region Properties
        public int[,] Board { get; private set; }

        public bool GameEnded { get; private set; }

        // Necessary when we want to manage move back and a player can't move
        public int LastPlayer { get; private set; }

        #endregion

        #region Static
        /// <summary>
        /// Create a basic
        /// </summary>
        /// <returns></returns>
        public static GameBoard CreateStartState()
        {
            int[,] state = new int[Settings.SIZE_WIDTH, Settings.SIZE_HEIGHT];

            state[3, 3] = CellState.WHITE;
            state[3, 4] = CellState.BLACK;
            state[4, 4] = CellState.WHITE;
            state[4, 3] = CellState.BLACK;

            return new GameBoard(state, CellState.WHITE);
        }

        /// <summary>
        /// Transform a game coord to an indices coord
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        private static Tuple<int, int> CoordToInt(Tuple<char, int> coord)
        {
            Tuple<int, int> indices = new Tuple<int, int>(coord.Item1 - 65, coord.Item2 - 1);
            Debug.Assert(indices.Item1 >= 0 && indices.Item1 < Settings.SIZE_WIDTH);
            Debug.Assert(indices.Item2 >= 0 && indices.Item2 < Settings.SIZE_HEIGHT);
            return indices;
        }

        /// <summary>
        /// Transform a game coord to an indices coord
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        private static Tuple<char, int> IntToCoord(Tuple<int, int> coord)
        {
            Debug.Assert(coord.Item1 >= 0 && coord.Item1 < Settings.SIZE_WIDTH);
            Debug.Assert(coord.Item2 >= 0 && coord.Item2 < Settings.SIZE_HEIGHT);
            Tuple<char, int> indices = new Tuple<char, int>((char)(coord.Item1 + 65), coord.Item2 + 1);
            return indices;
        }

        #endregion

        /// <summary>
        /// Constructor to create a new GameState
        /// </summary>
        /// <param name="cellState"></param>
        public GameBoard(int[,] cellState, int lastPlayer = CellState.WHITE)
        {
            Board = cellState;
            LastPlayer = lastPlayer;

            cellStateCount = new Dictionary<int, int>
            {
                // Init scores
                { CellState.BLACK, 0 },
                { CellState.WHITE, 0 },
                { CellState.EMPTY, 0 }
            };

            // Calculate scores, counting the empty allow to know how many cells are empty
            foreach (int cell in Board)
            {
                cellStateCount[cell]++;
            }

            ComputeGameEnded();
        }

        /// <summary>
        /// Compute the valid move for one player
        /// </summary>
        /// <param name="cellStatePlayer"></param>
        /// <returns></returns>
        public List<Tuple<char, int>> PossibleMoves(int cellStatePlayer)
        {
            List<Tuple<char, int>> validMoves = new List<Tuple<char, int>>();
            // Check if the user can make a move
            for (int i = 0; i < Settings.SIZE_WIDTH; ++i)
            {
                for (int j = 0; j < Settings.SIZE_HEIGHT; ++j)
                {
                    if (ValidateMove(new Tuple<int, int>(i, j), cellStatePlayer))
                    {
                        validMoves.Add(IntToCoord(new Tuple<int, int>(i, j)));
                    }
                }
            }

            return validMoves;
        }

        /// <summary>
        /// Return true if a given player can play or not
        /// </summary>
        /// <returns></returns>
        public bool PlayerCanPlay(int cellStatePlayer)
        {
            // Check if the user has been eradicated
            if(cellStateCount[cellStatePlayer] == 0)
            {
                return false;
            }

            // Check if the user can make a move
            for (int i = 0; i < Settings.SIZE_WIDTH; ++i)
            {
                for (int j = 0; j < Settings.SIZE_HEIGHT; ++j)
                {
                    if(ValidateMove(new Tuple<int, int>(i, j), cellStatePlayer))
                    {
                        return true;
                    }
                }
            }

            // No move has been found
            return false;
        }

        /// <summary>
        /// Apply a move and return the new GameState
        /// </summary>
        /// <param name="point"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public GameBoard ApplyMove(Tuple<char, int> coord, int cellStatePlayer)
        {
            Tuple<int, int> indices = CoordToInt(coord);

            List<Tuple<int, int>> tokenToReturn = new List<Tuple<int, int>>();
            List<Tuple<int, int>> tokenToReturnPotential = new List<Tuple<int, int>>();

            // Check in all 8 directions that there is at least one cellState of our own
            foreach (Tuple<int, int> direction in directions)
            {
                tokenToReturnPotential.Clear();
                bool end = false;
                Tuple<int, int> ij = indices;
                ij = new Tuple<int, int>(ij.Item1 + direction.Item1, ij.Item2 + direction.Item2);

                while (ij.Item1 >= 0 && ij.Item1 < Settings.SIZE_WIDTH && ij.Item2 >= 0 && ij.Item2 < Settings.SIZE_HEIGHT && !end)
                {
                    // Compute cell
                    int cellState = Board[ij.Item1, ij.Item2];

                    if (cellState == cellStatePlayer) // Token of the player
                    {
                        end = true;
                        tokenToReturn.AddRange(tokenToReturnPotential);
                    }
                    else if (cellState == CellState.EMPTY) // No token
                    {
                        end = true;
                    }
                    else // Token of the opponent
                    {
                        tokenToReturnPotential.Add(ij);
                    }
                    ij = new Tuple<int, int>(ij.Item1 + direction.Item1, ij.Item2 + direction.Item2);
                }
            }

            //Create new CellState
            int[,] newCellState = (int[,])Board.Clone();
            tokenToReturn.Add(indices);

            // Return the tokens
            foreach (Tuple<int, int> ij in tokenToReturn)
            {
                newCellState[ij.Item1, ij.Item2] = cellStatePlayer;
            }

            // Return the new state
            return new GameBoard(newCellState, cellStatePlayer);
        }

        /// <summary>
        /// Return the number of token of this sort
        /// </summary>
        /// <param name="cellStatePlayer"></param>
        /// <returns>Nb of cell conrtaining the given cellState</returns>
        public int GetNbToken(int cellStatePlayer)
        {
            return cellStateCount[cellStatePlayer];
        }

        /// <summary>
        /// Validate a potential move
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="cellStatePlayer"></param>
        /// <returns></returns>
        public bool ValidateMove(Tuple<char, int> coord, int cellStatePlayer)
        {
            return ValidateMove(CoordToInt(coord), cellStatePlayer);
        }

        /// <summary>
        /// Internal validation of a potential move
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="cellStatePlayer"></param>
        /// <returns></returns>
        private bool ValidateMove(Tuple<int, int> indices, int cellStatePlayer)
        {
            // Check if cell at given coord is empty
            if (Board[indices.Item1, indices.Item2] != CellState.EMPTY)
            {
                return false;
            }

            // Check in all 8 directions that there is at least one cellState of our own
            foreach (Tuple<int, int> direction in directions)
            {
                bool end = false;
                int nbTokenReturnedTemp = 0;

                Tuple<int, int> ij = indices;
                ij = new Tuple<int, int>(ij.Item1 + direction.Item1, ij.Item2 + direction.Item2);

                while (ij.Item1 >= 0 && ij.Item1 < Settings.SIZE_WIDTH && ij.Item2 >= 0 && ij.Item2 < Settings.SIZE_HEIGHT && !end)
                {
                    int cellState = Board[ij.Item1, ij.Item2];
                    if (cellState == cellStatePlayer)
                    {
                        end = true;
                        if (nbTokenReturnedTemp > 0)
                            return true;
                    }
                    else if (cellState == CellState.EMPTY)
                    {
                        end = true;
                    }
                    else
                    {
                        nbTokenReturnedTemp++;
                    }
                    ij = new Tuple<int, int>(ij.Item1 + direction.Item1, ij.Item2 + direction.Item2);
                }
            }
            return false;
        }

        /// <summary>
        /// Check if the game is finished
        /// </summary>
        /// <returns></returns>
        private void ComputeGameEnded()
        {
            // Manage all casses, when all token have been played
            // when a player as been eradicated or even when no one can move
            GameEnded = cellStateCount[CellState.EMPTY] == 0 || !PlayerCanPlay(CellState.WHITE) && !PlayerCanPlay(CellState.BLACK);
        }
    }
}
