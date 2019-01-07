namespace Tools.Classes
{
    public interface IOrder
    {
        string GetAcronym();
        string GetDefinition();
        int GetLength();
    }

    /// <summary>
    /// Flyweight orders provider
    /// </summary>
    public static class OrderProvider
    {
        public readonly static IOrder SearchLocalGame = new SearchLocalGameOrder();
        public readonly static IOrder SearchOnlineGame = new SearchOnlineGameOrder();
        public readonly static IOrder RegisterSuccessful = new RegisterSuccessfulOrder();
        public readonly static IOrder OpponentFound = new OpponentFoundOrder();
        public readonly static IOrder StartOfTheGame = new StartOfTheGameOrder();
        public readonly static IOrder EndOfTheGame = new EndOfTheGameOrder();
        public readonly static IOrder BlackAssigned = new BlackAssignedOrder();
        public readonly static IOrder WhiteAssigned = new WhiteAssignedOrder();
        public readonly static IOrder PlayerBegin = new PlayerBeginOrder();
        public readonly static IOrder PlayerAwait = new PlayerAwaitOrder();
        public readonly static IOrder PlayerState = new PlayerStateOrder();
        public readonly static IOrder OpponentDisconnected = new OpponentDisconnectedOrder();
        public readonly static IOrder OpponentConnectionLost = new OpponentConnectionLostOrder();
        public readonly static IOrder NextTurn = new NextTurnOrder();
        public readonly static IOrder GetCurrentGameState = new GetCurrentGameStateOrder();
        public readonly static IOrder GetPreviousGameState = new GetPreviousGameStateOrder();
    }

    public class SearchLocalGameOrder : IOrder
    {
        public string GetAcronym()
        {
            return "SL";
        }

        public string GetDefinition()
        {
            return "Search a local game for the current player";
        }

        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        internal SearchLocalGameOrder() { }
    }

    public class SearchOnlineGameOrder : IOrder
    {
        public string GetAcronym()
        {
            return "SO";
        }

        public string GetDefinition()
        {
            return "Search an online game for the current player";
        }

        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        internal SearchOnlineGameOrder() { }
    }

    public class RegisterSuccessfulOrder : IOrder
    {
        public string GetAcronym()
        {
            return "RS";
        }

        public string GetDefinition()
        {
            return "Inform current player that he has been registred successfully";
        }

        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        internal RegisterSuccessfulOrder() { }
    }

    public class OpponentFoundOrder : IOrder
    {
        public string GetAcronym()
        {
            return "OF";
        }

        public string GetDefinition()
        {
            return "Inform current player that an opponent has been found";
        }

        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        internal OpponentFoundOrder() { }
    }

    public class StartOfTheGameOrder : IOrder
    {
        public string GetAcronym()
        {
            return "SG";
        }

        public string GetDefinition()
        {
            return "Inform current player that the game has started";
        }

        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        internal StartOfTheGameOrder() { }
    }

    public class EndOfTheGameOrder : IOrder
    {
        public string GetAcronym()
        {
            return "EG";
        }

        public string GetDefinition()
        {
            return "Inform current player that the has ended";
        }

        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        internal EndOfTheGameOrder() { }
    }

    public class BlackAssignedOrder : IOrder
    {
        public string GetAcronym()
        {
            return "BA";
        }

        public string GetDefinition()
        {
            return "Inform current player that the color has been assigned to him";
        }

        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        internal BlackAssignedOrder() { }
    }

    public class WhiteAssignedOrder : IOrder
    {
        public string GetAcronym()
        {
            return "WA";
        }

        public string GetDefinition()
        {
            return "Inform current player that the color white has been assigned to him";
        }

        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        internal WhiteAssignedOrder() { }
    }

    public class PlayerBeginOrder : IOrder
    {
        public string GetAcronym()
        {
            return "PB";
        }

        public string GetDefinition()
        {
            return "Inform current player that he begins";
        }

        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        internal PlayerBeginOrder() { }
    }

    public class PlayerAwaitOrder : IOrder
    {
        public string GetAcronym()
        {
            return "PA";
        }

        public string GetDefinition()
        {
            return "Inform current player that he has to await his turn";
        }

        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        internal PlayerAwaitOrder() { }
    }

    public class PlayerStateOrder : IOrder
    {
        public string GetAcronym()
        {
            return "PS";
        }

        public string GetDefinition()
        {
            return "Ask for the current player state";
        }

        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        internal PlayerStateOrder() { }
    }

    public class OpponentDisconnectedOrder : IOrder
    {
        public string GetAcronym()
        {
            return "OD";
        }

        public string GetDefinition()
        {
            return "Inform current player that the opponent has disconnected";
        }

        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        internal OpponentDisconnectedOrder() { }
    }

    public class OpponentConnectionLostOrder : IOrder
    {
        public string GetAcronym()
        {
            return "OL";
        }

        public string GetDefinition()
        {
            return "Inform current player that the opponent has lost connection with the server";
        }

        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        internal OpponentConnectionLostOrder() { }
    }

    public class NextTurnOrder : IOrder
    {
        public string GetAcronym()
        {
            return "NT";
        }

        public string GetDefinition()
        {
            return "Current player either finish or pass his turn";
        }

        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        internal NextTurnOrder() { }
    }

    public class GetCurrentGameStateOrder : IOrder
    {
        public string GetAcronym()
        {
            return "GCGS"; // ExampleOrder
        }

        public string GetDefinition()
        {
            return "Ask the server for the current gameState";
        }

        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        internal GetCurrentGameStateOrder() { }
    }

    public class GetPreviousGameStateOrder : IOrder
    {
        public string GetAcronym()
        {
            return "GPGS"; // ExampleOrder
        }

        public string GetDefinition()
        {
            return "Ask the server for the previous gameState";
        }

        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        internal GetPreviousGameStateOrder() { }
    }

    #region Example
    /*
    public class Order : IOrder
    {
        public string GetAcronym()
        {
            return "EO"; // ExampleOrder
        }

        public string GetDefinition()
        {
            return "Example";
        }

        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        internal Order() { }
    }
    */
    #endregion
}