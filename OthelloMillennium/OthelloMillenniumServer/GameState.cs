using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OthelloMillenniumServer
{
    class GameState
    {
        public CellState[,] gameboard;
        private static readonly (int, int)[] directions =
        {
            (-1, -1),
            (-1, 0),
            (-1, 1),
            (0, -1),
            (0, 1),
            (1, -1),
            (1, 0),
            (1, 1),
        };

        /// <summary>
        /// Constructor to create a new GameState
        /// </summary>
        /// <param name="cellState"></param>
        public GameState(CellState[,] cellState)
        {
            gameboard = cellState;
        }

        /// <summary>
        /// Validate a potential move
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool ValidMove((char, int) coord, CellState cellStatePlayer)
        {
            (int, int) indices = CoordToInt(coord);

            //Check if celle at given coord is empty
            if (gameboard[indices.Item1, indices.Item2] != CellState.EMPTY)
            {
                return false;
            }

            //Check in all 8 directions that there is at least one cellState of our own
            int i = 0, j = 0;
            foreach((int, int)direction in directions)
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
                    else if(cellState == CellState.EMPTY)
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
            List<(int, int)> tokenToReturnTemp = new List<(int, int)>();

            //Check in all 8 directions that there is at least one cellState of our own
            foreach ((int, int) direction in directions)
            {
                tokenToReturnTemp.Clear();
                bool end = false;
                (int i, int j) = indices;
                while (i > 0 && i < Settings.SIZE_WIDTH && j > 0 && j < Settings.SIZE_HEIGHT && !end)
                {
                    (i, j) = (i + direction.Item1, j + direction.Item2);
                    CellState cellState = gameboard[i, j];
                    if (cellState == cellStatePlayer)
                    {
                        end = true;
                        tokenToReturn.AddRange(tokenToReturnTemp);
                    }
                    else if (cellState == CellState.EMPTY)
                    {
                        end = true;
                    }
                    else
                    {
                        tokenToReturnTemp.Add((i, j));
                    }
                }
            }

            foreach((int i, int j) in tokenToReturn)
            {
                newCellState[i, j] = cellStatePlayer;
            }

            //Return 
            return new GameState(newCellState);
        }
        
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
            (int,int) indices = (coord.Item1-65, coord.Item2);
            Debug.Assert(indices.Item1 >= 0 && indices.Item1 < Settings.SIZE_WIDTH);
            Debug.Assert(indices.Item2 >= 0 && indices.Item2 < Settings.SIZE_HEIGHT);
            return indices;
        }
    }
}
