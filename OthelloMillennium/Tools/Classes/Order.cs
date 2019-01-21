using System;
using System.Runtime.Serialization;

namespace Tools
{
    [Serializable]
    public abstract class Order : ISerializable
    {
        public Order() { }

        protected Order(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
        }

        public abstract string GetAcronym();
        public abstract string GetDefinition();

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }
    }

    #region Handshake

    [Serializable]
    public class RegisterOrder : Order
    {
        public string Name { get; private set; }
        public int PlayerType { get; private set; }

        public RegisterOrder(PlayerType playerType, string name) {
            PlayerType = (int)playerType;
            Name = name;
        }

        protected RegisterOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            PlayerType = (int)info.GetValue("PlayerType", typeof(int));
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
    public class SearchOrder : Order
    {
        public int OpponentType { get; private set; }


        public SearchOrder(PlayerType opponentType)
        {
            OpponentType = (int)opponentType;
        }

        protected SearchOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            OpponentType = (int)info.GetValue("OpponentType", typeof(int));
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
    public class OpponentFoundOrder : Order
    {
        public Data OpponentData { get; private set; }

        public OpponentFoundOrder(Data opponentData)
        {
            OpponentData = opponentData;
        }

        protected OpponentFoundOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            OpponentData = (Data)info.GetValue("OpponentData", typeof(Data));
        }

        public override string GetAcronym()
        {
            return "OFO";
        }

        public override string GetDefinition()
        {
            return "Inform current player that an opponent has been found";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("OpponentData", OpponentData);
        }
    }

    #endregion

    #region GameLogic

    [Serializable]
    public class ReadyOrder : Order
    {
        protected ReadyOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "RO";
        }

        public override string GetDefinition()
        {
            return "Inform the gameHandler that the player is ready";
        }

        public ReadyOrder() { }
    }

    [Serializable]
    public class GameReadyOrder : Order
    {
        protected GameReadyOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "GRO";
        }

        public override string GetDefinition()
        {
            return "Inform the player that the game is ready";
        }

        public GameReadyOrder() { }
    }

    [Serializable]
    public class GameStartedOrder : Order
    {
        protected GameStartedOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "GSO";
        }

        public override string GetDefinition()
        {
            return "Inform the player that the game started";
        }

        public GameStartedOrder() { }
    }

    [Serializable]
    public class GameEndedOrder : Order
    {
        protected GameEndedOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "GEO";
        }

        public override string GetDefinition()
        {
            return "Inform current player that the game has ended";
        }

        public GameEndedOrder() { }
    }

    [Serializable]
    public class PlayMoveOrder : Order
    {
        public Tuple<char, int> Coords { get; set; }

        public PlayMoveOrder(Tuple<char, int> coords)
        {
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

    #endregion

    #region Synchronization

    [Serializable]
    public class GetDataOrder : Order
    {
        public GetDataOrder() { }

        protected GetDataOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "GDO";
        }

        public override string GetDefinition()
        {
            return "Ask the server for data's";
        }
    }

    [Serializable]
    public class GetCurrentGameStateOrder : Order
    {
        protected GetCurrentGameStateOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "GCGS";
        }

        public override string GetDefinition()
        {
            return "Ask the server for the current gameState";
        }

        public GetCurrentGameStateOrder() { }
    }

    #region Opponent

    /// <summary>
    /// Client send this whenever it changes his datas
    /// </summary>
    [Serializable]
    public class OpponentDataChangedOrder : Order
    {
        public Data Data { get; private set; }

        public OpponentDataChangedOrder(Data data)
        {
            Data = data;
        }

        protected OpponentDataChangedOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Data = (Data)info.GetValue("Data", typeof(Data));
        }

        public override string GetAcronym()
        {
            return "ODCO";
        }

        public override string GetDefinition()
        {
            return "Opponent Data changed";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Data", Data);
        }
    }

    [Serializable]
    public class GetOpponentDataOrder : Order
    {
        public GetOpponentDataOrder() { }

        protected GetOpponentDataOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "GODO";
        }

        public override string GetDefinition()
        {
            return "Ask the server for opponent's data";
        }
    }

    #endregion

    #endregion

    #region TCP

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

    #endregion

    #region GameManager

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

    #endregion

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
}