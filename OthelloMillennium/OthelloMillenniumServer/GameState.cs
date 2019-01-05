using System;
using System.Collections.Generic;
using System.Text;

namespace OthelloMillenniumServer
{
    class GameState
    {
        public CellState[,] gameboard;

        /// <summary>
        /// Constructor to create a new GameState
        /// </summary>
        /// <param name="cellState"></param>
        public GameState(CellState[,] cellState)
        {
            gameboard = cellState;
        }

        public bool ValidMove()
        {
            return false;
        }

        /// <summary>
        /// Apply a move and return the new GameState
        /// </summary>
        /// <param name="point"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public GameState ApplyMove(Tuple<int, int> point, CellState state)
        {
            CellState[,] newCellState = (CellState[,])gameboard.Clone();
            newCellState[point.Item1, point.Item2] = state;
            return new GameState(newCellState);
        }
        
        /// <summary>
        /// Create a basic 
        /// </summary>
        /// <returns></returns>
        public static GameState CreateStartState()
        {
            CellState[,] state = new CellState[Settings.SIZE_HEIGHT, Settings.SIZE_WIDTH];

            state[3, 3] = CellState.WHITE;
            state[3, 4] = CellState.BLACK;
            state[4, 4] = CellState.WHITE;
            state[4, 3] = CellState.BLACK;

            return new GameState(state);
        }
    }

}
