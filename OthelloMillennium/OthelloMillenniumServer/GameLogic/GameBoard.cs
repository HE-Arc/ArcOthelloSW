using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OthelloMillenniumServer
{
    public class GameBoard
    {
        #region Internal Classes
        public enum CellState
        {
            //Default State = 0 -> EMPTY // DON'T CHANGE THOSE VALUES
            EMPTY = 0,
            BLACK = 1,
            WHITE = 2
        }

        #endregion

        #region Attributs
        public CellState[,] Board { get; private set; }

        //Directions to explore
        private static readonly (int, int)[] directions =
        {
            (-1, -1), (0, -1), (1, -1),
            (-1,  0),          (1,  0),
            (-1,  1), (0,  1), (1,  1),
        };

        private Dictionary<CellState, int> cellStateCount;

        #endregion

        #region Properties
        public bool GameEnded { get; private set; }

        // Neccessary when we want to manage move back and a player can't move
        public CellState LastPlayer { get; }

        #endregion

        #region Static
        /// <summary>
        /// Create a basic 
        /// </summary>
        /// <returns></returns>
        public static GameBoard CreateStartState()
        {
            CellState[,] state = new CellState[Settings.SIZE_WIDTH, Settings.SIZE_HEIGHT];

            state[3, 3] = CellState.WHITE;
            state[3, 4] = CellState.BLACK;
            state[4, 4] = CellState.WHITE;
            state[4, 3] = CellState.BLACK;

            return new GameBoard(state);
        }

        /// <summary>
        /// Transform a game coord to an indices coord
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        private static (int, int) CoordToInt((char, int) coord)
        {
            (int, int) indices = (coord.Item1 - 65, coord.Item2);
            Debug.Assert(indices.Item1 >= 0 && indices.Item1 < Settings.SIZE_WIDTH);
            Debug.Assert(indices.Item2 >= 0 && indices.Item2 < Settings.SIZE_HEIGHT);
            return indices;
        }

        #endregion

        /// <summary>
        /// Constructor to create a new GameState
        /// </summary>
        /// <param name="cellState"></param>
        public GameBoard(CellState[,] cellState, CellState cellStatePlayer = CellState.EMPTY)
        {
            Board = cellState;
            cellStateCount = new Dictionary<CellState, int>();

            // Init scores
            cellStateCount.Add(CellState.BLACK, 0);
            cellStateCount.Add(CellState.WHITE, 0);
            cellStateCount.Add(CellState.EMPTY, 0);

            // Calculate scores, counting the empty allow to know how many cells are empty
            foreach (CellState cell in Board)
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
        public List<(int, int)> PossibleMoves(CellState cellStatePlayer)
        {
            List<(int, int)> validMoves = new List<(int, int)>();
            // Check if the user can make a move
            for (int i = 0; i < Settings.SIZE_WIDTH; ++i)
            {
                for (int j = 0; j < Settings.SIZE_WIDTH; ++j)
                {
                    if (ValidateMove((i, j), cellStatePlayer))
                    {
                        validMoves.Add((i, j));
                    }
                }
            }

            return validMoves;
        }

        /// <summary>
        /// Return true if a given player can play or not
        /// </summary>
        /// <returns></returns>
        public bool PlayerCanPlay(CellState cellStatePlayer)
        {
            // Check if the user has been eradicated
            if(cellStateCount[cellStatePlayer] == 0)
            {
                return false;
            }

            // Check if the user can make a move
            for (int i = 0; i < Settings.SIZE_WIDTH; ++i)
            {
                for (int j = 0; j < Settings.SIZE_WIDTH; ++j)
                {
                    if(ValidateMove((i, j), cellStatePlayer))
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
        public GameBoard ApplyMove((char, int) coord, CellState cellStatePlayer)
        {
            (int, int) indices = CoordToInt(coord);

            CellState[,] newCellState = (CellState[,])Board.Clone();
            newCellState[indices.Item1, indices.Item2] = cellStatePlayer;

            List<(int, int)> tokenToReturn = new List<(int, int)>();
            List<(int, int)> tokenToReturnPotential = new List<(int, int)>();

            // Check in all 8 directions that there is at least one cellState of our own
            foreach ((int, int) direction in directions)
            {
                tokenToReturnPotential.Clear();
                bool end = false;
                (int i, int j) = indices;
                while (i > 0 && i < Settings.SIZE_WIDTH && j > 0 && j < Settings.SIZE_HEIGHT && !end)
                {
                    // Compute cell
                    (i, j) = (i + direction.Item1, j + direction.Item2);
                    CellState cellState = Board[i, j];

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
                        tokenToReturnPotential.Add((i, j));
                    }
                }
            }

            // Return the tokens
            foreach ((int i, int j) in tokenToReturn)
            {
                newCellState[i, j] = cellStatePlayer;
            }

            // Return the new state
            return new GameBoard(newCellState, cellStatePlayer);
        }

        /// <summary>
        /// Return the number of token of this sort
        /// </summary>
        /// <param name="cellStatePlayer"></param>
        /// <returns>Nb of cell conrtaining the given cellState</returns>
        public int getNbToken(CellState cellStatePlayer)
        {
            return cellStateCount[cellStatePlayer];
        }

        /// <summary>
        /// Validate a potential move
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="cellStatePlayer"></param>
        /// <returns></returns>
        public bool ValidateMove((char, int) coord, CellState cellStatePlayer)
        {
            return ValidateMove(CoordToInt(coord), cellStatePlayer);
        }

        /// <summary>
        /// Internal validation of a potential move
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="cellStatePlayer"></param>
        /// <returns></returns>
        private bool ValidateMove((int, int) indices, CellState cellStatePlayer)
        {
            //Check if celle at given coord is empty
            if (Board[indices.Item1, indices.Item2] != CellState.EMPTY)
            {
                return false;
            }

            //Check in all 8 directions that there is at least one cellState of our own
            int i = 0, j = 0;
            foreach ((int, int) direction in directions)
            {
                bool end = false;
                int nbTokenReturnedTemp = 0;
                (i, j) = indices;
                while (i > 0 && i < Settings.SIZE_WIDTH && j > 0 && j < Settings.SIZE_HEIGHT && !end)
                {
                    (i, j) = (i + direction.Item1, j + direction.Item2);
                    CellState cellState = Board[i, j];
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
            GameEnded = cellStateCount[CellState.EMPTY] == 0 || !PlayerCanPlay(CellState.WHITE) || !PlayerCanPlay(CellState.BLACK);
        }
    }
}
