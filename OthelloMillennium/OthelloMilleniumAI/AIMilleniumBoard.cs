using System;

namespace IAOthelloMillenium
{
    //Facade in the aim that the user new instanciate multiple class of AI!
    //TODO Maybe remove
    public class AIMilleniumBoard : IPlayable.IPlayable
    {
        public AIMilleniumBoard()
        {
            //Nothing
        }

        public int GetBlackScore() => AI.Instance.GetBlackScore();

        public int[,] GetBoard() => AI.Instance.GetBoard();

        public string GetName() => AI.Instance.GetName();

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
            => AI.Instance.GetNextMove(game, level, whiteTurn);

        public int GetWhiteScore() => AI.Instance.GetWhiteScore();

        public bool IsPlayable(int column, int line, bool isWhite) => AI.Instance.IsPlayable(column, line, isWhite);

        public bool PlayMove(int column, int line, bool isWhite) => AI.Instance.PlayMove(column, line, isWhite);
    }
}
