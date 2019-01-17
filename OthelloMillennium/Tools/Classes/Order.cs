using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tools.Classes
{
    [Serializable]
    public abstract class AOrder : ISerializable
    {
        public AOrder() { }

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

        public SearchBattleAgainstAIOrder() { }
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

        public SearchBattleAgainstPlayerOrder() { }
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

        public RegisterSuccessfulOrder() { }
    }

    [Serializable]
    public class OpponentFoundOrder : AOrder
    {
        public OthelloTCPClient Opponent;

        protected OpponentFoundOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Opponent = (OthelloTCPClient)info.GetValue("Opponent", typeof(OthelloTCPClient));
        }

        public override string GetAcronym()
        {
            return "OF";
        }

        public override string GetDefinition()
        {
            return "Inform current player that an opponent has been found";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Opponent", Opponent);
        }

        public OpponentFoundOrder() { }
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

        public StartOfTheGameOrder() { }
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

        public EndOfTheGameOrder() { }
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

        public BlackAssignedOrder() { }
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

        public WhiteAssignedOrder() { }
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

        public PlayerBeginOrder() { }
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

        public PlayerAwaitOrder() { }
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

        public PlayerStateOrder() { }
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

        public OpponentDisconnectedOrder() { }
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

        public OpponentConnectionLostOrder() { }
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

        public NextTurnOrder() { }
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

        public GetCurrentGameStateOrder() { }
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

        public GetPreviousGameStateOrder() { }
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

        public PlayMoveOrder() { }
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

        public Order() { }
    }
    */
    #endregion
}