using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OthelloMillenniumServer
{
    class GameState
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
        private CellState[,] gameboard;

        //Directions to explore
        private static readonly (int, int)[] directions =
        {
            (-1, -1), (0, -1), (1, -1),
            (-1,  0),          (1,  0),
            (-1,  1), (0,  1), (1,  1),
        };

        Dictionary<CellState, int> scores;

        #endregion

        #region Static
        /// <summary>
        /// Create a basic 
        /// </summary>
        /// <returns></returns>
        public static GameState CreateStartState()
        {
            CellState[,] state = new CellState[Settings.SIZE_WIDTH, Settings.SIZE_HEIGHT];

            //TODO: Potential to improve this part
            state[3, 3] = CellState.WHITE;
            state[3, 4] = CellState.BLACK;
            state[4, 4] = CellState.WHITE;
            state[4, 3] = CellState.BLACK;

            return new GameState(state);
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
        public GameState(CellState[,] cellState)
        {
            gameboard = cellState;
            scores = new Dictionary<CellState, int>();

            // Init scores
            scores.Add(CellState.BLACK, 0);
            scores.Add(CellState.WHITE, 0);
            scores.Add(CellState.EMPTY, 0);

            // Calculate scores, counting the empty allow to know how many cells are empty
            foreach (CellState cell in gameboard)
            {
                scores[cell]++;
            }

            // TODO update score for special casses
        }

        /// <summary>
        /// Return true if a given player can play or not
        /// </summary>
        /// <returns></returns>
        public bool PlayerCanPlay(CellState cellStatePlayer)
        {
            // Check if the user has been eradicated
            if(scores[cellStatePlayer] == 0)
            {
                return false;
            }

            // Check if the user can make a move
            for (int i = 0; i < Settings.SIZE_WIDTH; ++i)
            {
                for (int j = 0; j < Settings.SIZE_WIDTH; ++j)
                {
                    if(ValidMove((i, j), cellStatePlayer))
                    {
                        return true;
                    }
                }
            }
            
            // No move has been found
            return false;
        }

        /// <summary>
        /// Check if the game is finished
        /// </summary>
        /// <returns></returns>
        public bool IsGameEnded()
        {
            // Manage all casses, when all token have been played
            // when a player as been eradicated or even when no one can move
            return scores[CellState.WHITE] == 0 || !PlayerCanPlay(CellState.WHITE) || !PlayerCanPlay(CellState.BLACK);
        }

        /// <summary>
        /// Validate a potential move
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="cellStatePlayer"></param>
        /// <returns></returns>
        public bool ValidMove((char, int) coord, CellState cellStatePlayer)
        {
            return ValidMove(CoordToInt(coord), cellStatePlayer);
        }

        /// <summary>
        /// Internal validation of a potential move
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="cellStatePlayer"></param>
        /// <returns></returns>
        private bool ValidMove((int, int) indices, CellState cellStatePlayer)
        {
            //Check if celle at given coord is empty
            if (gameboard[indices.Item1, indices.Item2] != CellState.EMPTY)
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
                    CellState cellState = gameboard[i, j];
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
        /// Apply a move and return the new GameState
        /// </summary>
        /// <param name="point"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public GameState ApplyMove((char, int) coord, CellState cellStatePlayer)
        {
            (int, int) indices = CoordToInt(coord);

            CellState[,] newCellState = (CellState[,])gameboard.Clone();
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
                    CellState cellState = gameboard[i, j];

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
            foreach((int i, int j) in tokenToReturn)
            {
                newCellState[i, j] = cellStatePlayer;
            }

            // Return the new state
            return new GameState(newCellState);
        }
    }
}
