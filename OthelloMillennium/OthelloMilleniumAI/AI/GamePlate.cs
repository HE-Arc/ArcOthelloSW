using System;
using System.Collections.Generic;

namespace IAOthelloMillenium
{
    internal class GamePlate
    {
        //EMPTY = 0;
        //BLACK = 1;
        //WHITE = 2;

        #region Attributs
        //Directions to explore
        private static readonly Tuple<int, int>[] directions =
        {
            new Tuple<int, int>(-1, -1), new Tuple<int, int>(0, -1), new Tuple<int, int>(1, -1),
            new Tuple<int, int>(-1,  0),                             new Tuple<int, int>(1,  0),
            new Tuple<int, int>(-1,  1), new Tuple<int, int>(0,  1), new Tuple<int, int>(1,  1),
        };

        private readonly int[] cellStateCount;

        #endregion

        #region Properties
        public int[,] Board { get; private set; }

        public bool GameEnded { get; private set; }

        private bool? whiteCanPlay = null;
        private bool? blackCanPlay = null;
        private int? player;
        private List<Tuple<int, int>> validMoves = null;

        public bool WhiteCanPlay()
        {
            if (whiteCanPlay == null)
            {
                whiteCanPlay = PlayerCanPlay(Settings.WHITE);
            }
            return (bool)whiteCanPlay;
        }

        public bool BlackCanPlay()
        {
            if (blackCanPlay == null)
            {
                blackCanPlay = PlayerCanPlay(Settings.BLACK);
            }
            return (bool)blackCanPlay;
        }

        public int BlackScore { get; private set; }
        public int WhiteScore { get; private set; }

        #endregion

        #region static
        /// <summary>
        /// Create a basic
        /// </summary>
        /// <returns></returns>
        public static GamePlate CreateStartState()
        {
            int[,] state = new int[Settings.SIZE_WIDTH, Settings.SIZE_HEIGHT];
            for(int i=0;i<Settings.SIZE_WIDTH; ++i)
            {
                for (int j = 0; j < Settings.SIZE_HEIGHT; ++j)
                {
                    state[i, j] = Settings.EMPTY;
                }
            }

            state[3, 3] = Settings.WHITE;
            state[3, 4] = Settings.BLACK;
            state[4, 4] = Settings.WHITE;
            state[4, 3] = Settings.BLACK;

            return new GamePlate(state);
        }

        #endregion

        /// <summary>
        /// Constructor to create a new GameState
        /// </summary>
        /// <param name="board"></param>
        public GamePlate(int[,] board)
        {
            Board = board;

            cellStateCount = new int[]{ 0, 0, 0};

            // Calculate scores, counting the empty cells allow to know how many cells are empty
            foreach (int cell in Board)
            {
                cellStateCount[cell+1]++;
            }

            ComputeGameEnded();
        }

        /// <summary>
        /// Compute the valid move for one player
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public List<Tuple<int, int>> PossibleMoves(int player)
        {
            if (this.player != player || validMoves == null)
            {
                this.player = player;
                validMoves = new List<Tuple<int, int>>();
                // Check if the user can make a move
                for (int i = 0; i < Settings.SIZE_WIDTH; ++i)
                {
                    for (int j = 0; j < Settings.SIZE_HEIGHT; ++j)
                    {
                        if (ValidateMove(new Tuple<int, int>(i, j), player))
                        {
                            validMoves.Add(new Tuple<int, int>(i, j));
                        }
                    }
                }
            }

            return validMoves;
        }

        /// <summary>
        /// Return true if a given player can play or not
        /// </summary>
        /// <returns></returns>
        private bool PlayerCanPlay(int player)
        {
            // Check if the user has been eradicated
            if(cellStateCount[player+1] == 0)
            {
                return false;
            }

            // Check if the user can make a move
            for (int i = 0; i < Settings.SIZE_WIDTH; ++i)
            {
                for (int j = 0; j < Settings.SIZE_HEIGHT; ++j)
                {
                    if(ValidateMove(new Tuple<int, int>(i, j), player))
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
        public GamePlate ApplyMove(Tuple<int, int> indices, int player)
        {
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

                    if (cellState == player) // Token of the player
                    {
                        end = true;
                        tokenToReturn.AddRange(tokenToReturnPotential);
                    }
                    else if (cellState == Settings.EMPTY) // No token
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
                newCellState[ij.Item1, ij.Item2] = player;
            }

            // Return the new state
            return new GamePlate(newCellState);
        }

        /// <summary>
        /// Return the number of token of this sort
        /// </summary>
        /// <param name="player"></param>
        /// <returns>Nb of cell conrtaining the given cellState</returns>
        public int GetNbToken(int player)
        {
            return cellStateCount[player+1];
        }

        /// <summary>
        /// Internal validation of a potential move
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool ValidateMove(Tuple<int, int> indices, int player)
        {
            // Check if cell at given coord is empty
            if (Board[indices.Item1, indices.Item2] != Settings.EMPTY)
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
                    if (cellState == player)
                    {
                        end = true;
                        if (nbTokenReturnedTemp > 0)
                            return true;
                    }
                    else if (cellState == Settings.EMPTY)
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
            GameEnded = cellStateCount[Settings.EMPTY+1] == 0 || !WhiteCanPlay() && !BlackCanPlay();
        }
    }
}
