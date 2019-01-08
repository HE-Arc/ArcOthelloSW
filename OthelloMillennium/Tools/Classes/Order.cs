using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tools.Classes
{
    [Serializable]
    public abstract class AOrder : ISerializable
    {
        public abstract string GetAcronym();
        public abstract string GetDefinition();
        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Nothing to store
        }
    }

    /// <summary>
    /// Flyweight orders provider
    /// </summary>
    public static class OrderProvider
    {
        public readonly static AOrder SearchLocalGame = new SearchLocalGameOrder();
        public readonly static AOrder SearchOnlineGame = new SearchOnlineGameOrder();
        public readonly static AOrder RegisterSuccessful = new RegisterSuccessfulOrder();
        public readonly static AOrder OpponentFound = new OpponentFoundOrder();
        public readonly static AOrder StartOfTheGame = new StartOfTheGameOrder();
        public readonly static AOrder EndOfTheGame = new EndOfTheGameOrder();
        public readonly static AOrder BlackAssigned = new BlackAssignedOrder();
        public readonly static AOrder WhiteAssigned = new WhiteAssignedOrder();
        public readonly static AOrder PlayerBegin = new PlayerBeginOrder();
        public readonly static AOrder PlayerAwait = new PlayerAwaitOrder();
        public readonly static AOrder PlayerState = new PlayerStateOrder();
        public readonly static AOrder OpponentDisconnected = new OpponentDisconnectedOrder();
        public readonly static AOrder OpponentConnectionLost = new OpponentConnectionLostOrder();
        public readonly static AOrder NextTurn = new NextTurnOrder();
        public readonly static AOrder GetCurrentGameState = new GetCurrentGameStateOrder();
        public readonly static AOrder GetPreviousGameState = new GetPreviousGameStateOrder();
        public readonly static AOrder PlayMove = new PlayMoveOrder();
    }

    public class SearchLocalGameOrder : AOrder
    {
        public override string GetAcronym()
        {
            return "SL";
        }

        public override string GetDefinition()
        {
            return "Search a local game for the current player";
        }

        internal SearchLocalGameOrder() { }
    }

    public class SearchOnlineGameOrder : AOrder
    {
        public override string GetAcronym()
        {
            return "SO";
        }

        public override string GetDefinition()
        {
            return "Search an online game for the current player";
        }

        internal SearchOnlineGameOrder() { }
    }

    public class RegisterSuccessfulOrder : AOrder
    {
        public override string GetAcronym()
        {
            return "RS";
        }

        public override string GetDefinition()
        {
            return "Inform current player that he has been registred successfully";
        }

        internal RegisterSuccessfulOrder() { }
    }

    public class OpponentFoundOrder : AOrder
    {
        public override string GetAcronym()
        {
            return "OF";
        }

        public override string GetDefinition()
        {
            return "Inform current player that an opponent has been found";
        }

        internal OpponentFoundOrder() { }
    }

    public class StartOfTheGameOrder : AOrder
    {
        public override string GetAcronym()
        {
            return "SG";
        }

        public override string GetDefinition()
        {
            return "Inform current player that the game has started";
        }

        internal StartOfTheGameOrder() { }
    }

    public class EndOfTheGameOrder : AOrder
    {
        public override string GetAcronym()
        {
            return "EG";
        }

        public override string GetDefinition()
        {
            return "Inform current player that the has ended";
        }

        internal EndOfTheGameOrder() { }
    }

    public class BlackAssignedOrder : AOrder
    {
        public override string GetAcronym()
        {
            return "BA";
        }

        public override string GetDefinition()
        {
            return "Inform current player that the color has been assigned to him";
        }

        internal BlackAssignedOrder() { }
    }

    public class WhiteAssignedOrder : AOrder
    {
        public override string GetAcronym()
        {
            return "WA";
        }

        public override string GetDefinition()
        {
            return "Inform current player that the color white has been assigned to him";
        }

        internal WhiteAssignedOrder() { }
    }

    public class PlayerBeginOrder : AOrder
    {
        public override string GetAcronym()
        {
            return "PB";
        }

        public override string GetDefinition()
        {
            return "Inform current player that he begins";
        }

        internal PlayerBeginOrder() { }
    }

    public class PlayerAwaitOrder : AOrder
    {
        public override string GetAcronym()
        {
            return "PA";
        }

        public override string GetDefinition()
        {
            return "Inform current player that he has to await his turn";
        }

        internal PlayerAwaitOrder() { }
    }

    public class PlayerStateOrder : AOrder
    {
        public override string GetAcronym()
        {
            return "PS";
        }

        public override string GetDefinition()
        {
            return "Ask for the current player state";
        }

        internal PlayerStateOrder() { }
    }

    public class OpponentDisconnectedOrder : AOrder
    {
        public override string GetAcronym()
        {
            return "OD";
        }

        public override string GetDefinition()
        {
            return "Inform current player that the opponent has disconnected";
        }

        internal OpponentDisconnectedOrder() { }
    }

    public class OpponentConnectionLostOrder : AOrder
    {
        public override string GetAcronym()
        {
            return "OL";
        }

        public override string GetDefinition()
        {
            return "Inform current player that the opponent has lost connection with the server";
        }

        internal OpponentConnectionLostOrder() { }
    }

    public class NextTurnOrder : AOrder
    {
        public override string GetAcronym()
        {
            return "NT";
        }

        public override string GetDefinition()
        {
            return "Current player either finish or pass his turn";
        }

        internal NextTurnOrder() { }
    }

    public class GetCurrentGameStateOrder : AOrder
    {
        public override string GetAcronym()
        {
            return "GCGS"; // ExampleOrder
        }

        public override string GetDefinition()
        {
            return "Ask the server for the current gameState";
        }

        internal GetCurrentGameStateOrder() { }
    }

    public class GetPreviousGameStateOrder : AOrder
    {
        public override string GetAcronym()
        {
            return "GPGS"; // ExampleOrder
        }

        public override string GetDefinition()
        {
            return "Ask the server for the previous gameState";
        }

        internal GetPreviousGameStateOrder() { }
    }

    public class PlayMoveOrder : AOrder
    {
        public (char, int) Coords { get; set; }

        protected PlayMoveOrder(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            Coords = ((char, int))info.GetValue("Coords", typeof((char, int)));
        }

        public override string GetAcronym()
        {
            return "PM"; // ExampleOrder
        }

        public override string GetDefinition()
        {
            return "Contains coords where the player want to place an othello's game piece";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Coords", Coords);
        }

        internal PlayMoveOrder() { }
    }

    #region Example
    /*
    public class Order : IOrder
    {
        public override string GetAcronym()
        {
            return "EO"; // ExampleOrder
        }

        public override string GetDefinition()
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