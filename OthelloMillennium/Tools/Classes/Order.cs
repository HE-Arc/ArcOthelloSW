using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tools.Classes
{
    [Serializable]
    public abstract class AOrder : ISerializable
    {
        internal AOrder() { }

        public Dictionary<string, object> Properties { get; private set; } = new Dictionary<string, object>();

        protected AOrder(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            Properties = (Dictionary<string, object>)info.GetValue("Properties", typeof(Dictionary<string, object>));
        }

        public abstract string GetAcronym();
        public abstract string GetDefinition();
        public int GetLength()
        {
            return System.Text.Encoding.ASCII.GetByteCount(GetAcronym());
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Properties", Properties);
        }
    }

    /// <summary>
    /// Flyweight orders provider
    /// </summary>
    public static class OrderProvider
    {
        public readonly static AOrder SearchBattleAgainstAI = new SearchBattleAgainstAIOrder();
        public readonly static AOrder SearchBattleAgainstPlayer = new SearchBattleAgainstPlayerOrder();
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

    [Serializable]
    public class SearchBattleAgainstAIOrder : AOrder
    {
        public PlayerType PlayerType { get; set; }

        protected SearchBattleAgainstAIOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            PlayerType = (PlayerType)info.GetValue("PlayerType", typeof(PlayerType));
        }

        public override string GetAcronym()
        {
            return "SBAA";
        }

        public override string GetDefinition()
        {
            return "Search a battle against an AI";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("PlayerType", PlayerType);
        }

        internal SearchBattleAgainstAIOrder() { }
    }

    [Serializable]
    public class SearchBattleAgainstPlayerOrder : AOrder
    {
        public PlayerType PlayerType { get; set; }

        protected SearchBattleAgainstPlayerOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            PlayerType = (PlayerType)info.GetValue("PlayerType", typeof(PlayerType));
        }

        public override string GetAcronym()
        {
            return "SBAP";
        }

        public override string GetDefinition()
        {
            return "Search a battle against a player";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("PlayerType", PlayerType);
        }

        internal SearchBattleAgainstPlayerOrder() { }
    }

    [Serializable]
    public class RegisterSuccessfulOrder : AOrder
    {
        protected RegisterSuccessfulOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

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

    [Serializable]
    public class OpponentFoundOrder : AOrder
    {
        protected OpponentFoundOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

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

    [Serializable]
    public class StartOfTheGameOrder : AOrder
    {
        protected StartOfTheGameOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

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

    [Serializable]
    public class EndOfTheGameOrder : AOrder
    {
        protected EndOfTheGameOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

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

    [Serializable]
    public class BlackAssignedOrder : AOrder
    {
        protected BlackAssignedOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

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

    [Serializable]
    public class WhiteAssignedOrder : AOrder
    {
        protected WhiteAssignedOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

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

    [Serializable]
    public class PlayerBeginOrder : AOrder
    {
        protected PlayerBeginOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

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

    [Serializable]
    public class PlayerAwaitOrder : AOrder
    {
        protected PlayerAwaitOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

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

    [Serializable]
    public class PlayerStateOrder : AOrder
    {
        protected PlayerStateOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

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

    [Serializable]
    public class OpponentDisconnectedOrder : AOrder
    {
        protected OpponentDisconnectedOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

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

    [Serializable]
    public class OpponentConnectionLostOrder : AOrder
    {
        protected OpponentConnectionLostOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

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

    [Serializable]
    public class NextTurnOrder : AOrder
    {
        protected NextTurnOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

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

    [Serializable]
    public class GetCurrentGameStateOrder : AOrder
    {
        protected GetCurrentGameStateOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

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

    [Serializable]
    public class GetPreviousGameStateOrder : AOrder
    {
        protected GetPreviousGameStateOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

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

    [Serializable]
    public class PlayMoveOrder : AOrder
    {
        public Tuple<char, int> Coords { get; set; }

        protected PlayMoveOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Coords = (Tuple<char, int>)info.GetValue("Coords", typeof(Tuple<char, int>));
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