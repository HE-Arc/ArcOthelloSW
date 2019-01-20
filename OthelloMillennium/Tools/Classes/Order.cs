using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tools.Classes
{
    [Serializable]
    public abstract class Order : ISerializable
    {
        public Order() { }

        public Dictionary<string, object> Properties { get; private set; } = new Dictionary<string, object>();

        protected Order(SerializationInfo info, StreamingContext context)
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
    public class RegisterOrder : Order
    {
        public string Name { get; private set; }
        public PlayerType PlayerType { get; private set; }

        public RegisterOrder(PlayerType playerType, string name) {
            PlayerType = playerType;
            Name = name;
        }

        protected RegisterOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            PlayerType = (PlayerType)info.GetValue("PlayerType", typeof(PlayerType));
            Name = (string)info.GetValue("Name", typeof(string));
        }

        public override string GetAcronym()
        {
            return "RO";
        }

        public override string GetDefinition()
        {
            return "Register to the matchmaking";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("PlayerType", PlayerType);
            info.AddValue("Name", Name);
        }
    }

    [Serializable]
    public class AvatarChangedOrder : Order
    {
        public int AvatarID { get; private set; }

        public AvatarChangedOrder(int avatarID)
        {
            AvatarID = avatarID;
        }

        protected AvatarChangedOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            AvatarID = (int)info.GetValue("AvatarID", typeof(int));
        }

        public override string GetAcronym()
        {
            return "ACO";
        }

        public override string GetDefinition()
        {
            return "Inform an avatar's selection change";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("AvatarID", AvatarID);
        }
    }

    [Serializable]
    public class SearchOrder : Order
    {
        public PlayerType OpponentType { get; private set; }


        public SearchOrder(PlayerType opponentType)
        {
            OpponentType = opponentType;
        }

        protected SearchOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            OpponentType = (PlayerType)info.GetValue("OpponentType", typeof(PlayerType));
        }

        public override string GetAcronym()
        {
            return "SO";
        }

        public override string GetDefinition()
        {
            return "Search a battle against an opponent of the specified type";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("OpponentType", OpponentType);
        }
    }

    [Serializable]
    public class RegisterSuccessfulOrder : Order
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
    public class OpponentFoundOrder : Order
    {
        public OthelloTCPClient Opponent { get; private set; }

        public OpponentFoundOrder(OthelloTCPClient opponent) {
            Opponent = opponent;
        }

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
    }

    [Serializable]
    public class StartOfTheGameOrder : Order
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
    public class EndOfTheGameOrder : Order
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
    public class BlackAssignedOrder : Order
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
    public class WhiteAssignedOrder : Order
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
    public class PlayerBeginOrder : Order
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
    public class PlayerAwaitOrder : Order
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
    public class PlayerStateOrder : Order
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
    public class OpponentDisconnectedOrder : Order
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
    public class OpponentConnectionLostOrder : Order
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
    public class GetCurrentGameStateOrder : Order
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
    public class SaveOrder : Order
    {
        protected SaveOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "SS";
        }

        public override string GetDefinition()
        {
            return "Ask the server to save the game";
        }

        public SaveOrder() { }
    }

    [Serializable]
    public class LoadOrder : Order
    {
        public LoadOrder() { }

        protected LoadOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "LS";
        }

        public override string GetDefinition()
        {
            return "Ask the server to load the game";
        }
    }

    [Serializable]
    public class UndoOrder : Order
    {
        protected UndoOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "UDS";
        }

        public override string GetDefinition()
        {
            return "Ask the server to undo the last move";
        }

        public UndoOrder() { }
    }

    [Serializable]
    public class RedoOrder : Order
    {
        protected RedoOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "RDS";
        }

        public override string GetDefinition()
        {
            return "Ask the server to redo the last move";
        }

        public RedoOrder() { }
    }

    [Serializable]
    public class DeniedOrder : Order
    {
        protected DeniedOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "DS";
        }

        public override string GetDefinition()
        {
            return "inform the player that this action is denied";
        }

        public DeniedOrder() { }
    }

    [Serializable]
    public class PlayMoveOrder : Order
    {
        public Tuple<char, int> Coords { get; set; }

        public PlayMoveOrder(Tuple<char, int> coords) {
            Coords = coords;
        }

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